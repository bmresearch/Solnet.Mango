using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RootBankCache
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
            internal const int DepositIndexOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int BorrowIndexOffset = 16;

            /// <summary>
            /// 
            /// </summary>
            internal const int LastUpdatedOffset = 32;
        }

        /// <summary>
        /// 
        /// </summary>
        public I80F48 DepositIndex;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 BorrowIndex;

        /// <summary>
        /// 
        /// </summary>
        public ulong LastUpdated;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="RootBankCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBankCache"/> structure.</returns>
        public static RootBankCache Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");

            return new RootBankCache
            {
                DepositIndex = I80F48.Deserialize(data.GetSpan(Layout.DepositIndexOffset, I80F48.Length)),
                BorrowIndex = I80F48.Deserialize(data.GetSpan(Layout.BorrowIndexOffset, I80F48.Length)),
                LastUpdated = data.GetU64(Layout.LastUpdatedOffset)
            };
        }
    }
}