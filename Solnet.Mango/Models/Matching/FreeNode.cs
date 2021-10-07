using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a free node in the Mango Perpetual Market order book side.
    /// <remarks>
    /// This is similar to Serum's slabs.
    /// </remarks>
    /// </summary>
    public class FreeNode : Node
    {
        /// <summary>
        /// Represents the layout of the <see cref="FreeNode"/> structure.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the next value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int NextOffset = 4;
        }

        /// <summary>
        /// The next node.
        /// </summary>
        public uint Next;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="FreeNode"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="FreeNode"/> structure.</returns>
        public static new FreeNode Deserialize(ReadOnlySpan<byte> data)
        {
            return new FreeNode { Tag = NodeType.FreeNode, Next = data.GetU32(ExtraLayout.NextOffset) };
        }
    }
}