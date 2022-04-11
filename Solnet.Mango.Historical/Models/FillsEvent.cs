using Solnet.Mango.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents a fills event from the websocket connection to the Mango Fills Service.
    /// <remarks>For more information: https://docs.mango.markets/development-resources/client-libraries/fills-websocket-feed</remarks>
    /// </summary>
    public class FillsEvent
    {
        /// <summary>
        /// The event.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// The decoded event.
        /// </summary>
        public FillEvent DecodedEvent { get; set; }

        /// <summary>
        /// The market name.
        /// </summary>
        public string Market { get; set; }
        
        /// <summary>
        /// The market's event queue.
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// The status of the event.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The slot of the event.
        /// </summary>
        public ulong Slot { get; set; }

        /// <summary>
        /// The write version.
        /// </summary>
        public int WriteVersion { get; set; }
    }
}
