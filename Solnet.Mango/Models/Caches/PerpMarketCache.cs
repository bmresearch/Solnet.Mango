using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models.Caches
{
    /// <summary>
    /// Represents a perp market's cache.
    /// </summary>
    public class PerpMarketCache
    {
        /// <summary>
        /// The layout of the <see cref="PerpMarketCache"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="PerpMarketCache"/> structure.
            /// </summary>
            internal const int Length = 40;

            /// <summary>
            /// The offset at which the long funding begins.
            /// </summary>
            internal const int LongFundingOffset = 0;

            /// <summary>
            /// The offset at which the short funding begins.
            /// </summary>
            internal const int ShortFundingOffset = 16;

            /// <summary>
            /// The offset at which the last update timestamp begins.
            /// </summary>
            internal const int LastUpdatedOffset = 32;
        }

        /// <summary>
        /// The long funding.
        /// </summary>
        public I80F48 LongFunding;

        /// <summary>
        /// The short funding.
        /// </summary>
        public I80F48 ShortFunding;

        /// <summary>
        /// The last update.
        /// </summary>
        public ulong LastUpdated;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="RootBankCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBankCache"/> structure.</returns>
        public static PerpMarketCache Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            return new PerpMarketCache
            {
                LongFunding = I80F48.Deserialize(data.GetSpan(Layout.LongFundingOffset, I80F48.Length)),
                ShortFunding = I80F48.Deserialize(data.GetSpan(Layout.ShortFundingOffset, I80F48.Length)),
                LastUpdated = data.GetU64(Layout.LastUpdatedOffset)
            };
        }
    }
}