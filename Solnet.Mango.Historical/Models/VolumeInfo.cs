using Solnet.Mango.Historical.Converters;
using System.Text.Json.Serialization;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents the volume info for a market.
    /// </summary>
    public class VolumeInfo
    {
        /// <summary>
        /// The volume.
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal Volume { get; set; }
    }
}
