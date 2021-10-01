using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a root bank for a token's lending and borrowing info.
    /// </summary>
    public class RootBank
    {
        /// <summary>
        /// The layout of the <see cref="RootBank"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="RootBank"/> structure.
            /// </summary>
            internal const int Length = 424;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int OptimalUtilizationOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int OptimalRateOffset = 24;

            /// <summary>
            /// 
            /// </summary>
            internal const int MaxRateOffset = 40;

            /// <summary>
            /// 
            /// </summary>
            internal const int NumNodeBanksOffset = 56;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int NodeBanksOffset = 64;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int DepositIndexOffset = 320;

            /// <summary>
            /// 
            /// </summary>
            internal const int BorrowIndexOffset = 336;

            /// <summary>
            /// 
            /// </summary>
            internal const int LastUpdatedOffset = 352;
        }

        /// <summary>
        /// 
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 OptimalUtilization;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 OptimalRate;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 MaxRate;

        /// <summary>
        /// 
        /// </summary>
        public ulong NumNodeBanks;

        /// <summary>
        /// 
        /// </summary>
        public List<PublicKey> NodeBanks;

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
        /// Deserialize a span of bytes into a <see cref="RootBank"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBank"/> structure.</returns>
        public static RootBank Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
            ReadOnlySpan<byte> span = data.AsSpan();
            List<PublicKey> nodeBanks = new(Constants.MaxNodeBanks);
            ReadOnlySpan<byte> nodeBanksBytes =
                span.Slice(Layout.NodeBanksOffset, Constants.MaxNodeBanks * PublicKey.PublicKeyLength);

            for (int i = 0; i < Constants.MaxNodeBanks - 1; i++)
            {
                nodeBanks.Add(nodeBanksBytes.GetPubKey(i * PublicKey.PublicKeyLength));
            }

            return new RootBank
            {
                Metadata = MetaData.Deserialize(span.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                OptimalUtilization = I80F48.Deserialize(span.Slice(Layout.OptimalUtilizationOffset, I80F48.Length)),
                OptimalRate = I80F48.Deserialize(span.Slice(Layout.OptimalRateOffset, I80F48.Length)),
                MaxRate = I80F48.Deserialize(span.Slice(Layout.MaxRateOffset, I80F48.Length)),
                NumNodeBanks = span.GetU64(Layout.NumNodeBanksOffset),
                NodeBanks = nodeBanks,
                DepositIndex = I80F48.Deserialize(span.Slice(Layout.DepositIndexOffset, I80F48.Length)),
                BorrowIndex = I80F48.Deserialize(span.Slice(Layout.BorrowIndexOffset, I80F48.Length)),
                LastUpdated = span.GetU64(Layout.LastUpdatedOffset)
            };
        }
    }
}