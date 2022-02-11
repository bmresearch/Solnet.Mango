namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents the perpetual markets stats.
    /// </summary>
    public class PerpStats : Stats
    {
        /// <summary>
        /// The oldest long funding for the period.
        /// </summary>
        public decimal OldestLongFunding { get; set; }

        /// <summary>
        /// The oldest short funding for the period.
        /// </summary>
        public decimal OldestShortFunding { get; set; }

        /// <summary>
        /// The latest long funding for the period.
        /// </summary>
        public decimal LatestLongFunding { get; set; }

        /// <summary>
        /// The latest short funding for the period.
        /// </summary>
        public decimal LatestShortFunding { get; set; }

        /// <summary>
        /// The open interest
        /// </summary>
        public decimal OpenInterest { get; set; }

        /// <summary>
        /// The oracle price.
        /// </summary>
        public decimal BaseOraclePrice { get; set; }
    }
}
