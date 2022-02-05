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
        public bool UseSingleBaseUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri MangoStatsBaseUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri EventHistoryBaseUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri SerumHistoryBaseUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MangoGroup { get; set; }
    }
}
