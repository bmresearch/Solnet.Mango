using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models.Caches
{
    /// <summary>
    /// Represents a price cache in the <see cref="MangoCache"/>.
    /// </summary>
    public class PriceCache
    {
        /// <summary>
        /// The layout of the <see cref="PriceCache"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the structure.
            /// </summary>
            internal const int Length = 24;

            /// <summary>
            /// The offset at which the price begins.
            /// </summary>
            internal const int PriceOffset = 0;

            /// <summary>
            /// The offset at which the last updated timestmap begins.
            /// </summary>
            internal const int LastUpdatedOffset = 16;
        }

        /// <summary>
        /// The price.
        /// </summary>
        public I80F48 Price;

        /// <summary>
        /// The last updated timestamp.
        /// </summary>
        public ulong LastUpdated;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PriceCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PriceCache"/> structure.</returns>
        public static PriceCache Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            return new PriceCache
            {
                Price = I80F48.Deserialize(data.GetSpan(Layout.PriceOffset, I80F48.Length)),
                LastUpdated = data.GetU64(Layout.LastUpdatedOffset)
            };
        }
    }
}