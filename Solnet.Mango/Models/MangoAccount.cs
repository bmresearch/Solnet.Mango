using Microsoft.Extensions.Logging;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Serum;
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
            /// The size of the raw structure.
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
            /// The offset at which the perpetual markets accounts begin.
            /// </summary>
            internal const int PerpetualAccountsOffset = 1080;

            /// <summary>
            /// The offset at which the order's markets values begin.
            /// </summary>
            internal const int OrderMarketOffset = 2520;

            /// <summary>
            /// The offset at which the order's side values begin.
            /// </summary>
            internal const int OrderSideOffset = 2584;

            /// <summary>
            /// The offset at which the order's ids begin.
            /// </summary>
            internal const int OrderIdsOffset = 2648;

            /// <summary>
            /// The offset at which the order's client ids begin.
            /// </summary>
            internal const int ClientOrderIdsOffset = 3672;

            /// <summary>
            /// The offset at which the account's MSRM amount value begins.
            /// </summary>
            internal const int MegaSerumAmountOffset = 4184;

            /// <summary>
            /// The offset at which the boolean which specifies if the account is being liquidated begins.
            /// </summary>
            internal const int BeingLiquidatedOffset = 4192;

            /// <summary>
            /// The offset at which the boolean which specifies if the account is bankrupt begins.
            /// </summary>
            internal const int BankruptOffset = 4193;

            /// <summary>
            /// The offset at which the account's info string begins.
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
        /// 
        /// </summary>
        public List<bool> InMarginBasket;

        /// <summary>
        /// 
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
            if (!openOrdersAccounts.WasRequestSuccessfullyHandled) return openOrdersAccounts;
            logger?.LogInformation(
                $"Successfully fetched {openOrdersAccounts.Result.Value.Count} open orders accounts.");

            SpotOpenOrders.ForEach(key =>
            {
                int keyIndex = filteredOpenOrders.IndexOf(key);
                if (keyIndex == -1)
                {
                    OpenOrdersAccounts.Add(null);
                    return;
                }

                OpenOrdersAccounts.Add(
                    OpenOrdersAccount.Deserialize(
                        Convert.FromBase64String(openOrdersAccounts.Result.Value[keyIndex].Data[0])));
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
        public double GetNativeDeposit(RootBank rootBank, int tokenIndex)
        {
            return rootBank.DepositIndex.Value *
                   Deposits[tokenIndex].Value;
        }

        /// <summary>
        /// Gets the amount of native deposits.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native deposit.</returns>
        public double GetNativeDeposit(RootBankCache rootBankCache, int tokenIndex)
        {
            return rootBankCache.DepositIndex.Value *
                   Deposits[tokenIndex].Value;
        }

        /// <summary>
        /// Gets the amount of native borrows.
        /// </summary>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native borrow.</returns>
        public double GetNativeBorrow(RootBank rootBank, int tokenIndex)
        {
            return rootBank.BorrowIndex.Value *
                   Borrows[tokenIndex].Value;
        }

        /// <summary>
        /// Gets the amount of native borrows.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native borrow.</returns>
        public double GetNativeBorrow(RootBankCache rootBankCache, int tokenIndex)
        {
            return rootBankCache.BorrowIndex.Value *
                   Borrows[tokenIndex].Value;
        }

        /// <summary>
        /// Gets the amount deposited humanized for ui display.
        /// </summary>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount deposited humanized for ui display.</returns>
        public double GetUiDeposit(RootBank rootBank, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeDeposit(rootBank, tokenIndex),
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the amount deposited humanized for ui display.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount deposited humanized for ui display.</returns>
        public double GetUiDeposit(RootBankCache rootBankCache, MangoGroup mangoGroup, int tokenIndex)
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
        public double GetUiBorrow(RootBank rootBank, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeBorrow(rootBank, tokenIndex),
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the amount borrowed humanized for ui display.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount borrowed humanized for ui display.</returns>
        public double GetUiBorrow(RootBankCache rootBankCache, MangoGroup mangoGroup, int tokenIndex)
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
        public double GetNet(RootBankCache bankCache, int tokenIndex)
        {
            return (Deposits[tokenIndex].Value * bankCache.DepositIndex.Value) -
                   (Borrows[tokenIndex].Value * bankCache.BorrowIndex.Value);
        }

        /// <summary>
        /// Deposits minus borrows in native terms.
        /// </summary>
        /// <param name="bankCache">The root bank cache.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The net deposits.</returns>
        public double GetUiNet(RootBankCache bankCache, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(Deposits[tokenIndex].Value * bankCache.DepositIndex.Value -
                                             Borrows[tokenIndex].Value * bankCache.BorrowIndex.Value,
                mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the account's health.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="healthType">The health type.</param>
        /// <returns>The health.</returns>
        public double GetHealth(MangoGroup mangoGroup, MangoCache mangoCache, HealthType healthType)
        {
            (List<double> spot, List<double> perps, double quote) = GetHealthComponents(mangoGroup, mangoCache);

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
        public double GetSpotValue(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex, double assetWeight)
        {
            double assetsValue = 0;
            double price = mangoGroup.GetPrice(mangoCache, tokenIndex);

            double deposits =
                GetUiDeposit(mangoCache.RootBankCaches[tokenIndex],
                    mangoGroup, tokenIndex) * price *
                assetWeight;
            assetsValue += deposits;

            OpenOrdersAccount openOrdersAccount = OpenOrdersAccounts[tokenIndex];
            if (openOrdersAccount == null)
                return assetsValue;

            assetsValue += MangoUtils.HumanizeNative(openOrdersAccount.BaseTokenTotal,
                mangoGroup.Tokens[tokenIndex].Decimals) * price * assetWeight;
            assetsValue += MangoUtils.HumanizeNative(
                openOrdersAccount.QuoteTokenTotal + openOrdersAccount.ReferrerRebatesAccrued,
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
        public double GetAssetsValue(MangoGroup mangoGroup, MangoCache mangoCache, HealthType? healthType = null)
        {
            double assetsValue = GetUiDeposit(mangoCache.RootBankCaches[Constants.QuoteIndex], mangoGroup,
                Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                double assetWeight = healthType switch
                {
                    HealthType.Maintenance => mangoGroup.SpotMarkets[i].MaintenanceAssetWeight.Value,
                    HealthType.Initialization => mangoGroup.SpotMarkets[i].InitializationAssetWeight.Value,
                    _ => 1
                };

                assetsValue += GetSpotValue(mangoGroup, mangoCache, i, assetWeight);
                assetsValue += MangoUtils.HumanizeNative(
                    PerpetualAccounts[i].GetAssetValue(
                        mangoGroup.PerpetualMarkets[i],
                        mangoCache.PriceCaches[i].Price.Value,
                        mangoCache.PerpetualMarketCaches[i].ShortFunding.Value,
                        mangoCache.PerpetualMarketCaches[i].LongFunding.Value),
                    mangoGroup.GetQuoteTokenInfo().Decimals);
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
        public double GetLiabilitiesValue(MangoGroup mangoGroup, MangoCache mangoCache, HealthType? healthType = null)
        {
            double liabilitiesValue = GetUiBorrow(mangoCache.RootBankCaches[Constants.QuoteIndex], mangoGroup,
                Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                double liabilityWeight = healthType switch
                {
                    HealthType.Maintenance => mangoGroup.SpotMarkets[i].MaintenanceLiabilityWeight.Value,
                    HealthType.Initialization => mangoGroup.SpotMarkets[i].InitializationLiabilityWeight.Value,
                    _ => 1
                };
                liabilitiesValue += GetUiBorrow(mangoCache.RootBankCaches[i], mangoGroup, i) *
                                    mangoGroup.GetPrice(mangoCache, i) * liabilityWeight;
                liabilitiesValue += MangoUtils.HumanizeNative(
                    PerpetualAccounts[i].GetAssetValue(
                        mangoGroup.PerpetualMarkets[i],
                        mangoCache.PriceCaches[i].Price.Value,
                        mangoCache.PerpetualMarketCaches[i].ShortFunding.Value,
                        mangoCache.PerpetualMarketCaches[i].LongFunding.Value),
                    mangoGroup.GetQuoteTokenInfo().Decimals);
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
        public double GetNativeLiabilitiesValue(MangoGroup mangoGroup, MangoCache mangoCache, HealthType? healthType = null)
        {
            double liabilitiesValue =
                GetNativeBorrow(mangoCache.RootBankCaches[Constants.QuoteIndex], Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                double liabilityWeight = healthType switch
                {
                    HealthType.Maintenance => mangoGroup.SpotMarkets[i].MaintenanceLiabilityWeight.Value,
                    HealthType.Initialization => mangoGroup.SpotMarkets[i].InitializationLiabilityWeight.Value,
                    _ => 1
                };
                liabilitiesValue += GetNativeBorrow(mangoCache.RootBankCaches[i], i) *
                                    mangoGroup.GetPrice(mangoCache, i) * liabilityWeight;
                liabilitiesValue += PerpetualAccounts[i].GetAssetValue(
                    mangoGroup.PerpetualMarkets[i],
                    mangoCache.PriceCaches[i].Price.Value,
                    mangoCache.PerpetualMarketCaches[i].ShortFunding.Value,
                    mangoCache.PerpetualMarketCaches[i].LongFunding.Value);
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
        public (double SpotHealth, double PerpHealth) GetWeightedAssetsLiabilityValues(MangoGroup mangoGroup,
            MangoCache mangoCache, List<double> spot, List<double> perps, double quote, HealthType healthType)
        {
            double assets = 0;
            double liabilities = 0;

            if (quote > 0)
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
                double price = mangoCache.PriceCaches[i].Price.Value;
                if (spot[i] > 0)
                {
                    assets = (spot[i] * price * w.SpotAssetWeight) + assets;
                }
                else
                {
                    liabilities = (-spot[i] * price * w.SpotLiabilityWeight) + liabilities;
                }

                if (perps[i] > 0)
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
        public double GetHealthRatio(MangoGroup mangoGroup, MangoCache mangoCache, HealthType healthType)
        {
            (List<double> spot, List<double> perps, double quote) =
                GetHealthComponents(mangoGroup, mangoCache);
            (double assets, double liabilities) =
                GetWeightedAssetsLiabilityValues(mangoGroup, mangoCache, spot, perps, quote, healthType);

            if (liabilities > 0)
            {
                return ((assets / liabilities) - 1) * 100;
            }

            return 100;
        }
        
        /// <summary>
        /// Gets the account's available balance.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The available balance.</returns>
        public double GetUiAvailableBalance(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex) =>
            MangoUtils.HumanizeNative(GetAvailableBalance(mangoGroup, mangoCache, tokenIndex),
                mangoGroup.GetQuoteTokenInfo().Decimals);

        /// <summary>
        /// Gets the account's available balance.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The available balance.</returns>
        public double GetAvailableBalance(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex)
        {
            double health = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            double net =
                GetNet(mangoCache.RootBankCaches[tokenIndex],
                    tokenIndex);

            if (tokenIndex == Constants.QuoteIndex)
            {
                return Math.Max(Math.Min(health, net), 0);
            }

            Weights w = MangoUtils.GetWeights(mangoGroup, tokenIndex, HealthType.Initialization);
            return Math.Max(
                Math.Min(net, (health / w.SpotAssetWeight) / mangoCache.PriceCaches[tokenIndex].Price.Value), 0);
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
        public double GetHealthFromComponents(MangoGroup mangoGroup, MangoCache mangoCache,
            List<double> spot, List<double> perps, double quote, HealthType healthType)
        {
            double health = quote;

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                Weights weights = MangoUtils.GetWeights(mangoGroup, i, healthType);
                I80F48 price = mangoCache.PriceCaches[i].Price;
                double _spotHealth = spot[i] * price.Value * (spot[i] > 0
                    ? weights.SpotAssetWeight
                    : weights.SpotLiabilityWeight);
                double _perpHealth = perps[i] * price.Value * (perps[i] > 0
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
        public (double SpotHealth, double PerpHealth) GetHealthsFromComponents(MangoGroup mangoGroup,
            MangoCache mangoCache, List<double> spot, List<double> perps, double quote, HealthType healthType)
        {
            double spotHealth = quote;
            double perpHealth = quote;

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                Weights weights = MangoUtils.GetWeights(mangoGroup, i, healthType);
                I80F48 price = mangoCache.PriceCaches[i].Price;
                double _spotHealth = spot[i] * price.Value * (spot[i] > 0
                    ? weights.SpotAssetWeight
                    : weights.SpotLiabilityWeight);
                double _perpHealth = perps[i] * price.Value * (perps[i] > 0
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
        public (List<double> Spot, List<double> Perps, double Quote) GetHealthComponents(
            MangoGroup mangoGroup, MangoCache mangoCache)
        {
            double[] spot = new double[(int)mangoGroup.NumOracles];
            double[] perps = new double[(int)mangoGroup.NumOracles];
            double quote = GetNet(mangoCache.RootBankCaches[Constants.QuoteIndex], Constants.QuoteIndex);

            for (int i = 0; i < (int)mangoGroup.NumOracles; i++)
            {
                RootBankCache bankCache = mangoCache.RootBankCaches[i];
                double price = mangoCache.PriceCaches[i].Price.Value;
                double baseNet = GetNet(bankCache, i);

                OpenOrdersAccount ooa = OpenOrdersAccounts[i];

                if (InMarginBasket[i] && ooa != null)
                {
                    OpenOrdersStats s = MangoUtils.SplitOpenOrders(ooa);
                    double bidsBaseNet = baseNet + (s.QuoteLocked / price) + s.BaseFree + s.BaseLocked;
                    double asksBaseNet = baseNet + s.BaseFree;

                    if (Math.Abs(bidsBaseNet) > Math.Abs(asksBaseNet))
                    {
                        spot[i] = bidsBaseNet;
                        quote += s.QuoteFree;
                    }
                    else
                    {
                        spot[i] = asksBaseNet;
                        quote = (s.BaseLocked * price) + s.QuoteFree + s.QuoteLocked + quote;
                    }
                }
                else
                {
                    spot[i] = baseNet;
                }

                if (!mangoGroup.PerpetualMarkets[i].Market.Equals(SystemProgram.ProgramIdKey))
                {
                    PerpMarketCache marketCache = mangoCache.PerpetualMarketCaches[i];
                    PerpAccount perpAccount = PerpetualAccounts[i];
                    long baseLotSize = mangoGroup.PerpetualMarkets[i].BaseLotSize;
                    long quoteLotSize = mangoGroup.PerpetualMarkets[i].QuoteLotSize;

                    double takerQuote = perpAccount.TakerQuote * quoteLotSize;
                    double basePos = (perpAccount.BasePosition + perpAccount.TakerBase) * baseLotSize;
                    double bidsQuantity = perpAccount.BidsQuantity * baseLotSize;
                    double asksQuantity = perpAccount.AsksQuantity * baseLotSize;

                    double bidsBaseNet = basePos + bidsQuantity;
                    double asksBaseNet = basePos - asksQuantity;

                    if (Math.Abs(bidsBaseNet) > Math.Abs(asksBaseNet))
                    {
                        double quotePos = (perpAccount.GetQuotePosition(marketCache) + takerQuote) -
                                          (bidsQuantity * price);
                        quote += quotePos;
                        perps[i] = bidsBaseNet;
                    }
                    else
                    {
                        double quotePos = (perpAccount.GetQuotePosition(marketCache) + takerQuote) +
                                          (asksQuantity * price);
                        quote += quotePos;
                        perps[i] = asksBaseNet;
                    }
                }
                else
                {
                    perps[i] = 0;
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
        public double GetUiMarketMarginAvailable(MangoGroup mangoGroup, MangoCache mangoCache, int marketIndex,
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
        public double GetMarketMarginAvailable(MangoGroup mangoGroup, MangoCache mangoCache, int marketIndex,
            MarketType marketType)
        {
            double health = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);

            if (health < 0) return 0;

            Weights w = MangoUtils.GetWeights(mangoGroup, marketIndex, HealthType.Initialization);
            double weight = marketType == MarketType.Spot ? w.SpotAssetWeight : w.PerpAssetWeight;

            if (weight > 1)
            {
                //shouldn't happen
                return health;
            }

            return health / (1 - weight);
        }

        /// <summary>
        /// Gets the leverage of the account.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <returns>The leverage.</returns>
        public double GetLeverage(MangoGroup mangoGroup, MangoCache mangoCache)
        {
            double liabilities = GetLiabilitiesValue(mangoGroup, mangoCache);
            double assets = GetAssetsValue(mangoGroup, mangoCache);

            if (assets > 0)
            {
                return liabilities / (assets - liabilities);
            }

            return 0;
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
            byte decimals, PerpMarket market, Side side, double price)
        {
            double initHealth = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            double healthDecimals = Math.Pow(10, mangoGroup.GetQuoteTokenInfo().Decimals);
            double uiInitHealth = initHealth / healthDecimals;

            double initLiabilityWeight = mangoGroup.PerpetualMarkets[marketIndex].InitializationLiabilityWeight.Value;
            double initAssetWeight = mangoGroup.PerpetualMarkets[marketIndex].InitializationAssetWeight.Value;

            double deposits = 0, borrows = 0, uiDeposit = 0, uiBorrow = 0;
            double basePosition = PerpetualAccounts[marketIndex].BasePosition;

            if (basePosition > 0)
            {
                deposits = market.BaseLotsToNumber(basePosition, decimals);
                uiDeposit = deposits * price;
            }
            else
            {
                borrows = Math.Abs(market.BaseLotsToNumber(basePosition, decimals));
                uiBorrow = borrows * price;
            }

            double max;
            if (side == Side.Buy)
            {
                double uiHealthAtZero = uiInitHealth + (uiBorrow * (initLiabilityWeight - 1));
                max = (uiHealthAtZero / (1 - initAssetWeight)) + uiBorrow;
            }
            else
            {
                double uiHealthAtZero = uiInitHealth + (uiDeposit * (initAssetWeight - 1));
                max = (uiHealthAtZero / (1 - initLiabilityWeight)) + uiDeposit;
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
            Market market, Side side, double price)
        {
            double initHealth = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            double healthDecimals = Math.Pow(10, mangoGroup.GetQuoteTokenInfo().Decimals);
            double uiInitHealth = initHealth / healthDecimals;

            double initLiabilityWeight = mangoGroup.SpotMarkets[marketIndex].InitializationLiabilityWeight.Value;
            double initAssetWeight = mangoGroup.SpotMarkets[marketIndex].InitializationAssetWeight.Value;

            double deposits = GetUiDeposit(mangoCache.RootBankCaches[marketIndex], mangoGroup, marketIndex);
            double uiDeposit = deposits * price;
            double borrows = GetUiBorrow(mangoCache.RootBankCaches[marketIndex], mangoGroup, marketIndex);
            double uiBorrow = borrows * price;

            double max;
            if (side == Side.Buy)
            {
                double uiHealthAtZero = uiInitHealth + (uiBorrow * (initLiabilityWeight - 1));
                max = (uiHealthAtZero / (1 - initAssetWeight)) + uiBorrow;
            }
            else
            {
                double uiHealthAtZero = uiInitHealth + (uiDeposit * (initAssetWeight - 1));
                max = (uiHealthAtZero / (1 - initLiabilityWeight)) + uiDeposit;
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
        public double GetMaxWithBorrowForToken(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex)
        {
            double oldInitHealth = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            double tokenDeposits =
                GetNativeDeposit(
                    mangoCache.RootBankCaches[tokenIndex],
                    tokenIndex);
            double liabWeight, assetWeight, nativePrice;

            if (tokenIndex == Constants.QuoteIndex)
            {
                liabWeight = assetWeight = nativePrice = 1;
            }
            else
            {
                liabWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationLiabilityWeight.Value;
                assetWeight = mangoGroup.SpotMarkets[tokenIndex].InitializationAssetWeight.Value;
                nativePrice = mangoCache.PriceCaches[tokenIndex].Price.Value;
            }

            double newInitHealth = oldInitHealth - (tokenDeposits * nativePrice * assetWeight);
            double price = mangoGroup.GetPrice(mangoCache, tokenIndex);
            double healthDecimals = Math.Pow(10, mangoGroup.GetQuoteTokenInfo().Decimals);

            return (newInitHealth / healthDecimals) / (price * liabWeight);
        }

        /// <summary>
        /// Gets the account equity.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">the mango cache.</param>
        /// <returns>The account equity.</returns>
        public double GetEquity(MangoGroup mangoGroup, MangoCache mangoCache)
        {
            return GetAssetsValue(mangoGroup, mangoCache) - GetLiabilitiesValue(mangoGroup, mangoCache);
        }

        /// <summary>
        /// Whether the mango account is liquidatable.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">the mango cache.</param>
        /// <returns>true if it is, otherwise false.</returns>
        public bool IsLiquidatable(MangoGroup mangoGroup, MangoCache mangoCache)
        {
            return (BeingLiquidated && (GetHealth(mangoGroup, mangoCache, HealthType.Initialization) < 0)) ||
                   (GetHealth(mangoGroup, mangoCache, HealthType.Maintenance) < 0);
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="MangoAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="MangoAccount"/> structure.</returns>
        public static MangoAccount Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
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