using Solnet.Serum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the perp order types.
    /// </summary>
    public enum PerpOrderType : byte
    {
        /// <summary>
        /// A limit order.
        /// </summary>
        Limit,

        /// <summary>
        /// An immediate or cancel order.
        /// </summary>
        ImmediateOrCancel,

        /// <summary>
        /// A post only order.
        /// </summary>
        PostOnly,

        /// <summary>
        /// A market order.
        /// </summary>
        Market,

        /// <summary>
        /// 
        /// </summary>
        PostOnlySlide
    }
}
