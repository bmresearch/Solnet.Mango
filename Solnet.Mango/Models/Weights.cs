using Solnet.Mango.Types;
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
        public I80F48 SpotAssetWeight;

        /// <summary>
        /// The spot liability weight.
        /// </summary>
        public I80F48 SpotLiabilityWeight;

        /// <summary>
        /// The perpetual asset weight.
        /// </summary>
        public I80F48 PerpAssetWeight;

        /// <summary>
        /// The perpetual liability weight.
        /// </summary>
        public I80F48 PerpLiabilityWeight;
    }

    /// <summary>
    /// Represents leverage stats for a given market.
    /// </summary>
    public struct LeverageStats
    {
        /// <summary>
        /// Maximum leverage.
        /// </summary>
        public I80F48 Maximum;

        /// <summary>
        /// The amount of deposits formatted.
        /// </summary>
        public I80F48 UiDeposit;

        /// <summary>
        /// The amount of deposits.
        /// </summary>
        public I80F48 Deposits;

        /// <summary>
        /// The amount of borrows formatted.
        /// </summary>
        public I80F48 UiBorrow;

        /// <summary>
        /// The amount of borrows.
        /// </summary>
        public I80F48 Borrows;
    }

    /// <summary>
    /// Represents the funds in an <see cref="OpenOrdersAccount"/>.
    /// </summary>
    public struct OpenOrdersStats
    {
        /// <summary>
        /// The quote free.
        /// </summary>
        public I80F48 QuoteFree;

        /// <summary>
        /// The quote locked.
        /// </summary>
        public I80F48 QuoteLocked;

        /// <summary>
        /// The base free.
        /// </summary>
        public I80F48 BaseFree;

        /// <summary>
        /// The base locked.
        /// </summary>
        public I80F48 BaseLocked;
    }
}