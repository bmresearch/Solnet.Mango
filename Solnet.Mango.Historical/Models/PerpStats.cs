namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PerpStats : Stats
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal OldestLongFunding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal OldestShortFunding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal LatestLongFunding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal LatestShortFunding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal OpenInterest { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal BaseOraclePrice { get; set; }
    }
}
