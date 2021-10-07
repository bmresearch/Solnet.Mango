using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents an inner node in the Mango Perpetual Market order book side.
    /// <remarks>
    /// This is similar to Serum's slabs.
    /// </remarks>
    /// </summary>
    public class InnerNode : Node
    {
        /// <summary>
        /// Represents the layout of the <see cref="InnerNode"/> structure.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the prefix length value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int PrefixLengthOffset = 0;

            /// <summary>
            /// The offset at which the key value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int KeyOffset = 4;
            
            /// <summary>
            /// The offset at which the key value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int ChildrenOffset = 4;

            /// <summary>
            /// The number of entries in the array of children nodes.
            /// </summary>
            internal const int ChildrenLength = 2;
        }

        /// <summary>
        /// The prefix length.
        /// </summary>
        public uint PrefixLength;

        /// <summary>
        /// The children of this node.
        /// </summary>
        public List<uint> Children;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="InnerNode"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="InnerNode"/> structure.</returns>
        public static new InnerNode Deserialize(ReadOnlySpan<byte> data)
        {
            List<uint> children = new (ExtraLayout.ChildrenLength);
            ReadOnlySpan<byte> childrenBytes =
                data.GetSpan(ExtraLayout.ChildrenOffset, sizeof(uint) * ExtraLayout.ChildrenLength);
            
            for (int i = 0; i < ExtraLayout.ChildrenLength - 1 ; i++)
            {
                children.Add(childrenBytes.GetU32(i * sizeof(uint)));
            }
            
            return new InnerNode
            {
                Tag = NodeType.InnerNode,
                PrefixLength = data.GetU32(ExtraLayout.PrefixLengthOffset),
                Key = data.GetSpan(ExtraLayout.KeyOffset, Layout.KeyLength).ToArray(),
                Children = children,
            };
        }
    }
}