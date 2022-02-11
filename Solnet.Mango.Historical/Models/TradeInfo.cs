using System;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents a trade's info.
    /// </summary>
    public class TradeInfo
    {
        /// <summary>
        /// The market address.
        /// </summary>
        public string MarketAddress { get; set; }

        /// <summary>
        /// The price of the trade.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The size of the trade.
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// The side of the trade.
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// The time of the trade.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// The order id.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// The fee cost of the trade.
        /// </summary>
        public decimal FeeCost { get; set; }

        /// <summary>
        /// The parsed date time.
        /// </summary>
        public DateTime DateTime => DateTime.UnixEpoch.AddMilliseconds(Time);
    }
}
