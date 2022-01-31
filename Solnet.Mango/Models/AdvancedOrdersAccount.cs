using System;
using System.Collections.Generic;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents an advanced orders account in mango.
    /// </summary>
    public class AdvancedOrdersAccount
    {
        /// <summary>
        /// The layout of the <see cref="AdvancedOrdersAccount"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="AdvancedOrdersAccount"/> structure.
            /// </summary>
            internal const int Length = 2568;

            /// <summary>
            /// The length at which the account metadata begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The length at which the advanced orders begin.
            /// </summary>
            internal const int AdvancedOrdersOffset = 8;
        }

        /// <summary>
        /// The account metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The advanced orders.
        /// </summary>
        public List<AdvancedOrder> AdvancedOrders;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="AdvancedOrdersAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="AdvancedOrdersAccount"/> structure.</returns>
        public static AdvancedOrdersAccount Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"invalid data length, expected {Layout.Length}, got {data.Length}");

            List<AdvancedOrder> orders = new();
            for (int i = 0; i < Constants.MaxAdvancedOrders; i++)
            {
                orders.Add(AdvancedOrder.Deserialize(
                        data.Slice(i * AdvancedOrder.Layout.Length + Layout.AdvancedOrdersOffset,
                        AdvancedOrder.Layout.Length)));
            }

            return new AdvancedOrdersAccount
            {
                Metadata = MetaData.Deserialize(data.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
            };
        }
    }
}
