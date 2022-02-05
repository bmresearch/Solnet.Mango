using Solnet.Mango.Historical.Converters;
using System;
using System.Text.Json.Serialization;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class FundingRateStats
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal LongFunding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal ShortFunding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal OpenInterest { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal BaseOraclePrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Time { get; set; }
    }
}
