using System.Numerics;

namespace Solnet.Mango.Models.Matching
{
    /// <summary>
    /// A base order.
    /// </summary>
    public abstract class OrderBase
    {
        /// <summary>
        /// The order id.
        /// </summary>
        public BigInteger OrderId;

        /// <summary>
        /// The client's order id.
        /// </summary>
        public ulong ClientOrderId;
    }
}