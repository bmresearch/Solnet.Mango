using Solnet.Mango.Historical.Converters;
using System;
using System.Text.Json.Serialization;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents the funding rate stats.
    /// </summary>
    public class FundingRateStats
    {
        /// <summary>
        /// The long funding.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal LongFunding { get; set; }

        /// <summary>
        /// The short funding.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal ShortFunding { get; set; }

        /// <summary>
        /// The open interest.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal OpenInterest { get; set; }

        /// <summary>
        /// The oracle price.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal BaseOraclePrice { get; set; }

        /// <summary>
        /// The time.
        /// </summary>
        public DateTime Time { get; set; }
    }
}
