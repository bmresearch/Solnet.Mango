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
    public class CandlestickProviderConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("supported_resolutions")]
        public List<string> SupportedResolutions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("supports_group_request")]
        public bool SupportsGroupRequest { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("supports_marks")]
        public bool SupportsMarks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("supports_search")]
        public bool SupportsSearch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("supports_timescale_marks")]
        public bool SupportsTimescaleMarks { get; set; }
    }
}
