using Solnet.Wallet;
using System.Numerics;

namespace Solnet.Mango.Models.Matching
{
    /// <summary>
    /// Represents an open order in the Mango order book.
    /// </summary>
    public class OpenOrder : OrderBase
    {
        /// <summary>
        /// The index of the order within the lists of order data in the <see cref="MangoAccount"/>.
        /// </summary>
        public int OrderIndex;

        /// <summary>
        /// The raw value for the price of the order.
        /// <remarks>This value needs to be converted according to decimals and lot sizes.</remarks>
        /// </summary>
        public long RawPrice;

        /// <summary>
        /// The raw value for the quantity of the order.
        /// <remarks>This value needs to be converted according to decimals and lot sizes.</remarks>
        /// </summary>
        public long RawQuantity;

        /// <summary>
        /// The owner of this order.
        /// </summary>
        public PublicKey Owner;

        /// <summary>
        /// The timestamp.
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// The timestamp.
        /// </summary>
        public ulong ExpiryTimestamp;

        /// <summary>
        /// The time in force.
        /// </summary>
        public byte TimeInForce;

        /// <summary>
        /// The order type.
        /// </summary>
        public PerpOrderType OrderType;
    }
}