using Solnet.Mango.Historical.Converters;
using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents a perp trade.
    /// </summary>
    public class PerpTrade
    {
        /// <summary>
        /// The timestamp.
        /// </summary>
        [JsonConverter(typeof(StringToDateTimeJsonConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The address of the market.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The sequence number.
        /// </summary>
        [JsonConverter(typeof(StringToUlongJsonConverter))]
        public ulong SequenceNumber { get; set; }

        /// <summary>
        /// The maker's fee.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal MakerFee { get; set; }

        /// <summary>
        /// The taker's fee.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
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
        [JsonConverter(typeof(StringToBigIntegerJsonConverter))]
        public BigInteger MakerOrderId { get; set; }

        /// <summary>
        /// The taker's mango account.
        /// </summary>
        public string Taker { get; set; }

        /// <summary>
        /// The taker's order id.
        /// </summary>
        [JsonConverter(typeof(StringToBigIntegerJsonConverter))]
        public BigInteger TakerOrerId { get; set; }

        /// <summary>
        /// The price.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal Price { get; set; }

        /// <summary>
        /// The quantity.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The maker's client order id.
        /// </summary>
        [JsonConverter(typeof(StringToUlongJsonConverter))]
        public ulong MakerClientOrderId { get; set; }

        /// <summary>
        /// The teaker's client order id.
        /// </summary>
        [JsonConverter(typeof(StringToUlongJsonConverter))]
        public ulong TakerClientOrderId { get; set; }
    }
}
