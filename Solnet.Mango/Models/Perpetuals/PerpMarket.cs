using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Mango.Models.Perpetuals
{
    /// <summary>
    /// Represents a perpetual market in Mango.
    /// </summary>
    public class PerpMarket
    {
        /// <summary>
        /// Represents the layout of the <see cref="PerpMarket"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="PerpMarket"/> structure.
            /// </summary>
            internal const int Length = 320;

            /// <summary>
            /// The offset at whiuch the metadata begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the mango group begins.
            /// </summary>
            internal const int MangoGroupOffset = 8;

            /// <summary>
            /// The offest at which the bids begin.
            /// </summary>
            internal const int BidsOffset = 40;

            /// <summary>
            /// The offset at which th assks begin.
            /// </summary>
            internal const int AsksOffset = 72;

            /// <summary>
            /// The offset at which the event queue begins.
            /// </summary>
            internal const int EventQueueOffset = 104;

            /// <summary>
            /// The offset at which the quote lot size value begins.
            /// </summary>
            internal const int QuoteLotSizeOffset = 136;

            /// <summary>
            /// The offset at which the base lot size value begins.
            /// </summary>
            internal const int BaseLotSizeOffset = 144;

            /// <summary>
            /// The offset at which the long funding begins.
            /// </summary>
            internal const int LongFundingOffset = 152;

            /// <summary>
            /// The offset at which the shoprt funding begins.
            /// </summary>
            internal const int ShortFundingOffset = 168;

            /// <summary>
            /// The offset at which the open interest begins.
            /// </summary>
            internal const int OpenInterestOffset = 184;

            /// <summary>
            /// The offset at which the last update begins.
            /// </summary>
            internal const int LastUpdatedOffset = 192;

            /// <summary>
            /// The offset at which the sequence number begins.
            /// </summary>
            internal const int SequenceNumberOffset = 200;

            /// <summary>
            /// The offset at which the fees accrued begins.
            /// </summary>
            internal const int FeesAccruedOffset = 208;

            /// <summary>
            /// The offset at which the liquidity mining info begins.
            /// </summary>
            internal const int LiquidityMiningInfoOffset = 224;

            /// <summary>
            /// The offset at which the mango vault begins.
            /// </summary>
            internal const int MangoVaultOffset = 288;
        }

        /// <summary>
        /// The account metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The mango group.
        /// </summary>
        public PublicKey MangoGroup;

        /// <summary>
        /// The bids.
        /// </summary>
        public PublicKey Bids;

        /// <summary>
        /// The asks.
        /// </summary>
        public PublicKey Asks;

        /// <summary>
        /// The event queue.
        /// </summary>
        public PublicKey EventQueue;

        /// <summary>
        /// The base lot size.
        /// </summary>
        public long BaseLotSize;

        /// <summary>
        /// The quote lot size.
        /// </summary>
        public long QuoteLotSize;

        /// <summary>
        /// The long funding.
        /// </summary>
        public I80F48 LongFunding;

        /// <summary>
        /// The short funding.
        /// </summary>
        public I80F48 ShortFunding;

        /// <summary>
        /// The open interest.
        /// </summary>
        public long OpenInterest;

        /// <summary>
        /// The last update timestamp.
        /// </summary>
        public ulong LastUpdated;

        /// <summary>
        /// The sequence number.
        /// </summary>
        public ulong SequenceNumber;

        /// <summary>
        /// The fees accrued.
        /// </summary>
        public I80F48 FeesAccrued;

        /// <summary>
        /// The liquidity mining info.
        /// </summary>
        public LiquidityMiningInfo LiquidityMiningInfo;

        /// <summary>
        /// The mango vault.
        /// </summary>
        public PublicKey MangoVault;

        /// <summary>
        /// Converts the price lots quantity to a the native value.
        /// </summary>
        /// <param name="price">The price</param>
        /// <returns>Convert price lots to the native value.</returns>
        public long PriceLotsToNative(double price) => (long)(QuoteLotSize * price) / BaseLotSize;

        /// <summary>
        /// Converts the base lots quantity to a the native value.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Convert price lots to the native value.</returns>
        public long BaseLotsToNative(double quantity) => (long)(BaseLotSize * quantity);

        /// <summary>
        /// Converts the price lots quantity to a humanized number.
        /// </summary>
        /// <param name="price">The price</param>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <param name="quoteDecimals">The quote decimals.</param>
        /// <returns>Convert price lots to humanized number.</returns>
        public decimal PriceLotsToNumber(I80F48 price, byte baseDecimals, byte quoteDecimals)
        {
            decimal nativeToUi = (decimal) Math.Pow(10, baseDecimals - quoteDecimals);
            decimal lotsToNative = (decimal) QuoteLotSize / BaseLotSize;
            return price.ToDecimal() * lotsToNative * nativeToUi;
        }

        /// <summary>
        /// Converts the base lots quantity to a humanized number.
        /// </summary>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Converted base lots to humanized number.</returns>
        public decimal BaseLotsToNumber(decimal quantity, byte baseDecimals)
            => (quantity * BaseLotSize) / (decimal) Math.Pow(10, baseDecimals);

        /// <summary>
        /// Get the minimum order size.
        /// </summary>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <returns>The minimum order size.</returns>
        public decimal MinOrderSize(byte baseDecimals) => BaseLotsToNumber(1, baseDecimals);

        /// <summary>
        /// The tick size.
        /// </summary>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <param name="quoteDecimals">The quote decimals.</param>
        /// <returns>The tick size.</returns>
        public decimal TickSize(byte baseDecimals, byte quoteDecimals) => PriceLotsToNumber(I80F48.One, baseDecimals, quoteDecimals);

        /// <summary>
        /// Conversion for order values.
        /// </summary>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="baseDecimals"></param>
        /// <param name="quoteDecimals"></param>
        /// <returns></returns>
        public (long Price, long Quantity) UiToNativePriceQuantity(double price, double quantity, byte baseDecimals, byte quoteDecimals)
        {
            var baseUnit = Math.Pow(10, baseDecimals);
            var quoteUnit = Math.Pow(10, quoteDecimals);

            var nativePrice = (long)(price * quoteUnit * BaseLotSize / (QuoteLotSize * baseUnit));
            var nativeQuantity = (long)(quantity * baseUnit / BaseLotSize);

            return (nativePrice, nativeQuantity);
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PerpMarket"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PerpMarket"/> structure.</returns>
        public static PerpMarket Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");
            ReadOnlySpan<byte> span = data.AsSpan();

            return new PerpMarket
            {
                Metadata = MetaData.Deserialize(span.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                MangoGroup = span.GetPubKey(Layout.MangoGroupOffset),
                Bids = span.GetPubKey(Layout.BidsOffset),
                Asks = span.GetPubKey(Layout.AsksOffset),
                EventQueue = span.GetPubKey(Layout.EventQueueOffset),
                QuoteLotSize = span.GetS64(Layout.QuoteLotSizeOffset),
                BaseLotSize = span.GetS64(Layout.BaseLotSizeOffset),
                LongFunding = I80F48.Deserialize(span.Slice(Layout.LongFundingOffset, I80F48.Length)),
                ShortFunding = I80F48.Deserialize(span.Slice(Layout.ShortFundingOffset, I80F48.Length)),
                OpenInterest = span.GetS64(Layout.OpenInterestOffset),
                LastUpdated = span.GetU64(Layout.LastUpdatedOffset),
                SequenceNumber = span.GetU64(Layout.SequenceNumberOffset),
                FeesAccrued = I80F48.Deserialize(span.Slice(Layout.FeesAccruedOffset, I80F48.Length)),
                LiquidityMiningInfo =
                    LiquidityMiningInfo.Deserialize(span.Slice(Layout.LiquidityMiningInfoOffset,
                        LiquidityMiningInfo.Layout.Length)),
                MangoVault = span.GetPubKey(Layout.MangoVaultOffset)
            };
        }
    }
}