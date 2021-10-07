using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Mango.Models
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