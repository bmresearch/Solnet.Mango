using Solnet.Mango.Models;
using Solnet.Mango.Models.Perpetuals;
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
        public static I80F48 HumanizeNative(I80F48 value, byte decimals)
        {
            return value / new I80F48((decimal) Math.Pow(10, decimals));
        }

        /// <summary>
        /// Converts a humanized value to a native <see cref="I80F48"/> value.
        /// </summary>
        /// <param name="triggerPrice">The trigger price.</param>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <param name="quoteDecimals">The quote decimals.</param>
        /// <returns>The native value.</returns>
        public static I80F48 TriggerPriceToNative(decimal triggerPrice, byte baseDecimals, byte quoteDecimals)
        {
            decimal uiToNative = (decimal)Math.Pow(10, quoteDecimals - baseDecimals);
            return new I80F48(triggerPrice * uiToNative);
        }

        /// <summary>
        /// Converts a humanized value to a native <see cref="I80F48"/> value.
        /// </summary>
        /// <param name="triggerPrice">The trigger price.</param>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <param name="quoteDecimals">The quote decimals.</param>
        /// <returns>The native value.</returns>
        public static decimal TriggerPriceToNumber(I80F48 triggerPrice, byte baseDecimals, byte quoteDecimals)
        {
            decimal nativeToUi = (decimal)Math.Pow(10, baseDecimals - quoteDecimals);
            return triggerPrice.ToDecimal() * nativeToUi;
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

            _ = PublicKey.TryFindProgramAddress(new List<byte[]>() { mangoGroup, owner, accountNumByteSeed },
                programIdKey, out PublicKey mangoAccount, out byte _);

            return mangoAccount;
        }

        /// <summary>
        /// Derives the <see cref="PublicKey"/> of a <see cref="AdvancedOrdersAccount"/>.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <returns>The derived <see cref="PublicKey"/> if it was found, otherwise null.</returns>
        public static PublicKey DeriveAdvancedOrdersAccountAddress(PublicKey programIdKey, PublicKey mangoAccount)
        {
            _ = PublicKey.TryFindProgramAddress(new List<byte[]>() { mangoAccount },
                programIdKey, out PublicKey advancedOrdersAccount, out _);

            return advancedOrdersAccount;
        }

        /// <summary>
        /// Derives the <see cref="PublicKey"/> of a referrer memory account.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <returns>The derived <see cref="PublicKey"/> if it was found, otherwise null.</returns>
        public static PublicKey DeriveReferrerMemory(PublicKey programIdKey, PublicKey mangoAccount)
        {
            _ = PublicKey.TryFindProgramAddress(new List<byte[]>() { 
                mangoAccount, 
                Encoding.UTF8.GetBytes("ReferrerMemory") 
            }, programIdKey, out PublicKey referrerMemory, out _);

            return referrerMemory;
        }

        /// <summary>
        /// Derives the <see cref="PublicKey"/> of a referrer pda.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="referrerId">The referrer id.</param>
        /// <returns>The derived <see cref="PublicKey"/> if it was found, otherwise null.</returns>
        public static PublicKey DeriveReferrerPda(PublicKey programIdKey, PublicKey mangoGroup, string referrerId)
        {
            byte[] buffer = new byte[Constants.InfoLength];
            byte[] encoded = Encoding.UTF8.GetBytes(referrerId);
            if (encoded.Length > Constants.InfoLength)
                throw new ArgumentException($"referrer id is too long, must be less or equal than {Constants.InfoLength} bytes",
                    nameof(referrerId));
            encoded.CopyTo(buffer, 0);

            _ = PublicKey.TryFindProgramAddress(new List<byte[]>() {
                mangoGroup,
                Encoding.UTF8.GetBytes("ReferrerIdRecord"),
                buffer
            }, programIdKey, out PublicKey referrerIdRecord, out _);

            return referrerIdRecord;
        }

        /// <summary>
        /// Splits the open orders funds.
        /// </summary>
        /// <param name="openOrdersAccount">The open orders account.</param>
        /// <returns>The open orders stats.</returns>
        public static OpenOrdersStats SplitOpenOrders(OpenOrdersAccount openOrdersAccount)
        {
            I80F48 quoteFree = new((decimal)openOrdersAccount.QuoteTokenFree + openOrdersAccount.ReferrerRebatesAccrued);
            I80F48 quoteLocked = new((decimal)openOrdersAccount.QuoteTokenTotal - openOrdersAccount.QuoteTokenFree);

            I80F48 baseFree = new((decimal)openOrdersAccount.BaseTokenFree);
            I80F48 baseLocked = new((decimal)openOrdersAccount.BaseTokenTotal - openOrdersAccount.BaseTokenFree);

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
                    SpotAssetWeight = mangoGroup.SpotMarkets[tokenIndex].MaintenanceAssetWeight,
                    SpotLiabilityWeight = mangoGroup.SpotMarkets[tokenIndex].MaintenanceLiabilityWeight,
                    PerpAssetWeight = mangoGroup.PerpetualMarkets[tokenIndex].MaintenanceAssetWeight,
                    PerpLiabilityWeight = mangoGroup.PerpetualMarkets[tokenIndex].MaintenanceLiabilityWeight,
                },
                HealthType.Initialization => new Weights
                {
                    SpotAssetWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationAssetWeight,
                    SpotLiabilityWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationLiabilityWeight,
                    PerpAssetWeight = mangoGroup.PerpetualMarkets[tokenIndex].InitializationAssetWeight,
                    PerpLiabilityWeight = mangoGroup.PerpetualMarkets[tokenIndex].InitializationLiabilityWeight,
                },
                _ => new Weights
                {
                    SpotAssetWeight = new(1m),
                    SpotLiabilityWeight = new(1m),
                    PerpAssetWeight = new(1m),
                    PerpLiabilityWeight = new(1m),
                }
            };
        }
    }
}