using Solnet.Wallet;
using System.Numerics;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenOrder
    {
        /// <summary>
        /// The index of the order within the <see cref="OpenOrdersAccount"/> data.
        /// </summary>
        public int OrderIndex;
        
        /// <summary>
        /// The order id.
        /// </summary>
        public BigInteger OrderId;

        /// <summary>
        /// The client's order id.
        /// </summary>
        public ulong ClientOrderId;

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
    }
}