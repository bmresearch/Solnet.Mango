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
}
