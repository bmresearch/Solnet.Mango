namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents the margin lending stats.
    /// </summary>
    public class MarginLendingStats : Stats
    {
        /// <summary>
        /// The total deposits.
        /// </summary>
        public decimal TotalDeposits { get; set; }

        /// <summary>
        /// The total borrows.
        /// </summary>
        public decimal TotalBorrows { get; set; }

        /// <summary>
        /// The deposit index.
        /// </summary>
        public decimal DepositIndex { get; set; }

        /// <summary>
        /// The borrow index.
        /// </summary>
        public decimal BorrowIndex { get; set; }

        /// <summary>
        /// The utilization.
        /// </summary>
        public decimal Utilization { get; set; }

        /// <summary>
        /// The deposit rate.
        /// </summary>
        public decimal DepositRate { get; set; }

        /// <summary>
        /// The borrow rate.
        /// </summary>
        public decimal BorrowRate { get; set; }
    }
}
