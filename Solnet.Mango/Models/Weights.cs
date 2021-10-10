using Solnet.Serum.Models;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the weights to calculate account health ratios.
    /// </summary>
    public struct Weights
    {
        /// <summary>
        /// The spot asset weight.
        /// </summary>
        public double SpotAssetWeight;

        /// <summary>
        /// The spot liability weight.
        /// </summary>
        public double SpotLiabilityWeight;

        /// <summary>
        /// The perpetual asset weight.
        /// </summary>
        public double PerpAssetWeight;

        /// <summary>
        /// The perpetual liability weight.
        /// </summary>
        public double PerpLiabilityWeight;
    }

    /// <summary>
    /// Represents leverage stats for a given market.
    /// </summary>
    public struct LeverageStats
    {
        /// <summary>
        /// Maximum leverage.
        /// </summary>
        public double Maximum;

        /// <summary>
        /// The amount of deposits formatted.
        /// </summary>
        public double UiDeposit;

        /// <summary>
        /// The amount of deposits.
        /// </summary>
        public double Deposits;

        /// <summary>
        /// The amount of borrows formatted.
        /// </summary>
        public double UiBorrow;

        /// <summary>
        /// The amount of borrows.
        /// </summary>
        public double Borrows;
    }

    /// <summary>
    /// Represents the funds in an <see cref="OpenOrdersAccount"/>.
    /// </summary>
    public struct OpenOrdersStats
    {
        /// <summary>
        /// The quote free.
        /// </summary>
        public double QuoteFree;

        /// <summary>
        /// The quote locked.
        /// </summary>
        public double QuoteLocked;

        /// <summary>
        /// The base free.
        /// </summary>
        public double BaseFree;

        /// <summary>
        /// The base locked.
        /// </summary>
        public double BaseLocked;
    }
}