using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Mango.Models.Banks
{
    /// <summary>
    /// Represents a node bank in Mango.
    /// </summary>
    public class NodeBank
    {
        /// <summary>
        /// The layout of the <see cref="NodeBank"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="NodeBank"/> structure.
            /// </summary>
            internal const int Length = 72;

            /// <summary>
            /// The offset at which the metadata begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the deposits begin.
            /// </summary>
            internal const int DepositsOffset = 8;

            /// <summary>
            /// The offset at which the borrows begin.
            /// </summary>
            internal const int BorrowsOffset = 24;

            /// <summary>
            /// The offset at which the vault begins.
            /// </summary>
            internal const int VaultOffset = 40;
        }

        /// <summary>
        /// The account metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The deposits.
        /// </summary>
        public I80F48 Deposits;

        /// <summary>
        /// The borrows.
        /// </summary>
        public I80F48 Borrows;

        /// <summary>
        /// The vault of the node bank.
        /// </summary>
        public PublicKey Vault;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="NodeBank"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="NodeBank"/> structure.</returns>
        public static NodeBank Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");
            ReadOnlySpan<byte> span = data.AsSpan();

            return new NodeBank
            {
                Metadata = MetaData.Deserialize(span.GetSpan(Layout.MetadataOffset, MetaData.Layout.Length)),
                Deposits = I80F48.Deserialize(span.GetSpan(Layout.DepositsOffset, I80F48.Length)),
                Borrows = I80F48.Deserialize(span.GetSpan(Layout.BorrowsOffset, I80F48.Length)),
                Vault = span.GetPubKey(Layout.VaultOffset),
            };
        }
    }
}