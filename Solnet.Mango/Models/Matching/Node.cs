using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using System;

namespace Solnet.Mango.Models.Matching
{
    /// <summary>
    /// Represents a node in the Mango Perpetual Market order book side.
    /// <remarks>
    /// This is similar to Serum's slabs.
    /// </remarks>
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Represents the layout of the <see cref="Node"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of a node.
            /// </summary>
            internal const int Length = 88;

            /// <summary>
            /// The offset at which the tag value begins.
            /// </summary>
            internal const int TagOffset = 0;

            /// <summary>
            /// The offset at which the slab node's blob starts.
            /// <remarks>
            /// This value is valid before reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int BlobOffset = 4;

            /// <summary>
            /// The size of the data for the slab node's blob.
            /// </summary>
            internal const int BlobSpanLength = 84;

            /// <summary>
            /// The length of the key value.
            /// </summary>
            internal const int KeyLength = 16;
        }

        /// <summary>
        /// The node type.
        /// </summary>
        public NodeType Tag;

        /// <summary>
        /// The key of the node.
        /// </summary>
        public byte[] Key;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="Node"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="Node"/> structure.</returns>
        public static Node Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new Exception("data length is invalid");

            uint tag = data.GetU32(Layout.TagOffset);
            if (tag is (byte)NodeType.Uninitialized or (byte)NodeType.LastFreeNode or (byte)NodeType.FreeNode)
                return null;

            ReadOnlySpan<byte> blob = data.GetSpan(Layout.BlobOffset, Layout.BlobSpanLength);

            return tag switch
            {
                (byte)NodeType.FreeNode => FreeNode.Deserialize(blob),
                (byte)NodeType.InnerNode => InnerNode.Deserialize(blob),
                (byte)NodeType.LeafNode => LeafNode.Deserialize(blob),
                _ => null
            };
        }
    }
}