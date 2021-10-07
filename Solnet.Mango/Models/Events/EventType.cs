namespace Solnet.Mango.Models
{
    /// <summary>
    /// The type of event raised by the <see cref="EventQueue"/>.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// An event that represents an order fill.
        /// </summary>
        Fill,
        
        /// <summary>
        /// An event that represents an order removal.
        /// </summary>
        Out,
        
        /// <summary>
        /// An event that represents a liquidation.
        /// </summary>
        Liquidate
    }
}