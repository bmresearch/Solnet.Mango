using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PerpMarketCache
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            internal const int Length = 40;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int LongFundingOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int ShortFundingOffset = 16;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int LastUpdatedOffset = 32;
        }

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
        public ulong LastUpdated;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="RootBankCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBankCache"/> structure.</returns>
        public static PerpMarketCache Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
            
            return new PerpMarketCache
            {
                LongFunding = I80F48.Deserialize(data.GetSpan(Layout.LongFundingOffset, I80F48.Length)),
                ShortFunding = I80F48.Deserialize(data.GetSpan(Layout.ShortFundingOffset, I80F48.Length)),
                LastUpdated = data.GetU64(Layout.LastUpdatedOffset)
            };
        }
    }
}