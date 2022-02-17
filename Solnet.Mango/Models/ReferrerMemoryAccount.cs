using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// The referrer memory account.
    /// </summary>
    public class ReferrerMemoryAccount
    {
        /// <summary>
        /// The layout of the <see cref="ReferrerMemoryAccount"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ReferrerMemoryAccount"/> structure.
            /// </summary>
            public const int Length = 40;

            /// <summary>
            /// The offset at which the metadata structure begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the metadata structure begins.
            /// </summary>
            internal const int ReferrerOffset = 8;
        }

        /// <summary>
        /// The account's metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The referrer's mango account public key.
        /// </summary>
        public PublicKey Referrer;

        /// <summary>
        /// Deserialize a byte array into a <see cref="ReferrerMemoryAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="ReferrerMemoryAccount"/> structure.</returns>
        public static ReferrerMemoryAccount Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            ReadOnlySpan<byte> span = data.AsSpan();

            return new ReferrerMemoryAccount()
            {
                Metadata = MetaData.Deserialize(span.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                Referrer = span.GetPubKey(Layout.ReferrerOffset)
            };
        }
    }
}
