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
    public abstract class Stats
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Hourly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
}
