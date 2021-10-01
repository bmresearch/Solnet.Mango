using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PerpAccount
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            internal const int Length = 96;

            /// <summary>
            /// 
            /// </summary>
            internal const int BasePositionOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int QuotedPositionOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int LongSettledFundingOffset = 24;

            /// <summary>
            /// 
            /// </summary>
            internal const int ShortSettledFundingOffset = 40;

            /// <summary>
            /// 
            /// </summary>
            internal const int BidsQuantityOffset = 56;

            /// <summary>
            /// /
            /// </summary>
            internal const int AsksQuantityOffset = 64;

            /// <summary>
            /// 
            /// </summary>
            internal const int TakerBaseOffset = 72;

            /// <summary>
            /// 
            /// </summary>
            internal const int TakerQuoteOffset = 80;

            /// <summary>
            /// 
            /// </summary>
            internal const int MngoAccruedOffset = 88;
        }

        /// <summary>
        /// 
        /// </summary>
        public long BasePosition;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 QuotePosition;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 LongSettledFunding;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 ShortSettledFunding;

        /// <summary>
        /// 
        /// </summary>
        public long BidsQuantity;

        /// <summary>
        /// 
        /// </summary>
        public long AsksQuantity;

        /// <summary>
        /// 
        /// </summary>
        public long TakerBase;

        /// <summary>
        /// 
        /// </summary>
        public long TakerQuote;

        /// <summary>
        /// 
        /// </summary>
        public ulong MngoAccrued;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PerpAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PerpAccount"/> structure.</returns>
        public static PerpAccount Deserialize(ReadOnlySpan<byte> data)
        {
            
            return new PerpAccount
            {
                BasePosition = data.GetS64(Layout.BasePositionOffset),
                QuotePosition = I80F48.Deserialize(data.GetSpan(Layout.QuotedPositionOffset, I80F48.Length)),
                LongSettledFunding = I80F48.Deserialize(data.GetSpan(Layout.LongSettledFundingOffset, I80F48.Length)),
                ShortSettledFunding = I80F48.Deserialize(data.GetSpan(Layout.ShortSettledFundingOffset, I80F48.Length)),
                BidsQuantity = data.GetS64(Layout.BidsQuantityOffset),
                AsksQuantity = data.GetS64(Layout.AsksQuantityOffset),
                TakerBase = data.GetS64(Layout.TakerBaseOffset),
                TakerQuote = data.GetS64(Layout.TakerQuoteOffset),
                MngoAccrued = data.GetU64(Layout.MngoAccruedOffset)
            };
        }
    }
}