using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PriceCache
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            internal const int Length = 24;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int PriceOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int LastUpdatedOffset = 16;
        }

        /// <summary>
        /// 
        /// </summary>
        public I80F48 Price;

        /// <summary>
        /// 
        /// </summary>
        public ulong LastUpdated;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PriceCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PriceCache"/> structure.</returns>
        public static PriceCache Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
            
            return new PriceCache
            {
                Price = I80F48.Deserialize(data.GetSpan(Layout.PriceOffset, I80F48.Length)),
                LastUpdated = data.GetU64(Layout.LastUpdatedOffset)
            };
        }
    }
}