namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the type of an account in Mango Markets.
    /// </summary>
    public enum DataType : byte
    {
        /// <summary>
        /// The Mango group which specifies which assets are cross-collateralized.
        /// </summary>
        MangoGroup = 0,

        /// <summary>
        /// The Mango account for a user interacting with the protocol.
        /// </summary>
        MangoAccount = 1,

        /// <summary>
        /// The root bank account for Mango.
        /// </summary>
        RootBank = 2,

        /// <summary>
        /// The node bank account for Mango.
        /// </summary>
        NodeBank = 3,

        /// <summary>
        /// The account for a Mango perpetual market.
        /// </summary>
        PerpMarket = 4,

        /// <summary>
        /// The account for bid orders related to an on-chain order book for Mango's perpetual markets.
        /// </summary>
        Bids = 5,

        /// <summary>
        /// The account for ask orders related to an on-chain order book for Mango's perpetual markets.
        /// </summary>
        Asks = 6,

        /// <summary>
        /// The account for the Mango's caches.
        /// </summary>
        MangoCache = 7,

        /// <summary>
        /// The event queue account for an on-chain perpetual market.
        /// </summary>
        EventQueue = 8,

        /// <summary>
        /// An advanced orders account for advanced order types such as conditional trigger orders.
        /// </summary>
        AdvancedOrders = 9,

        /// <summary>
        /// The referrer memory account.
        /// </summary>
        ReferrerMemory = 10,

        /// <summary>
        /// The referrer id record.
        /// </summary>
        ReferrerIdRecord = 11
    }
}