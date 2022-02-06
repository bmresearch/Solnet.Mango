using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TvSymbol
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Session { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ListedExchange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasIntraday { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> SupportedResolutions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MinMov { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PriceScale { get; set; }
    }
}
