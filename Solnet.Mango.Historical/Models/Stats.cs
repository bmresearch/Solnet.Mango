using System;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents stats.
    /// </summary>
    public abstract class Stats
    {
        /// <summary>
        /// The hourly snapshot.
        /// </summary>
        public DateTime Hourly { get; set; }

        /// <summary>
        /// The time.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The name of the market or token.
        /// </summary>
        public string Name { get; set; }
    }
}
