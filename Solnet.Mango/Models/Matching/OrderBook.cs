using Solnet.Mango.Models.Matching;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Solnet.Mango.Models.Matching
{
    /// <summary>
    /// Represents an order book in Mango Perpetual Markets.
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// The bids in the order book.
        /// </summary>
        public OrderBookSide Bids;

        /// <summary>
        /// The asks in the order book.
        /// </summary>
        public OrderBookSide Asks;

        /// <summary>
        /// Gets the orders in the bid side of the order book.
        /// </summary>
        /// <returns>A list of orders.</returns>
        public List<OpenOrder> GetBids()
        {
            if (Bids == null) return new List<OpenOrder>();
            return (from node in Bids.Nodes
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
        /// Gets the orders in the bid side of the order book.
        /// </summary>
        /// <returns>A list of orders.</returns>
        public List<OpenOrder> GetAsks()
        {
            if (Asks == null) return new List<OpenOrder>();
            return (from node in Asks.Nodes
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
    }
}