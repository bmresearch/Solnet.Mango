namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SpotStats : Stats
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalDeposits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TotalBorrows { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DepositIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal BorrowIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Utilization { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DepositRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal BorrowRate { get; set; }
    }
}
