namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the types of markets available.
    /// </summary>
    public enum MarketType : byte
    {
        /// <summary>
        /// A spot margined market.
        /// </summary>
        Spot,

        /// <summary>
        /// A perpetual market.
        /// </summary>
        Perpetual
    }
}