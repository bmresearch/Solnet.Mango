using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models.Caches
{
    /// <summary>
    /// Represents a root bank cache in the <see cref="MangoCache"/>.
    /// </summary>
    public class RootBankCache
    {
        /// <summary>
        /// The layout of the <see cref="RootBankCache"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="RootBankCache"/> structure.
            /// </summary>
            internal const int Length = 40;

            /// <summary>
            /// The offset at which the deposit index value begins.
            /// </summary>
            internal const int DepositIndexOffset = 0;

            /// <summary>
            /// The offset at which the borrow index value begins.
            /// </summary>
            internal const int BorrowIndexOffset = 16;

            /// <summary>
            /// The offset at which the timestamp of the last update begins.
            /// </summary>
            internal const int LastUpdatedOffset = 32;
        }

        /// <summary>
        /// The deposit index.
        /// </summary>
        public I80F48 DepositIndex;

        /// <summary>
        /// The borrow index.
        /// </summary>
        public I80F48 BorrowIndex;

        /// <summary>
        /// Timestamp of the last update of the root bank.
        /// </summary>
        public ulong LastUpdated;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="RootBankCache"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBankCache"/> structure.</returns>
        public static RootBankCache Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            return new RootBankCache
            {
                DepositIndex = I80F48.Deserialize(data.GetSpan(Layout.DepositIndexOffset, I80F48.Length)),
                BorrowIndex = I80F48.Deserialize(data.GetSpan(Layout.BorrowIndexOffset, I80F48.Length)),
                LastUpdated = data.GetU64(Layout.LastUpdatedOffset)
            };
        }
    }
}