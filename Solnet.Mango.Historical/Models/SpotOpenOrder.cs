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
    /// Represents a spot open order.
    /// </summary>
    public class SpotOpenOrder
    {
        /// <summary>
        /// The timestamp.
        /// </summary>
        [JsonConverter(typeof(StringToDateTimeJsonConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The market address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The serum dex program id.
        /// </summary>
        public string ProgramId { get; set; }

        /// <summary>
        /// The base currency symbol.
        /// </summary>
        public string BaseCurrency { get; set; }

        /// <summary>
        /// The quote currency symbol.
        /// </summary>
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// Whether the corresponding event was a fill.
        /// </summary>
        public bool Fill { get; set; }

        /// <summary>
        /// Whether the corresponding event was an out.
        /// </summary>
        public bool Out { get; set; }

        /// <summary>
        /// Whether the trade was a bid.
        /// </summary>
        public bool Bid { get; set; }

        /// <summary>
        /// Whether the trade was a maker.
        /// </summary>
        public bool Maker { get; set; }

        /// <summary>
        /// The slot of the order in the open orders account.
        /// </summary>
        [JsonConverter(typeof(StringToByteJsonConverter))]
        public byte OpenOrderSlot { get; set; }

        /// <summary>
        /// The fee tier.
        /// </summary>
        [JsonConverter(typeof(StringToByteJsonConverter))]
        public byte FeeTier { get; set; }

        /// <summary>
        /// The native quantity released.
        /// </summary>
        [JsonConverter(typeof(StringToUlongJsonConverter))]
        public ulong NativeQuantityReleased { get; set; }

        /// <summary>
        /// The native quantity paid.
        /// </summary>
        [JsonConverter(typeof(StringToUlongJsonConverter))]
        public ulong NativeQuantityPaid { get; set; }

        /// <summary>
        /// The native fee or rebate.
        /// </summary>
        [JsonConverter(typeof(StringToUlongJsonConverter))]
        public ulong NativeFeeOrRebate { get; set; }

        /// <summary>
        /// The order id.
        /// </summary>
        [JsonConverter(typeof(StringToBigIntegerJsonConverter))]
        public BigInteger OrderId { get; set; }

        /// <summary>
        /// The open orders account.
        /// </summary>
        public string OpenOrders { get; set; }

        /// <summary>
        /// The client's order id.
        /// </summary>
        [JsonConverter(typeof(StringToUlongJsonConverter))]
        public ulong ClientOrderId { get; set; }

        /// <summary>
        /// The mango uuid.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The sequence number.
        /// </summary>
        [JsonPropertyName("seqNum")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// The decimals of the base token.
        /// </summary>
        public int BaseTokenDecimals { get; set; }

        /// <summary>
        /// The decimals of the quote token.
        /// </summary>
        public int QuoteTokenDecimals { get; set; }

        /// <summary>
        /// The side of the order.
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// The price of the order.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The fee cost.
        /// </summary>
        public decimal FeeCost { get; set; }

        /// <summary>
        /// The size of the order.
        /// </summary>
        public decimal Size { get; set; }
    }
}
