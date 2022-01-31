using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// The trigger conditions for the <see cref="MangoProgram"/> advanced orders.
    /// </summary>
    public enum TriggerCondition : byte
    {
        /// <summary>
        /// The advanced order is triggered when the conditional value is above the trigger value.
        /// </summary>
        Above,

        /// <summary>
        /// The advanced order is triggered when the conditional value is below the trigger value.
        /// </summary>
        Below
    }
}
