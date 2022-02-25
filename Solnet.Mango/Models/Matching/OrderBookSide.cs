using Solnet.Mango.Models.Matching;
using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Solnet.Mango.Models.Matching
{
    /// <summary>
    /// Represents an order book side in Mango Perpetual Markets.
    /// </summary>
    public class OrderBookSide
    {
        /// <summary>
        /// Represents the layout of the <see cref="OrderBookSide"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="OrderBookSide"/> structure.
            /// </summary>
            internal const int Length = 90152;

            /// <summary>
            /// The offset at which the metadata begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the bump index begins.
            /// </summary>
            internal const int BumpIndexOffset = 8;

            /// <summary>
            /// The offset at which the free list length begins.
            /// </summary>
            internal const int FreeListLengthOffset = 16;

            /// <summary>
            /// The offset at which the free list head begins.
            /// </summary>
            internal const int FreeListHeadOffset = 24;

            /// <summary>
            /// The offset at which the root node begins.
            /// </summary>
            internal const int RootNodeOffset = 28;

            /// <summary>
            /// The offset at which the leaf count begins.
            /// </summary>
            internal const int LeafCountOffset = 32;

            /// <summary>
            /// The offset at which the nodes begin.
            /// </summary>
            internal const int NodesOffset = 40;
        }

        /// <summary>
        /// The account metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The bump index.
        /// </summary>
        public ulong BumpIndex;

        /// <summary>
        /// The free list length.
        /// </summary>
        public ulong FreeListLength;

        /// <summary>
        /// The free list head.
        /// </summary>
        public uint FreeListHead;

        /// <summary>
        /// The root node.
        /// </summary>
        public uint RootNode;

        /// <summary>
        /// The number of leaf nodes.
        /// </summary>
        public ulong LeafCount;

        /// <summary>
        /// The nodes.
        /// </summary>
        public List<Node> Nodes;

        /// <summary>
        /// The orders.
        /// </summary>
        private List<OpenOrder> _orders;

        /// <summary>
        /// Gets the list of orders in the order book.
        /// </summary>
        /// <param name="includeExpired">Whether to include expired orders in the list of orders or not.</param>
        /// <returns>A list of open orders on the book side.</returns>
        public List<OpenOrder> GetOrders(bool includeExpired = false)
        {
            bool isBids = Metadata.DataType == DataType.Bids;

            _orders = new List<OpenOrder>();

            foreach (var node in Nodes)
            {
                if (node is LeafNode leafNode)
                {
                    var valid = leafNode.IsValid();
                    if (valid || includeExpired)
                    {
                        _orders.Add(new OpenOrder
                        {
                            RawPrice = leafNode.Price,
                            RawQuantity = leafNode.Quantity,
                            ClientOrderId = leafNode.ClientOrderId,
                            Owner = leafNode.Owner,
                            OrderIndex = leafNode.OwnerSlot,
                            OrderId = new BigInteger(leafNode.Key),
                            Timestamp = leafNode.Timestamp,
                            ExpiryTimestamp = leafNode.TimeInForce != 0 ? leafNode.Timestamp + leafNode.TimeInForce : ulong.MaxValue,
                            TimeInForce = leafNode.TimeInForce,
                            OrderType = leafNode.OrderType,
                        });
                    }
                }
            }

            if (!isBids)
            {
                _orders.Sort(Comparer<OpenOrder>.Create((order, order1) => order.RawPrice.CompareTo(order1.RawPrice)));
            }
            else
            {
                _orders.Sort(Comparer<OpenOrder>.Create((order, order1) => order1.RawPrice.CompareTo(order.RawPrice)));
            }
            return _orders;
        }

        /// <summary>
        /// Gets the price reached for a given quantity up the book.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The price or zero if desired amount is not on the book.</returns>
        public long GetImpactPrice(long quantity)
        {
            long s = 0;
            var orders = _orders != null ? _orders : GetOrders();
            foreach (var order in orders)
            {
                s += order.RawQuantity;
                if (s > quantity)
                    return order.RawPrice;
            }
            return 0;
        }

        /// <summary>
        /// Gets the best order on the book.
        /// </summary>
        /// <returns>The order.</returns>
        public OpenOrder GetBest()
        {
            if (_orders != null) return _orders.FirstOrDefault();
            return GetOrders().FirstOrDefault();
        }


        /// <summary>
        /// Deserialize a span of bytes into a <see cref="OrderBookSide"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="OrderBookSide"/> structure.</returns>
        public static OrderBookSide Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");
            ReadOnlySpan<byte> span = data.AsSpan();

            List<Node> nodes = new(Constants.MaxBookNodes);
            ReadOnlySpan<byte> nodesBytes = span[Layout.NodesOffset..];
            for (int i = 0; i < Constants.MaxBookNodes - 1; i++)
            {
                nodes.Add(Node.Deserialize(nodesBytes.Slice(i * Node.Layout.Length, Node.Layout.Length)));
            }

            return new OrderBookSide
            {
                Metadata = MetaData.Deserialize(span.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                BumpIndex = span.GetU32(Layout.BumpIndexOffset),
                FreeListLength = span.GetU64(Layout.FreeListLengthOffset),
                FreeListHead = span.GetU32(Layout.FreeListHeadOffset),
                RootNode = span.GetU32(Layout.RootNodeOffset),
                LeafCount = span.GetU64(Layout.LeafCountOffset),
                Nodes = nodes,
            };
        }
    }
}