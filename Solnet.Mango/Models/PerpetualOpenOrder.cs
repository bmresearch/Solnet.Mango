using Solnet.Serum.Models;
namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents an open order in the <see cref="MangoAccount"/> pertaining to perpetual markets.
    /// </summary>
    public class PerpetualOpenOrder : OrderBase
    {
        /// <summary>
        /// The side of the order.
        /// </summary>
        public Side Side;

        /// <summary>
        /// The index of the market.
        /// </summary>
        public byte MarketIndex;
    }
}