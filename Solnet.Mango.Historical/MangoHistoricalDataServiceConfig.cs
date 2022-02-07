using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical
{
    /// <summary>
    /// 
    /// </summary>
    public class MangoHistoricalDataServiceConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string MangoStatsBaseUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EventHistoryBaseUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EventHistoryCandlesBaseUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MangoGroup { get; set; }
    }
}
