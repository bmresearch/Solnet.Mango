using Solnet.Mango.Models.Matching;
using Solnet.Rpc;
using Solnet.Wallet;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Constants relating to the mango markets logic.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The maximum number of tokens.
        /// </summary>
        public const int MaxTokens = 16;

        /// <summary>
        /// The maximum number of pairs.
        /// </summary>
        public const int MaxPairs = MaxTokens - 1;

        /// <summary>
        /// The maximum number of node banks.
        /// </summary>
        public const int MaxNodeBanks = 8;

        /// <summary>
        /// The quote index.
        /// </summary>
        public const int QuoteIndex = MaxTokens - 1;

        /// <summary>
        /// The account info length.
        /// </summary>
        public const int InfoLength = 32;

        /// <summary>
        /// Maximum amount of open orders in perpetuals.
        /// </summary>
        public const int MaxPerpOpenOrders = 64;

        /// <summary>
        /// The maximum number of <see cref="Node"/> in an <see cref="OrderBookSide"/>.
        /// </summary>
        public const int MaxBookNodes = 1024;

        /// <summary>
        /// Maximum amount of numbers in the margin basket.
        /// </summary>
        public const int MaxNumInMarginBasket = 10;

        /// <summary>
        /// The maximum number of advanced orders in an <see cref="AdvancedOrdersAccount"/>.
        /// </summary>
        public const int MaxAdvancedOrders = 32;

        /// <summary>
        /// The mango group.
        /// </summary>
        public static readonly PublicKey MangoGroup = new("98pjRuQjK3qA6gXts96PqZT4Ze5QmnCmt3QYjhbUSPue");

        /// <summary>
        /// The mango cache.
        /// </summary>
        public static readonly PublicKey MangoCache = new("EBDRoayCDDUvDgCimta45ajQeXbexv7aKqJubruqpyvu");

        /// <summary>
        /// The mango group for <see cref="Cluster.DevNet"/>.
        /// </summary>
        public static readonly PublicKey DevNetMangoGroup = new("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA");
    }
}