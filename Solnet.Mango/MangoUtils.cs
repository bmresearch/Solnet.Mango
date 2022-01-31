using Solnet.Mango.Models;
using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Utilities;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango
{
    /// <summary>
    /// Implements extension methods and utilities to easily operate with Mango.
    /// </summary>
    public static class MangoUtils
    {

        /// <summary>
        /// Humanizes a native <see cref="I80F48"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decimals">The number of decimals for the asset.</param>
        /// <returns>The humanized value.</returns>
        public static double HumanizeNative(double value, byte decimals)
        {
            return value / Math.Pow(10, decimals);
        }

        /// <summary>
        /// Derives the <see cref="PublicKey"/> of a <see cref="MangoAccount"/> of <see cref="MetaData.Version"/> 1.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The derived <see cref="PublicKey"/> if it was found, otherwise null.</returns>
        public static PublicKey DeriveMangoAccountAddress(PublicKey programIdKey, PublicKey mangoGroup, PublicKey owner, ulong accountNumber)
        {
            byte[] accountNumByteSeed = new byte[8];
            accountNumByteSeed.WriteU64(accountNumber, 0);

            bool success = AddressExtensions.TryFindProgramAddress(new List<byte[]>() { Constants.DevNetMangoGroup, owner, accountNumByteSeed },
                programIdKey, out byte[] mangoAccount, out _);

            return success ? new(mangoAccount) : null;
        }

        /// <summary>
        /// Splits the open orders funds.
        /// </summary>
        /// <param name="openOrdersAccount">The open orders account.</param>
        /// <returns>The open orders stats.</returns>
        public static OpenOrdersStats SplitOpenOrders(OpenOrdersAccount openOrdersAccount)
        {
            double quoteFree = openOrdersAccount.QuoteTokenFree + openOrdersAccount.ReferrerRebatesAccrued;
            double quoteLocked = openOrdersAccount.QuoteTokenTotal - openOrdersAccount.QuoteTokenFree;

            double baseFree = openOrdersAccount.BaseTokenFree;
            double baseLocked = openOrdersAccount.BaseTokenTotal - openOrdersAccount.BaseTokenFree;

            return new OpenOrdersStats
            {
                QuoteFree = quoteFree,
                QuoteLocked = quoteLocked,
                BaseLocked = baseLocked,
                BaseFree = baseFree
            };
        }

        /// <summary>
        /// Gets the weights to calculate account health ratios.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The index of the market.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The weights structure.</returns>
        public static Weights GetWeights(MangoGroup mangoGroup, int tokenIndex, HealthType healthType)
        {
            return healthType switch
            {
                HealthType.Maintenance => new Weights
                {
                    SpotAssetWeight = mangoGroup.SpotMarkets[tokenIndex].MaintenanceAssetWeight.Value,
                    SpotLiabilityWeight = mangoGroup.SpotMarkets[tokenIndex].MaintenanceLiabilityWeight.Value,
                    PerpAssetWeight = mangoGroup.PerpetualMarkets[tokenIndex].MaintenanceAssetWeight.Value,
                    PerpLiabilityWeight = mangoGroup.PerpetualMarkets[tokenIndex].MaintenanceLiabilityWeight.Value,
                },
                HealthType.Initialization => new Weights
                {
                    SpotAssetWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationAssetWeight.Value,
                    SpotLiabilityWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationLiabilityWeight.Value,
                    PerpAssetWeight = mangoGroup.PerpetualMarkets[tokenIndex].InitializationAssetWeight.Value,
                    PerpLiabilityWeight = mangoGroup.PerpetualMarkets[tokenIndex].InitializationLiabilityWeight.Value,
                },
                _ => new Weights
                {
                    SpotAssetWeight = 1,
                    SpotLiabilityWeight = 1,
                    PerpAssetWeight = 1,
                    PerpLiabilityWeight = 1,
                }
            };
        }
    }
}