using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Text;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// The referrer memory account.
    /// </summary>
    public class ReferrerIdRecordAccount
    {
        /// <summary>
        /// The layout of the <see cref="ReferrerIdRecordAccount"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ReferrerIdRecordAccount"/> structure.
            /// </summary>
            public const int Length = 72;

            /// <summary>
            /// The offset at which the metadata structure begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the metadata structure begins.
            /// </summary>
            internal const int ReferrerOffset = 8;

            /// <summary>
            /// The offset at which the metadata structure begins.
            /// </summary>
            internal const int IdOffset = 40;
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
        /// The referrer id.
        /// </summary>
        public string Id;

        /// <summary>
        /// Deserialize a byte array into a <see cref="ReferrerIdRecordAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="ReferrerIdRecordAccount"/> structure.</returns>
        public static ReferrerIdRecordAccount Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            ReadOnlySpan<byte> span = data.AsSpan();

            return new ReferrerIdRecordAccount()
            {
                Metadata = MetaData.Deserialize(span.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                Referrer = span.GetPubKey(Layout.ReferrerOffset),
                Id = Encoding.UTF8.GetString(span.GetSpan(Layout.IdOffset, Constants.InfoLength)).Trim('\0'),
            };
        }
    }
}
