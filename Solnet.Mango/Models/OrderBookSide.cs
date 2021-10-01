using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Solnet.Mango.Models
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
            /// 
            /// </summary>
            internal const int Length = 90152;

            /// <summary>
            /// 
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int BumpIndexOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int FreeListLengthOffset = 16;

            /// <summary>
            /// 
            /// </summary>
            internal const int FreeListHeadOffset = 24;

            /// <summary>
            /// 
            /// </summary>
            internal const int RootNodeOffset = 28;

            /// <summary>
            /// 
            /// </summary>
            internal const int LeafCountOffset = 32;

            /// <summary>
            /// 
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
        /// 
        /// </summary>
        public ulong FreeListLength;

        /// <summary>
        /// 
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
        /// Gets the list of orders in the order book.
        /// </summary>
        /// <returns></returns>
        public List<OpenOrder> GetOrders()
        {
            return (from node in Nodes
                where node is LeafNode
                select (LeafNode)node
                into leafNode
                select new OpenOrder
                {
                    RawPrice = leafNode.Price, 
                    RawQuantity = leafNode.Quantity, 
                    ClientOrderId = leafNode.ClientOrderId, 
                    Owner = leafNode.Owner,
                    OrderIndex = leafNode.OwnerSlot,
                    OrderId = new BigInteger(leafNode.Key)
                }).ToList();
        }
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="OrderBookSide"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="OrderBookSide"/> structure.</returns>
        public static OrderBookSide Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length) throw new Exception("data length is invalid");
            ReadOnlySpan<byte> span = data.AsSpan();

            List<Node> nodes = new(Constants.MaxBookNodes);
            ReadOnlySpan<byte> nodesBytes = span[Layout.NodesOffset..];
            for (int i = 0; i < Constants.MaxBookNodes - 1; i++)
            {
                nodes.Add(Node.Deserialize(nodesBytes.Slice(i *  Node.Layout.Length, Node.Layout.Length)));
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