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
    public class VolumeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(StringToDecimalJsonConverter))]
        public decimal Volume { get; set; }
    }
}
