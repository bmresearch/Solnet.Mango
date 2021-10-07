using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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
            /// 
            /// </summary>
            internal const int PerpetualAccountsOffset = 1080;

            /// <summary>
            /// 
            /// </summary>
            internal const int OrderMarketOffset = 2520;

            /// <summary>
            /// 
            /// </summary>
            internal const int OrderSideOffset = 2584;

            /// <summary>
            /// 
            /// </summary>
            internal const int OrderIdsOffset = 2648;

            /// <summary>
            /// 
            /// </summary>
            internal const int ClientOrderIdsOffset = 3672;

            /// <summary>
            /// 
            /// </summary>
            internal const int MegaSerumAmountOffset = 4184;

            /// <summary>
            /// 
            /// </summary>
            internal const int BeingLiquidatedOffset = 4192;

            /// <summary>
            /// 
            /// </summary>
            internal const int BankruptOffset = 4193;

            /// <summary>
            /// 
            /// </summary>
            internal const int InfoOffset = 4194;
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
        public string AccountInfo;

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
            return rootBank.DepositIndex.Value * this.Deposits[tokenIndex].Value;
        }

        /// <summary>
        /// Gets the amount of native deposits.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native deposit.</returns>
        public double GetNativeDeposit(RootBankCache rootBankCache, int tokenIndex)
        {
            return rootBankCache.DepositIndex.Value * this.Deposits[tokenIndex].Value;
        }

        /// <summary>
        /// Gets the amount of native borrows.
        /// </summary>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native borrow.</returns>
        public double GetNativeBorrow(RootBank rootBank, int tokenIndex)
        {
            return rootBank.BorrowIndex.Value * this.Borrows[tokenIndex].Value;
        }

        /// <summary>
        /// Gets the amount of native borrows.
        /// </summary>
        /// <param name="rootBankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The amount of native borrow.</returns>
        public double GetNativeBorrow(RootBankCache rootBankCache, int tokenIndex)
        {
            return rootBankCache.BorrowIndex.Value * this.Borrows[tokenIndex].Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootBank"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="tokenIndex"></param>
        /// <returns></returns>
        public double GetUiDeposit(RootBank rootBank, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeDeposit(rootBank, tokenIndex), mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootBank"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="tokenIndex"></param>
        /// <returns></returns>
        public double GetUiBorrow(RootBank rootBank, MangoGroup mangoGroup, int tokenIndex)
        {
            return MangoUtils.HumanizeNative(GetNativeBorrow(rootBank, tokenIndex), mangoGroup.Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Deposits minus borrows in native terms.
        /// </summary>
        /// <param name="bankCache">The root bank cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The net deposits.</returns>
        public double GetNet(RootBankCache bankCache, int tokenIndex)
        {
            return Deposits[tokenIndex].Value * bankCache.DepositIndex.Value - Borrows[tokenIndex].Value * bankCache.BorrowIndex.Value;
        }

        public double GetHealth(MangoGroup mangoGroup, MangoCache mangoCache, HealthType healthType)
        {
            var hc = GetHealthComponents(mangoGroup, mangoCache);

            return GetHealthFromComponents(mangoGroup, mangoCache, hc.Spot, hc.Perps, hc.Quote, healthType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="healthType"></param>
        /// <returns></returns>
        public double GetHealthRatio(MangoGroup mangoGroup, MangoCache mangoCache, HealthType healthType)
        {
            var healthComponents = GetHealthComponents(mangoGroup, mangoCache);
            //TODO: DO
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="tokenIndex"></param>
        /// <returns></returns>
        public double GetAvailableBalance(MangoGroup mangoGroup, MangoCache mangoCache, int tokenIndex)
        {
            var health = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            var net = GetNet(mangoCache.RootBankCaches[tokenIndex], tokenIndex);

            if (tokenIndex == Constants.QuoteIndex)
            {
                return Math.Max(health - net, 0);
            }

            var w = MangoUtils.GetWeights(mangoGroup, tokenIndex, HealthType.Initialization);
            return Math.Max(Math.Min(net, ((health / w.SpotAssetWeight) / mangoCache.PriceCaches[tokenIndex].Price.Value)), 0);

        }

        /// <summary>
        /// Gets
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="spot"></param>
        /// <param name="perps"></param>
        /// <param name="quote"></param>
        /// <param name="healthType"></param>
        /// <returns></returns>
        public double GetHealthFromComponents(MangoGroup mangoGroup, MangoCache mangoCache,
            List<double> spot, List<double> perps, double quote, HealthType healthType)
        {
            double health = quote;

            for (int i = 0; i < mangoGroup.Oracles.Count; i++)
            {
                var weights = MangoUtils.GetWeights(mangoGroup, i, healthType);
                var price = mangoCache.PriceCaches[i].Price;
                double _spotHealth = spot[i] * price.Value * spot[i] > 0 ? weights.SpotAssetWeight : weights.SpotLiabilityWeight;
                double _perpHealth = perps[i] * price.Value * perps[i] > 0 ? weights.PerpAssetWeight : weights.PerpLiabilityWeight;

                health += _spotHealth + _perpHealth;
            }

            return health;
        }

        /// <summary>
        /// Gets the healths from the components.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="spot"></param>
        /// <param name="perps"></param>
        /// <param name="quote"></param>
        /// <param name="healthType"></param>
        /// <returns></returns>
        public (double SpotHealth, double PerpHealth) GetHealthsFromComponents(MangoGroup mangoGroup, MangoCache mangoCache,
            List<double> spot, List<double> perps, double quote, HealthType healthType)
        {
            double spotHealth = quote;
            double perpHealth = quote;

            for (int i = 0; i < mangoGroup.Oracles.Count; i++)
            {
                var weights = MangoUtils.GetWeights(mangoGroup, i, healthType);
                var price = mangoCache.PriceCaches[i].Price;
                double _spotHealth = spot[i] * price.Value * spot[i] > 0 ? weights.SpotAssetWeight : weights.SpotLiabilityWeight;
                double _perpHealth = perps[i] * price.Value * perps[i] > 0 ? weights.PerpAssetWeight : weights.PerpLiabilityWeight;

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
            List<double> spot = new List<double>();
            List<double> perps = new List<double>();
            double quote = 0d;

            return (spot, perps, quote);
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
            var oldInitHealth = GetHealth(mangoGroup, mangoCache, HealthType.Initialization);
            var tokenDeposits = GetNativeDeposit(mangoCache.RootBankCaches[tokenIndex], tokenIndex);
            double liabWeight, assetWeight, nativePrice = 0;

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

            var newInitHealth = oldInitHealth - (tokenDeposits * nativePrice * assetWeight);
            var price = mangoGroup.GetPrice(mangoCache, tokenIndex);
            var healthDecimals = Math.Pow(10, mangoGroup.Tokens[Constants.QuoteIndex].Decimals);

            return ((newInitHealth / healthDecimals) / (price * liabWeight));

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
            foreach (var b in inMarginBasketBytes)
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
            ReadOnlySpan<byte> spotOpenOrdersBytes = span.Slice(Layout.SpotOpenOrdersOffset, PublicKey.PublicKeyLength * Constants.MaxPairs);
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                spotOpenOrders.Add(spotOpenOrdersBytes.GetPubKey(i * PublicKey.PublicKeyLength));
            }

            List<PerpAccount> perpAccounts = new(Constants.MaxPairs);
            ReadOnlySpan<byte> perpAccountsBytes = span.Slice(Layout.PerpetualAccountsOffset,
                PerpAccount.Layout.Length * Constants.MaxTokens);
            for (int i = 0; i < Constants.MaxTokens; i++)
            {
                perpAccounts.Add(PerpAccount.Deserialize(perpAccountsBytes.Slice(i * PerpAccount.Layout.Length,
                    PerpAccount.Layout.Length)));
            }

            List<byte> orderMarkets = new(Constants.MaxPairs);
            ReadOnlySpan<byte> orderMarketsBytes = span.Slice(Layout.OrderMarketOffset, Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                orderMarkets.Add(orderMarketsBytes.GetU8(i));
            }

            List<Side> orderSides = new(Constants.MaxPairs);
            ReadOnlySpan<byte> orderSidesBytes = span.Slice(Layout.OrderSideOffset, Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                orderSides.Add(orderSidesBytes.GetU8(i) == 0 ? Side.Buy : Side.Sell);
            }

            List<BigInteger> orderIds = new(Constants.MaxPairs);
            ReadOnlySpan<byte> orderIdsBytes = span.Slice(Layout.OrderIdsOffset, I80F48.Length * Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders; i++)
            {
                orderIds.Add(orderIdsBytes.GetBigInt(i * I80F48.Length, I80F48.Length, true));
            }

            List<ulong> clientOrderIds = new(Constants.MaxPairs);
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
                AccountInfo = Encoding.UTF8.GetString(span.GetSpan(Layout.InfoOffset, Constants.InfoLength))
            };
        }
    }
}