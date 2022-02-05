using System;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TradeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string MarketAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal FeeCost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTime => DateTime.UnixEpoch.AddMilliseconds(Time);
    }
}
