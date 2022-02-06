using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents the OHLCV data.
    /// </summary>
    public class OHLCV
    {
        /// <summary>
        /// Bar time. Unix timestamp (UTC)
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Closing price
        /// </summary>
        public decimal Close { get; set; }

        /// <summary>
        /// Opening price
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// High price
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Low price
        /// </summary>
        public decimal Low { get; set; }


        /// <summary>
        /// Volume
        /// </summary>
        public decimal Volume { get; set; }
    }
}
