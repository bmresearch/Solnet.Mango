using Microsoft.Extensions.Logging;
using Solnet.Mango.Models.Banks;
using Solnet.Mango.Models.Caches;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a cross margined mango account.
    /// </summary>
    public class MangoAccount
    {
        /// <summary>
        /// Represents the layout of the <see cref="MangoAccount"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="MangoAccount"/> structure.
            /// </summary>
            public const int Length = 4296;

            /// <summary>
            /// The offset at which the metadata structure begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the mango group public key value begins.
            /// </summary>
            internal const int MangoGroupOffset = 8;

            /// <summary>
            /// The offset at which the owner's public key value begins.
            /// </summary>
            internal const int OwnerOffset = 40;

            /// <summary>
            /// The offset at which the list of boolean values which define whether an asset is margined begins.
            /// </summary>
            internal const int InMarginBasketOffset = 72;

            /// <summary>
            /// The offset at which the number of assets in the margin basket value begins.
            /// </summary>
            internal const int NumInMarginBasketOffset = 87;

            /// <summary>
            /// The offset at which the number of assets in the margin basket value begins.
            /// </summary>
            internal const int DepositsOffset = 88;

            /// <summary>
            /// The offset at which the number of assets in the margin basket value begins.
            /// </summary>
            internal const int BorrowsOffset = 344;

            /// <summary>
            /// The offset at which the number of assets in the margin basket value begins.
            /// </summary>
            internal const int SpotOpenOrdersOffset = 600;

            /// <summary>
            /// The offset at which the perpetual accounts begin.
            /// </summary>
            internal const int PerpetualAccountsOffset = 1080;

            /// <summary>
            /// The offset at which the markets for the open orders begin.
            /// </summary>
            internal const int OrderMarketOffset = 2520;

            /// <summary>
            /// The offset at which the sides for the open orders begin.
            /// </summary>
            internal const int OrderSideOffset = 2584;

            /// <summary>
            /// The offset at which the order ids begin.
            /// </summary>
            internal const int OrderIdsOffset = 2648;

            /// <summary>
            /// The offset at which the client order ids begin.
            /// </summary>
            internal const int ClientOrderIdsOffset = 3672;

            /// <summary>
            /// The offset at which the account's MSRM amount begins.
            /// </summary>
            internal const int MegaSerumAmountOffset = 4184;

            /// <summary>
            /// The offset at which the boolean which defines if the account is being liquidated begins.
            /// </summary>
            internal const int BeingLiquidatedOffset = 4192;

            /// <summary>
            /// The offset at which the boolean which defines if the account is bankrupt begins.
            /// </summary>
            internal const int BankruptOffset = 4193;

            /// <summary>
            /// The offset at which the account info begins.
            /// </summary>
            internal const int InfoOffset = 4194;

            /// <summary>
            /// The offset at which the advanced orders account public key begins.
            /// </summary>
            internal const int AdvancedOrdersOffset = 4226;

            /// <summary>
            /// The offset at which the upgradeable boolean begins.
            /// </summary>
            internal const int NotUpgradeableOffset = 4258;

            /// <summary>
            /// The offset at which the delegate public key begins.
            /// </summary>
            internal const int DelegateOffset = 4259;
        }

        /// <summary>
        /// The account's metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The associated <see cref="MangoGroup"/> public key.
        /// </summary>
        public PublicKey MangoGroup;

        /// <summary>
        /// The owner's public key.
        /// </summary>
        public PublicKey Owner;

        /// <summary>
        /// Whether the asset at the corresponding index is in the margin basket.
        /// </summary>
        public List<bool> InMarginBasket;

        /// <summary>
        /// The number of assets in the margin basket.
        /// </summary>
        public byte NumInMarginBasket;

        /// <summary>
        /// A list with the amount of deposits per token.
        /// </summary>
        public List<I80F48> Deposits;

        /// <summary>
        /// A list with the amount of borrows per token.
        /// </summary>
        public List<I80F48> Borrows;

        /// <summary>
        /// A list with the open orders account public keys for each market.
        /// </summary>
        public List<PublicKey> SpotOpenOrders;

        /// <summary>
        /// A list with the <see cref="PerpAccount"/>.
        /// </summary>
        public List<PerpAccount> PerpetualAccounts;

        /// <summary>
        /// A list with the markets of the perp orders.
        /// </summary>
        public List<byte> OrderMarket;

        /// <summary>
        /// A list with the side of the perp orders.
        /// </summary>
        public List<Side> OrderSide;

        /// <summary>
        /// A list with the order ids of perp orders.
        /// </summary>
        public List<BigInteger> OrderIds;

        /// <summary>
        /// A list with the client order ids of perp orders.
        /// </summary>
        public List<ulong> ClientOrderIds;

        /// <summary>
        /// The amount of MSRM.
        /// </summary>
        public ulong MegaSerumAmount;

        /// <summary>
        /// Whether the account is being liquidated.
        /// </summary>
        public bool BeingLiquidated;

        /// <summary>
        /// Whether the account is bankrupt.
        /// </summary>
        public bool Bankrupt;

        /// <summary>
        /// Account info.
        /// </summary>
        public string AccountInfo { get; set; }

        /// <summary>
        /// The public key of the <see cref="Models.AdvancedOrdersAccount"/>.
        /// </summary>
        public PublicKey AdvancedOrdersAccount { get; set; }

        /// <summary>
        /// Whether this account can be upgraded to v1 so it can be closed.
        /// </summary>
        public bool NotUpgradeable { get; set; }

        /// <summary>
        /// The alternative authority/signer of transactions associated with this mango account.
        /// </summary>
        public PublicKey Delegate { get; set; }

        /// <summary>
        /// The loaded open orders accounts.
        /// </summary>
        public List<OpenOrdersAccount> OpenOrdersAccounts;

        /// <summary>
        /// Load the spot open orders accounts. This is an asynchronous operation.
        /// </summary>
        /// <param name="rpcClient">The rpc client.</param>
        /// <param name="logger">A logger instance.</param>
        public async Task<RequestResult<ResponseValue<List<AccountInfo>>>> LoadOpenOrdersAccountsAsync(
            IRpcClient rpcClient, ILogger logger = null)
        {
            IList<PublicKey> filteredOpenOrders =
                SpotOpenOrders.Where(x => !x.Equals(SystemProgram.ProgramIdKey)).ToList();
            RequestResult<ResponseValue<List<AccountInfo>>> openOrdersAccounts =
                await rpcClient.GetMultipleAccountsAsync(filteredOpenOrders.Select(x => x.Key).ToList());
            if (!openOrdersAccounts.WasRequestSuccessfullyHandled)
            {
                logger?.LogInformation($"Could not fetch open orders accounts.");
                return openOrdersAccounts; 
            }
            logger?.LogInformation($"Successfully fetched {openOrdersAccounts.Result.Value.Count} open orders accounts.");

            SpotOpenOrders.ForEach(key =>
            {
                int keyIndex = filteredOpenOrders.IndexOf(key);
                if (keyIndex == -1)
                {
                    OpenOrdersAccounts.Add(null);
                    return;
                }

                OpenOrdersAccounts.Add(OpenOrdersAccount.Deserialize(Convert.FromBase64String(openOrdersAccounts.Result.Value[keyIndex].Data[0])));
            });

            return openOrdersAccounts;
        }

        /// <summary>
        /// Load the spot open orders accounts.
        /// </summary>
        /// <param name="rpcClient">The rpc client.</param>
        /// <param name="logger">A logger instance.</param>
        public RequestResult<ResponseValue<List<AccountInfo>>> LoadOpenOrdersAccounts(IRpcClient rpcClient,
            ILogger logger = null) => LoadOpenOrdersAccountsAsync(rpcClient, logger).Result;

        /// <summary>
        /// Gets the list of open orders in perpetual markets..
        /// </summary>
        /// <returns>The list of open orders.</returns>
        public List<PerpetualOpenOrder> GetOrders()
        {
            List<PerpetualOpenOrder> orders = new(Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                if (OrderIds[i].IsZero) continue;
                orders.Add(new PerpetualOpenOrder
                {
                    OrderId = OrderIds[i],
                    ClientOrderId = ClientOrderIds[i],
                    Side = OrderSide[i],
                    MarketIndex = OrderMarket[i]
                });
            }

            return orders;
        }

        /// <summary>
        /// Gets the amount of native deposits.
        /// </summary>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native deposit.</returns>
        public I80F48 GetNativeDeposit(RootBank rootBank, int tokenIndex)
        {
            return rootBank.DepositIndex *
                   Deposits[tokenIndex];
        }

        /// <summary>
        /// Gets the amount of native deposits.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native deposit.</returns>
        public I80F48 GetNativeDeposit(RootBankCache rootBankCache, int tokenIndex)
        {
            return rootBankCache.DepositIndex *
                   Deposits[tokenIndex];
        }

        /// <summary>
        /// Gets the amount of native borrows.
        /// </summary>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native borrow.</returns>
        public I80F48 GetNativeBorrow(RootBank rootBank, int tokenIndex)
        {
            return rootBank.BorrowIndex *
                   Borrows[tokenIndex];
        }

        /// <summary>
        /// Gets the amount of native borrows.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native borrow.</returns>
        public I80F48 GetNativeBorrow(RootBankCache rootBankCache, int tokenIndex)
        {
            return rootBankCache.BorrowIndex *
                   Borrows[tokenIndex];
        }

        /// <summary>
        /// Gets the amount deposited humanized for ui display.
        /// </summary>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount deposited humanized for ui display.</returns>
        public I80F48 GetUiDeposit(RootBank rootBank, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeDeposit(rootBank, tokenIndex).Floor(),
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the amount deposited humanized for ui display.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount deposited humanized for ui display.</returns>
        public I80F48 GetUiDeposit(RootBankCache rootBankCache, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeDeposit(rootBankCache, tokenIndex),
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the amount borrowed humanized for ui display.
        /// </summary>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount borrowed humanized for ui display.</returns>
        public I80F48 GetUiBorrow(RootBank rootBank, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeBorrow(rootBank, tokenIndex).Ceil(),
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the amount borrowed humanized for ui display.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount borrowed humanized for ui display.</returns>
        public I80F48 GetUiBorrow(RootBankCache rootBankCache, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeBorrow(rootBankCache, tokenIndex),
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Deposits minus borrows in native terms.
        /// </summary>
        /// <param name="bankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The net deposits.</returns>
        public I80F48 GetNet(RootBankCache bankCache, int tokenIndex)
        {
            return (Deposits[tokenIndex] * bankCache.DepositIndex) -
                   (Borrows[tokenIndex] * bankCache.BorrowIndex);
        }

        /// <summary>
        /// Deposits minus borrows in native terms.
        /// </summary>
        /// <param name="bankCache">The root bank cache.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The net deposits.</returns>
        public I80F48 GetUiNet(RootBankCache bankCache, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(Deposits[tokenIndex] * bankCache.DepositIndex -
                                             Borrows[tokenIndex] * bankCache.BorrowIndex,
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the account's health.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The health.</returns>
        public I80F48 GetHealth(MangoGroup mangoGroup, MangoCache mangoCache, HealthType healthType)
        {
            (List<I80F48> spot, List<I80F48> perps, I80F48 quote) = GetHealthComponents(mangoGroup, mangoCache);

            return GetHealthFromComponents(mangoGroup, mangoCache, spot, perps, quote, healthType);
        }

        /// <summary>
        /// Gets the value of the spot holdings.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <param name="assetWeight">The asset weight-</param>
        /// <returns>The spot value.</returns>
        public I80F48 GetSpotValue(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex, I80F48 assetWeight)
        {
            I80F48 assetsValue = I80F48.Zero;
            I80F48 price = mangoGroup.GetPrice(mangoCache, tokenIndex);

            I80F48 deposits =
                GetUiDeposit(mangoCache.RootBankCaches[tokenIndex],
                    mangoGroup, tokenIndex) * price *
                assetWeight;
            assetsValue += deposits;

            if (OpenOrdersAccounts.Count == 0)
                return assetsValue;

            OpenOrdersAccount openOrdersAccount = OpenOrdersAccounts[tokenIndex];
            if (openOrdersAccount == null)
                return assetsValue;

            assetsValue += MangoUtils.HumanizeNative(new I80F48((decimal) openOrdersAccount.BaseTokenTotal),
                mangoGroup.Tokens[tokenIndex].Decimals) * price * assetWeight;
            assetsValue += MangoUtils.HumanizeNative(new I80F48((decimal) openOrdersAccount.QuoteTokenTotal + openOrdersAccount.ReferrerRebatesAccrued),
                mangoGroup.GetQuoteTokenInfo().Decimals) * price * assetWeight;

            return assetsValue;
        }

        /// <summary>
        /// Gets the assets value.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The assets value.</returns>
        public I80F48 GetAssetsValue(MangoGroup mangoGroup, MangoCache mangoCache, HealthType? healthType = null)
        {
            I80F48 assetsValue = GetUiDeposit(mangoCache.RootBankCaches[Constants.QuoteIndex], mangoGroup,
                Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                I80F48 assetWeight = healthType switch
                {
                    HealthType.Maintenance => mangoGroup.SpotMarkets[i].MaintenanceAssetWeight,
                    HealthType.Initialization => mangoGroup.SpotMarkets[i].InitializationAssetWeight,
                    _ => I80F48.One
                };
                var spotValue = GetSpotValue(mangoGroup, mangoCache, i, assetWeight);
                assetsValue += spotValue;
                var perpsValue = MangoUtils.HumanizeNative(
                    PerpetualAccounts[i].GetAssetValue(
                        mangoGroup.PerpetualMarkets[i], 
                        mangoCache.PriceCaches[i].Price,
                        mangoCache.PerpetualMarketCaches[i].ShortFunding,
                        mangoCache.PerpetualMarketCaches[i].LongFunding),
                    mangoGroup.GetQuoteTokenInfo().Decimals);
                assetsValue += perpsValue;
            }

            return assetsValue;
        }

        /// <summary>
        /// Gets the liabilities value.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The liabilities value.</returns>
        public I80F48 GetLiabilitiesValue(MangoGroup mangoGroup, MangoCache mangoCache, HealthType? healthType = null)
        {
            I80F48 liabilitiesValue = GetUiBorrow(mangoCache.RootBankCaches[Constants.QuoteIndex], mangoGroup,
                Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                I80F48 liabilityWeight = healthType switch
                {
                    HealthType.Maintenance => mangoGroup.SpotMarkets[i].MaintenanceLiabilityWeight,
                    HealthType.Initialization => mangoGroup.SpotMarkets[i].InitializationLiabilityWeight,
                    _ => I80F48.One
                };
                var price = mangoGroup.GetPrice(mangoCache, i);
                var borrowedValue = GetUiBorrow(mangoCache.RootBankCaches[i], mangoGroup, i) *
                                    price * liabilityWeight;
                var borrowedValue2 = GetUiBorrow(mangoCache.RootBankCaches[i], mangoGroup, i) *
                                    (price * liabilityWeight);
                liabilitiesValue += borrowedValue;
                var perpsValue = MangoUtils.HumanizeNative(
                    PerpetualAccounts[i].GetLiabilitiesValue(
                        mangoGroup.PerpetualMarkets[i],
                        mangoCache.PriceCaches[i].Price,
                        mangoCache.PerpetualMarketCaches[i].ShortFunding,
                        mangoCache.PerpetualMarketCaches[i].LongFunding),
                    mangoGroup.GetQuoteTokenInfo().Decimals);
                liabilitiesValue += perpsValue;
            }

            return liabilitiesValue;
        }

        /// <summary>
        /// Gets the native liabilities value.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The native liabilities value.</returns>
        public I80F48 GetNativeLiabilitiesValue(MangoGroup mangoGroup, MangoCache mangoCache, HealthType? healthType = null)
        {
            I80F48 liabilitiesValue =
                GetNativeBorrow(mangoCache.RootBankCaches[Constants.QuoteIndex], Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                I80F48 liabilityWeight = healthType switch
                {
                    HealthType.Maintenance => mangoGroup.SpotMarkets[i].MaintenanceLiabilityWeight,
                    HealthType.Initialization => mangoGroup.SpotMarkets[i].InitializationLiabilityWeight,
                    _ => I80F48.One
                };
                liabilitiesValue += GetNativeBorrow(mangoCache.RootBankCaches[i], i) *
                                    mangoGroup.GetPrice(mangoCache, i) * liabilityWeight;
                liabilitiesValue += PerpetualAccounts[i].GetAssetValue(
                    mangoGroup.PerpetualMarkets[i],
                    mangoCache.PriceCaches[i].Price,
                    mangoCache.PerpetualMarketCaches[i].ShortFunding,
                    mangoCache.PerpetualMarketCaches[i].LongFunding);
            }

            return liabilitiesValue;
        }

        /// <summary>
        /// Gets the weighted asset and liabilities values.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="spot">The spot components.</param>
        /// <param name="perps">The perp components.</param>
        /// <param name="quote">The quote component.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The weighted assets and liabilities values.</returns>
        public (I80F48 SpotHealth, I80F48 PerpHealth) GetWeightedAssetsLiabilityValues(MangoGroup mangoGroup,
            MangoCache mangoCache, List<I80F48> spot, List<I80F48> perps, I80F48 quote, HealthType healthType)
        {
            I80F48 assets = I80F48.Zero;
            I80F48 liabilities = I80F48.Zero;

            if (quote > I80F48.Zero)
            {
                assets += quote;
            }
            else
            {
                liabilities += -quote;
            }

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                Weights w = MangoUtils.GetWeights(mangoGroup, i, healthType);
                I80F48 price = mangoCache.PriceCaches[i].Price;
                if (spot[i] > I80F48.Zero)
                {
                    assets = (spot[i] * price * w.SpotAssetWeight) + assets;
                }
                else
                {
                    liabilities = (-spot[i] * price * w.SpotLiabilityWeight) + liabilities;
                }

                if (perps[i] > I80F48.Zero)
                {
                    assets = (perps[i] * price * w.PerpAssetWeight) + assets;
                }
                else
                {
                    liabilities = (-perps[i] * price * w.PerpLiabilityWeight) + liabilities;
                }
            }

            return (assets, liabilities);
        }

        /// <summary>
        /// Gets the account's health ratio.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The health ratio.</returns>
        public I80F48 GetHealthRatio(MangoGroup mangoGroup, MangoCache mangoCache, HealthType healthType)
        {
            (List<I80F48> spot, List<I80F48> perps, I80F48 quote) =
                GetHealthComponents(mangoGroup, mangoCache);
            (I80F48 assets, I80F48 liabilities) =
                GetWeightedAssetsLiabilityValues(mangoGroup, mangoCache, spot, perps, quote, healthType);

            if (liabilities > I80F48.Zero)
            {
                return ((assets / liabilities) - I80F48.One) * I80F48.OneHundred;
            }

            return I80F48.OneHundred;
        }
        
        /// <summary>
        /// Gets the account's available balance.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The available balance.</returns>
        public I80F48 GetUiAvailableBalance(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex) =>
            MangoUtils.HumanizeNative(GetAvailableBalance(mangoGroup, mangoCache, tokenIndex),
                mangoGroup.GetQuoteTokenInfo().Decimals);

        /// <summary>
        /// Gets the account's available balance.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The available balance.</returns>
        public I80F48 GetAvailableBalance(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex)
        {
            I80F48 health = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            I80F48 net =
                GetNet(mangoCache.RootBankCaches[tokenIndex],
                    tokenIndex);

            if (tokenIndex == Constants.QuoteIndex)
            {
                return health.Min(net).Max(I80F48.Zero);
            }

            Weights w = MangoUtils.GetWeights(mangoGroup, tokenIndex, HealthType.Initialization);
            return net.Min((health / w.SpotAssetWeight) / mangoCache.PriceCaches[tokenIndex].Price).Max(I80F48.Zero);
        }

        /// <summary>
        /// Gets the health from the given components.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="spot">The spot components.</param>
        /// <param name="perps">The perp components.</param>
        /// <param name="quote">The quote component.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The health.</returns>
        public I80F48 GetHealthFromComponents(MangoGroup mangoGroup, MangoCache mangoCache,
            List<I80F48> spot, List<I80F48> perps, I80F48 quote, HealthType healthType)
        {
            I80F48 health = quote;

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                Weights weights = MangoUtils.GetWeights(mangoGroup, i, healthType);
                I80F48 price = mangoCache.PriceCaches[i].Price;
                I80F48 _spotHealth = spot[i] * price * (spot[i].IsPositive()
                    ? weights.SpotAssetWeight
                    : weights.SpotLiabilityWeight);
                I80F48 _perpHealth = perps[i] * price * (perps[i].IsPositive()
                    ? weights.PerpAssetWeight
                    : weights.PerpLiabilityWeight);

                health += _spotHealth + _perpHealth;
            }

            return health;
        }

        /// <summary>
        /// Gets the healths from the components.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="spot">The spot components.</param>
        /// <param name="perps">The perp components.</param>
        /// <param name="quote">The quote component.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The health.</returns>
        public (I80F48 SpotHealth, I80F48 PerpHealth) GetHealthsFromComponents(MangoGroup mangoGroup,
            MangoCache mangoCache, List<I80F48> spot, List<I80F48> perps, I80F48 quote, HealthType healthType)
        {
            I80F48 spotHealth = quote;
            I80F48 perpHealth = quote;

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                Weights weights = MangoUtils.GetWeights(mangoGroup, i, healthType);
                I80F48 price = mangoCache.PriceCaches[i].Price;
                I80F48 _spotHealth = spot[i] * price * (spot[i] > I80F48.Zero
                    ? weights.SpotAssetWeight
                    : weights.SpotLiabilityWeight);
                I80F48 _perpHealth = perps[i] * price * (perps[i] > I80F48.Zero
                    ? weights.PerpAssetWeight
                    : weights.PerpLiabilityWeight);

                spotHealth += _spotHealth;
                perpHealth += _perpHealth;
            }

            return (spotHealth, perpHealth);
        }

        /// <summary>
        /// Gets the health components.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <returns>The health components.</returns>
        public (List<I80F48> Spot, List<I80F48> Perps, I80F48 Quote) GetHealthComponents(
            MangoGroup mangoGroup, MangoCache mangoCache)
        {
            I80F48[] spot = new I80F48[(int)mangoGroup.NumOracles];
            I80F48[] perps = new I80F48[(int)mangoGroup.NumOracles];
            I80F48 quote = GetNet(mangoCache.RootBankCaches[Constants.QuoteIndex], Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                RootBankCache bankCache = mangoCache.RootBankCaches[i];
                I80F48 price = mangoCache.PriceCaches[i].Price;
                I80F48 baseNet = GetNet(bankCache, i);

                if (OpenOrdersAccounts.Count != 0)
                {
                    OpenOrdersAccount ooa = OpenOrdersAccounts[i];
                    if (InMarginBasket[i] && ooa != null)
                    {
                        OpenOrdersStats s = MangoUtils.SplitOpenOrders(ooa);
                        I80F48 bidsBaseNet = baseNet + (s.QuoteLocked / price) + s.BaseFree + s.BaseLocked;
                        I80F48 asksBaseNet = baseNet + s.BaseFree;

                        if (bidsBaseNet.Abs() > asksBaseNet.Abs())
                        {
                            spot[i] = bidsBaseNet;
                            quote += s.QuoteFree;
                        }
                        else
                        {
                            spot[i] = asksBaseNet;
                            var iadd = (s.BaseLocked * price) + s.QuoteFree + s.QuoteLocked;
                            quote += iadd;
                        }
                    }
                    else
                    {
                        spot[i] = baseNet;
                    }
                } else
                {
                    spot[i] = baseNet;
                }

                if (!mangoGroup.PerpetualMarkets[i].Market.Equals(SystemProgram.ProgramIdKey))
                {
                    PerpMarketCache marketCache = mangoCache.PerpetualMarketCaches[i];
                    PerpAccount perpAccount = PerpetualAccounts[i];
                    long baseLotSize = mangoGroup.PerpetualMarkets[i].BaseLotSize;
                    long quoteLotSize = mangoGroup.PerpetualMarkets[i].QuoteLotSize;

                    I80F48 takerQuote = new((decimal)perpAccount.TakerQuote * quoteLotSize);
                    I80F48 basePos = new((decimal) (perpAccount.BasePosition + perpAccount.TakerBase) * baseLotSize);
                    I80F48 bidsQuantity = new((decimal)perpAccount.BidsQuantity * baseLotSize);
                    I80F48 asksQuantity = new((decimal)perpAccount.AsksQuantity * baseLotSize);

                    I80F48 bidsBaseNet = basePos + bidsQuantity;
                    I80F48 asksBaseNet = basePos - asksQuantity;

                    if (bidsBaseNet.Abs() > asksBaseNet.Abs())
                    {
                        var quotePosition = perpAccount.GetQuotePosition(marketCache);
                        I80F48 quotePos = (quotePosition + takerQuote) -
                                          (bidsQuantity * price);
                        quote += quotePos;
                        perps[i] = bidsBaseNet;
                    }
                    else
                    {
                        var quotePosition = perpAccount.GetQuotePosition(marketCache);
                        I80F48 quotePos = (quotePosition + takerQuote) +
                                          (asksQuantity * price);
                        quote += quotePos;
                        perps[i] = asksBaseNet;
                    }
                }
                else
                {
                    perps[i] = I80F48.Zero;
                }
            }

            return (spot.ToList(), perps.ToList(), quote);
        }

        /// <summary>
        /// Gets the margin available to expand a position in this market.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="marketIndex">The market index.</param>
        /// <param name="marketType">The market type.</param>
        /// <returns>The margin available.</returns>
        public I80F48 GetUiMarketMarginAvailable(MangoGroup mangoGroup, MangoCache mangoCache, int marketIndex,
            MarketType marketType) =>
            MangoUtils.HumanizeNative(GetMarketMarginAvailable(mangoGroup, mangoCache, marketIndex, marketType),
                mangoGroup.GetQuoteTokenInfo().Decimals);

        /// <summary>
        /// Gets the margin available to expand a position in this market.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="marketIndex">The market index.</param>
        /// <param name="marketType">The market type.</param>
        /// <returns>The margin available.</returns>
        public I80F48 GetMarketMarginAvailable(MangoGroup mangoGroup, MangoCache mangoCache, int marketIndex,
            MarketType marketType)
        {
            I80F48 health = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);

            if (health < I80F48.Zero) return I80F48.Zero;

            Weights w = MangoUtils.GetWeights(mangoGroup, marketIndex, HealthType.Initialization);
            I80F48 weight = marketType == MarketType.Spot ? w.SpotAssetWeight : w.PerpAssetWeight;

            if (weight > I80F48.One)
            {
                //shouldn't happen
                return health;
            }

            return health / (I80F48.Zero - weight);
        }

        /// <summary>
        /// Gets the leverage of the account.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <returns>The leverage.</returns>
        public I80F48 GetLeverage(MangoGroup mangoGroup, MangoCache mangoCache)
        {
            I80F48 liabilities = GetLiabilitiesValue(mangoGroup, mangoCache);
            I80F48 assets = GetAssetsValue(mangoGroup, mangoCache);

            if (assets > I80F48.Zero)
            {
                return liabilities / (assets - liabilities);
            }

            return I80F48.Zero;
        }

        /// <summary>
        /// Gets the maximum leverage available for the given market.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="marketIndex">The market index.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="market">The market.</param>
        /// <param name="side">The side of the desired order.</param>
        /// <param name="price">The price.</param>
        /// <returns>The leverage stats.</returns>
        public LeverageStats GetMaxLeverageForMarket(MangoGroup mangoGroup, MangoCache mangoCache, int marketIndex,
            byte decimals, PerpMarket market, Side side, I80F48 price)
        {
            I80F48 initHealth = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            I80F48 healthDecimals = new I80F48((decimal) Math.Pow(10, mangoGroup.GetQuoteTokenInfo().Decimals));
            I80F48 uiInitHealth = initHealth / healthDecimals;

            I80F48 initLiabilityWeight = mangoGroup.PerpetualMarkets[marketIndex].InitializationLiabilityWeight;
            I80F48 initAssetWeight = mangoGroup.PerpetualMarkets[marketIndex].InitializationAssetWeight;

            I80F48 deposits = I80F48.Zero, borrows = I80F48.Zero, uiDeposit = I80F48.Zero, uiBorrow = I80F48.Zero;
            I80F48 basePosition = new I80F48((decimal)PerpetualAccounts[marketIndex].BasePosition);

            if (basePosition > I80F48.Zero)
            {
                deposits = new I80F48(market.BaseLotsToNumber(basePosition.ToDecimal(), decimals));
                uiDeposit = deposits * price;
            }
            else
            {
                borrows = new I80F48(market.BaseLotsToNumber(basePosition.ToDecimal(), decimals)).Abs();
                uiBorrow = borrows * price;
            }

            I80F48 max;
            if (side == Side.Buy)
            {
                I80F48 uiHealthAtZero = uiInitHealth + (uiBorrow * (initLiabilityWeight - I80F48.One));
                max = (uiHealthAtZero / (I80F48.One - initAssetWeight)) + uiBorrow;
            }
            else
            {
                I80F48 uiHealthAtZero = uiInitHealth + (uiDeposit * (initAssetWeight - I80F48.One));
                max = (uiHealthAtZero / (I80F48.One - initLiabilityWeight)) + uiDeposit;
            }

            return new LeverageStats
            {
                Maximum = max,
                UiDeposit = uiDeposit,
                UiBorrow = uiBorrow,
                Deposits = deposits,
                Borrows = borrows
            };
        }

        /// <summary>
        /// Gets the maximum leverage available for the given market.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="marketIndex">The market index.</param>
        /// <param name="market">The market.</param>
        /// <param name="side">The side of the desired order.</param>
        /// <param name="price">The price.</param>
        /// <returns>The leverage stats.</returns>
        public LeverageStats GetMaxLeverageForMarket(MangoGroup mangoGroup, MangoCache mangoCache, int marketIndex,
            Market market, Side side, I80F48 price)
        {
            I80F48 initHealth = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            I80F48 healthDecimals = new((decimal) Math.Pow(10, mangoGroup.GetQuoteTokenInfo().Decimals));
            I80F48 uiInitHealth = initHealth / healthDecimals;

            I80F48 initLiabilityWeight = mangoGroup.SpotMarkets[marketIndex].InitializationLiabilityWeight;
            I80F48 initAssetWeight = mangoGroup.SpotMarkets[marketIndex].InitializationAssetWeight;

            I80F48 deposits = GetUiDeposit(mangoCache.RootBankCaches[marketIndex], mangoGroup, marketIndex);
            I80F48 uiDeposit = deposits * price;
            I80F48 borrows = GetUiBorrow(mangoCache.RootBankCaches[marketIndex], mangoGroup, marketIndex);
            I80F48 uiBorrow = borrows * price;

            I80F48 max;
            if (side == Side.Buy)
            {
                I80F48 uiHealthAtZero = uiInitHealth + (uiBorrow * (initLiabilityWeight - I80F48.One));
                max = (uiHealthAtZero / (I80F48.One - initAssetWeight)) + uiBorrow;
            }
            else
            {
                I80F48 uiHealthAtZero = uiInitHealth + (uiDeposit * (initAssetWeight - I80F48.One));
                max = (uiHealthAtZero / (I80F48.One - initLiabilityWeight)) + uiDeposit;
            }

            return new LeverageStats
            {
                Maximum = max,
                UiDeposit = uiDeposit,
                UiBorrow = uiBorrow,
                Deposits = deposits,
                Borrows = borrows
            };
        }

        /// <summary>
        /// Gets the maximum amount available to be withdrawn while borrowing.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The maximum withdraw amount.</returns>
        public I80F48 GetMaxWithBorrowForToken(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex)
        {
            I80F48 oldInitHealth = GetHealth(mangoGroup, mangoCache, HealthType.Initialization).Floor();
            I80F48 tokenDeposits = GetNativeDeposit(mangoCache.RootBankCaches[tokenIndex], tokenIndex).Floor();
            I80F48 liabWeight, assetWeight, nativePrice;

            if (tokenIndex == Constants.QuoteIndex)
            {
                liabWeight = assetWeight = nativePrice = I80F48.One;
            }
            else
            {
                liabWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationLiabilityWeight;
                assetWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationAssetWeight;
                nativePrice = mangoCache.PriceCaches[tokenIndex].Price;
            }

            I80F48 newInitHealth = (oldInitHealth - (tokenDeposits * nativePrice * assetWeight)).Floor();
            I80F48 price = mangoGroup.GetPrice(mangoCache, tokenIndex);
            I80F48 healthDecimals = new ((decimal) Math.Pow(10, mangoGroup.GetQuoteTokenInfo().Decimals));

            return (newInitHealth / healthDecimals) / (price * liabWeight);
        }

        /// <summary>
        /// Gets the account equity.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">the mango cache.</param>
        /// <returns>The account equity.</returns>
        public I80F48 ComputeValue(MangoGroup mangoGroup, MangoCache mangoCache)
        {
            return GetAssetsValue(mangoGroup, mangoCache) - GetLiabilitiesValue(mangoGroup, mangoCache);
        }

        /// <summary>
        /// Gets the account equity in standard UI numbers. E.g. if equity is $100, this returns 100.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">the mango cache.</param>
        /// <returns>The account equity.</returns>
        public I80F48 GetUiEquity(MangoGroup mangoGroup, MangoCache mangoCache)
        {
            return MangoUtils.HumanizeNative(ComputeValue(mangoGroup, mangoCache), mangoGroup.GetQuoteTokenInfo().Decimals);
        }

        /// <summary>
        /// Whether the mango account is liquidatable.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">the mango cache.</param>
        /// <returns>true if it is, otherwise false.</returns>
        public bool IsLiquidatable(MangoGroup mangoGroup, MangoCache mangoCache)
        {
            return (BeingLiquidated && (GetHealth(mangoGroup, mangoCache, HealthType.Initialization) < I80F48.Zero)) ||
                   (GetHealth(mangoGroup, mangoCache, HealthType.Maintenance) < I80F48.Zero);
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="MangoAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="MangoAccount"/> structure.</returns>
        public static MangoAccount Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length) 
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");
            ReadOnlySpan<byte> span = data.AsSpan();

            List<bool> inMarginBasket = new(Constants.MaxPairs);
            ReadOnlySpan<byte> inMarginBasketBytes = span.Slice(Layout.InMarginBasketOffset, Constants.MaxPairs);
            foreach (byte b in inMarginBasketBytes)
            {
                inMarginBasket.Add(b != 0);
            }

            List<I80F48> deposits = new(Constants.MaxTokens);
            ReadOnlySpan<byte> depositsBytes = span.Slice(Layout.DepositsOffset, I80F48.Length * Constants.MaxTokens);
            for (int i = 0; i < Constants.MaxTokens; i++)
            {
                deposits.Add(I80F48.Deserialize(depositsBytes.Slice(i * I80F48.Length, I80F48.Length)));
            }

            List<I80F48> borrows = new(Constants.MaxTokens);
            ReadOnlySpan<byte> borrowBytes = span.Slice(Layout.BorrowsOffset, I80F48.Length * Constants.MaxTokens);
            for (int i = 0; i < Constants.MaxTokens; i++)
            {
                borrows.Add(I80F48.Deserialize(borrowBytes.Slice(i * I80F48.Length, I80F48.Length)));
            }

            List<PublicKey> spotOpenOrders = new(Constants.MaxPairs);
            ReadOnlySpan<byte> spotOpenOrdersBytes =
                span.Slice(Layout.SpotOpenOrdersOffset, PublicKey.PublicKeyLength * Constants.MaxPairs);
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                spotOpenOrders.Add(spotOpenOrdersBytes.GetPubKey(i * PublicKey.PublicKeyLength));
            }

            List<PerpAccount> perpAccounts = new(Constants.MaxPairs);
            ReadOnlySpan<byte> perpAccountsBytes = span.Slice(Layout.PerpetualAccountsOffset,
                PerpAccount.Layout.Length * Constants.MaxPairs);
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                perpAccounts.Add(PerpAccount.Deserialize(perpAccountsBytes.Slice(i * PerpAccount.Layout.Length,
                    PerpAccount.Layout.Length)));
            }

            List<byte> orderMarkets = new(Constants.MaxPerpOpenOrders);
            ReadOnlySpan<byte> orderMarketsBytes = span.Slice(Layout.OrderMarketOffset, Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                orderMarkets.Add(orderMarketsBytes.GetU8(i));
            }

            List<Side> orderSides = new(Constants.MaxPerpOpenOrders);
            ReadOnlySpan<byte> orderSidesBytes = span.Slice(Layout.OrderSideOffset, Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                orderSides.Add(orderSidesBytes.GetU8(i) == 0 ? Side.Buy : Side.Sell);
            }

            List<BigInteger> orderIds = new(Constants.MaxPerpOpenOrders);
            ReadOnlySpan<byte> orderIdsBytes =
                span.Slice(Layout.OrderIdsOffset, I80F48.Length * Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                orderIds.Add(orderIdsBytes.GetBigInt(i * I80F48.Length, I80F48.Length, true));
            }

            List<ulong> clientOrderIds = new(Constants.MaxPerpOpenOrders);
            ReadOnlySpan<byte> clientOrderIdsBytes =
                span.Slice(Layout.ClientOrderIdsOffset, sizeof(ulong) * Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                clientOrderIds.Add(clientOrderIdsBytes.GetU64(i * sizeof(ulong)));
            }

            return new MangoAccount
            {
                Metadata = MetaData.Deserialize(span.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                MangoGroup = span.GetPubKey(Layout.MangoGroupOffset),
                Owner = span.GetPubKey(Layout.OwnerOffset),
                InMarginBasket = inMarginBasket,
                NumInMarginBasket = span.GetU8(Layout.NumInMarginBasketOffset),
                Deposits = deposits,
                Borrows = borrows,
                SpotOpenOrders = spotOpenOrders,
                PerpetualAccounts = perpAccounts,
                OrderMarket = orderMarkets,
                OrderSide = orderSides,
                OrderIds = orderIds,
                ClientOrderIds = clientOrderIds,
                MegaSerumAmount = span.GetU64(Layout.MegaSerumAmountOffset),
                BeingLiquidated = span.GetU8(Layout.BeingLiquidatedOffset) == 1,
                Bankrupt = span.GetU8(Layout.BankruptOffset) == 1,
                AccountInfo = Encoding.UTF8.GetString(span.GetSpan(Layout.InfoOffset, Constants.InfoLength)).Trim('\0'),
                OpenOrdersAccounts = new List<OpenOrdersAccount>(Constants.MaxPairs),
                AdvancedOrdersAccount = span.GetPubKey(Layout.AdvancedOrdersOffset),
                NotUpgradeable = span.GetU8(Layout.NotUpgradeableOffset) == 1,
                Delegate = span.GetPubKey(Layout.DelegateOffset)
            };
        }
    }
}