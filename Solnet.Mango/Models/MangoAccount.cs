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
            for (int i = 0; i < Constants.MaxTokens - 1; i++)
            {
                deposits.Add(I80F48.Deserialize(depositsBytes.Slice(i * I80F48.Length, I80F48.Length)));
            }

            List<I80F48> borrows = new(Constants.MaxTokens);
            ReadOnlySpan<byte> borrowBytes = span.Slice(Layout.BorrowsOffset, I80F48.Length * Constants.MaxTokens);
            for (int i = 0; i < Constants.MaxTokens - 1; i++)
            {
                borrows.Add(I80F48.Deserialize(borrowBytes.Slice(i * I80F48.Length, I80F48.Length)));
            }

            List<PerpAccount> perpAccounts = new(Constants.MaxPairs);
            ReadOnlySpan<byte> perpAccountsBytes = span.Slice(Layout.PerpetualAccountsOffset,
                PerpAccount.Layout.Length * Constants.MaxTokens);
            for (int i = 0; i < Constants.MaxTokens - 1; i++)
            {
                perpAccounts.Add(PerpAccount.Deserialize(perpAccountsBytes.Slice(i * PerpAccount.Layout.Length,
                    PerpAccount.Layout.Length)));
            }

            List<byte> orderMarkets = new(Constants.MaxPairs);
            ReadOnlySpan<byte> orderMarketsBytes = span.Slice(Layout.OrderMarketOffset, Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders - 1; i++)
            {
                orderMarkets.Add(orderMarketsBytes.GetU8(i));
            }

            List<Side> orderSides = new(Constants.MaxPairs);
            ReadOnlySpan<byte> orderSidesBytes = span.Slice(Layout.OrderSideOffset, Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders - 1; i++)
            {
                orderSides.Add(orderSidesBytes.GetU8(i) == 0 ? Side.Buy : Side.Sell);
            }

            List<BigInteger> orderIds = new(Constants.MaxPairs);
            ReadOnlySpan<byte> orderIdsBytes = span.Slice(Layout.OrderIdsOffset, I80F48.Length * Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders - 1; i++)
            {
                orderIds.Add(orderIdsBytes.GetBigInt(i * I80F48.Length, I80F48.Length, true));
            }

            List<ulong> clientOrderIds = new(Constants.MaxPairs);
            ReadOnlySpan<byte> clientOrderIdsBytes =
                span.Slice(Layout.ClientOrderIdsOffset, sizeof(ulong) * Constants.MaxPerpOpenOrders);
            for (int i = 0; i < Constants.MaxPerpOpenOrders - 1; i++)
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