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
            /// 
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int MangoGroupOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int BidsOffset = 40;

            /// <summary>
            /// 
            /// </summary>
            internal const int AsksOffset = 72;

            /// <summary>
            /// 
            /// </summary>
            internal const int EventQueueOffset = 104;

            /// <summary>
            /// 
            /// </summary>
            internal const int QuoteLotSizeOffset = 136;

            /// <summary>
            /// 
            /// </summary>
            internal const int BaseLotSizeOffset = 144;

            /// <summary>
            /// 
            /// </summary>
            internal const int LongFundingOffset = 152;

            /// <summary>
            /// 
            /// </summary>
            internal const int ShortFundingOffset = 168;

            /// <summary>
            /// 
            /// </summary>
            internal const int OpenInterestOffset = 184;

            /// <summary>
            /// 
            /// </summary>
            internal const int LastUpdatedOffset = 192;

            /// <summary>
            /// 
            /// </summary>
            internal const int SequenceNumberOffset = 200;

            /// <summary>
            /// 
            /// </summary>
            internal const int FeesAccruedOffset = 208;

            /// <summary>
            /// 
            /// </summary>
            internal const int LiquidityMiningInfoOffset = 224;

            /// <summary>
            /// 
            /// </summary>
            internal const int MangoVaultOffset = 288;
        }

        /// <summary>
        /// The account metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey MangoGroup;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey Bids;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey Asks;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey EventQueue;

        /// <summary>
        /// 
        /// </summary>
        public long BaseLotSize;

        /// <summary>
        /// 
        /// </summary>
        public long QuoteLotSize;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 LongFunding;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 ShortFunding;

        /// <summary>
        /// 
        /// </summary>
        public long OpenInterest;

        /// <summary>
        /// 
        /// </summary>
        public ulong LastUpdated;

        /// <summary>
        /// 
        /// </summary>
        public ulong SequenceNumber;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 FeesAccrued;

        /// <summary>
        /// 
        /// </summary>
        public LiquidityMiningInfo LiquidityMiningInfo;

        /// <summary>
        /// 
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
        public double PriceLotsToNumber(double price, byte baseDecimals, byte quoteDecimals)
        {
            double nativeToUi = Math.Pow(10, baseDecimals - quoteDecimals);
            double lotsToNative = QuoteLotSize / BaseLotSize;
            return price * lotsToNative * nativeToUi;
        }

        /// <summary>
        /// Converts the base lots quantity to a humanized number.
        /// </summary>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Converted base lots to humanized number.</returns>
        public double BaseLotsToNumber(double quantity, byte baseDecimals)
            => (quantity * BaseLotSize) / Math.Pow(10, baseDecimals);

        /// <summary>
        /// Get the minimum order size.
        /// </summary>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <returns>The minimum order size.</returns>
        public double MinOrderSize(byte baseDecimals) => BaseLotsToNumber(1, baseDecimals);

        /// <summary>
        /// The tick size.
        /// </summary>
        /// <param name="baseDecimals">The base decimals.</param>
        /// <param name="quoteDecimals">The quote decimals.</param>
        /// <returns>The tick size.</returns>
        public double TickSize(byte baseDecimals, byte quoteDecimals) => PriceLotsToNumber(1, baseDecimals, quoteDecimals);

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

            var nativePrice = (long)((price * quoteUnit * BaseLotSize) / (QuoteLotSize / baseUnit));
            var nativeQuantity = (long)((quantity * baseUnit) / BaseLotSize);

            return (nativePrice, nativeQuantity);
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PerpMarket"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PerpMarket"/> structure.</returns>
        public static PerpMarket Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
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