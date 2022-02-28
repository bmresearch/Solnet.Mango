using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models.Configs
{
    /// <summary>
    /// Represents the config of a market.
    /// </summary>
    public class MarketConfig
    {
        /// <summary>
        /// The name of the market.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The public key of the market.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// The base symbol of the market.
        /// </summary>
        public string BaseSymbol { get; set; }

        /// <summary>
        /// The decimals of the base token.
        /// </summary>
        public byte BaseDecimals { get; set; }

        /// <summary>
        /// The decimals of the quote token.
        /// </summary>
        public byte QuoteDecimals { get; set; }

        /// <summary>
        /// The market's index.
        /// </summary>
        public int MarketIndex { get; set; }

        /// <summary>
        /// The public key of the bids account.
        /// </summary>
        public string BidsKey { get; set; }

        /// <summary>
        /// The public key of the asks account.
        /// </summary>
        public string AsksKey { get; set; }

        /// <summary>
        /// The public key of the event queue account.
        /// </summary>
        public string EventsKey { get; set; }
    }
}
