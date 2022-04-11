using Solnet.Mango.Models.Events;
using System.Collections.Generic;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents a fills snapshot from the websocket connection to the Mango Fills Service.
    /// <remarks>For more information: https://docs.mango.markets/development-resources/client-libraries/fills-websocket-feed</remarks>
    /// </summary>
    public class FillsSnapshot
    {
        /// <summary>
        /// The list of events in the snapshot.
        /// </summary>
        public List<string> Events { get; set; }

        /// <summary>
        /// The decoded events.
        /// </summary>
        public List<FillEvent> DecodedEvents { get; set; }

        /// <summary>
        /// The market of the snapshot.
        /// </summary>
        public string Market { get; set; }

        /// <summary>
        /// The slot of the snapshot.
        /// </summary>
        public ulong Slot { get; set; }

        /// <summary>
        /// The write version.
        /// </summary>
        public int WriteVersion { get; set; }
    }
}
