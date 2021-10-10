namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the types of collateralized tokens and positions in Mango Markets.
    /// </summary>
    public enum AssetType : byte
    {
        /// <summary>
        /// A collateralized token.
        /// </summary>
        Token = 0,

        /// <summary>
        /// A perpetual position.
        /// </summary>
        Perp = 1
    }
}