using Solnet.Mango.Historical.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TvHistory
    {
        /// <summary>
        /// Status code. Expected values: ok | error | no_data
        /// </summary>
        /// 
        [JsonPropertyName("s")]
        public TvBarStatus Status { get; set; }

        /// <summary>
        /// Error message. Should be present only when status = 'error'
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("o")]
        [JsonConverter(typeof(StringArrayToDecimalArrayJsonConverter))]
        public decimal[] Open { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("c")]
        [JsonConverter(typeof(StringArrayToDecimalArrayJsonConverter))]
        public decimal[] Close { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("h")]
        [JsonConverter(typeof(StringArrayToDecimalArrayJsonConverter))]
        public decimal[] High { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("l")]
        [JsonConverter(typeof(StringArrayToDecimalArrayJsonConverter))]
        public decimal[] Low { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("v")]
        [JsonConverter(typeof(StringArrayToDecimalArrayJsonConverter))]
        public decimal[] Volume { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("t")]
        public int[] Timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public OHLCV[] ParsedOHLCVs { get; set; }

        /// <summary>
        /// Should be the time of the closest available bar in the past if there is no data (status code is no_data) in the requested period (optional).
        /// </summary>
        public DateTime? NextTime { get; set; }
    }
}
