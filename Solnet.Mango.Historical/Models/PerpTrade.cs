using Solnet.Mango.Historical.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents a perp trade.
    /// </summary>
    [JsonConverter(typeof(PerpTradeJsonConverter))]
    public class PerpTrade
    {
        /// <summary>
        /// The timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The address of the market.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The sequence number.
        /// </summary>
        public ulong SequenceNumber { get; set; }

        /// <summary>
        /// The maker's fee.
        /// </summary>
        public decimal MakerFee { get; set; }

        /// <summary>
        /// The taker's fee.
        /// </summary>
        public decimal TakerFee { get; set; }

        /// <summary>
        /// The taker's side.
        /// </summary>
        public string TakerSide { get; set; }

        /// <summary>
        /// The maker's mango account.
        /// </summary>
        public string Maker { get; set; }

        /// <summary>
        /// The maker's order id.
        /// </summary>
        public BigInteger MakerOrderId { get; set; }

        /// <summary>
        /// The taker's mango account.
        /// </summary>
        public string Taker { get; set; }

        /// <summary>
        /// The taker's order id.
        /// </summary>
        public BigInteger TakerOrerId { get; set; }

        /// <summary>
        /// The price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// The maker's client order id.
        /// </summary>
        public ulong MakerClientOrderId { get; set; }

        /// <summary>
        /// The teaker's client order id.
        /// </summary>
        public ulong TakerClientOrderId { get; set; }
    }
}
