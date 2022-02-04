using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// The types of advanced orders.
    /// </summary>
    public enum AdvancedOrderType : byte
    {
        /// <summary>
        /// Triggered by the perp price.
        /// </summary>
        PerpTrigger,

        /// <summary>
        /// Triggered by the spot price.
        /// <remark>Not implemented yet.</remark>
        /// </summary>
        SpotTrigger,
    }
}
