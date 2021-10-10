namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the types of healths for an account in Mango Markets.
    /// </summary>
    public enum HealthType : byte
    {
        /// <summary>
        /// The maintenance health.
        /// </summary>
        Maintenance,

        /// <summary>
        /// The initialization health.
        /// </summary>
        Initialization
    }
}