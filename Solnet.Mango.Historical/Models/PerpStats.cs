using Solnet.Mango.Models.Perpetuals;
using System;
using System.Collections.Generic;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// Represents the perpetual markets stats.
    /// </summary>
    public class PerpStats : Stats
    {
        /// <summary>
        /// The oldest long funding for the period.
        /// </summary>
        public decimal OldestLongFunding { get; set; }

        /// <summary>
        /// The oldest short funding for the period.
        /// </summary>
        public decimal OldestShortFunding { get; set; }

        /// <summary>
        /// The latest long funding for the period.
        /// </summary>
        public decimal LatestLongFunding { get; set; }

        /// <summary>
        /// The latest short funding for the period.
        /// </summary>
        public decimal LatestShortFunding { get; set; }

        /// <summary>
        /// The open interest
        /// </summary>
        public decimal OpenInterest { get; set; }

        /// <summary>
        /// The oracle price.
        /// </summary>
        public decimal BaseOraclePrice { get; set; }

        /// <summary>
        /// Calculates the funding rate from the indexed long and short funding data.
        /// </summary>
        /// <param name="oldestLongFunding">The oldest long funding.</param>
        /// <param name="oldestShortFunding">The oldest short funding.</param>
        /// <param name="latestLongFunding">the latest long funding.</param>
        /// <param name="latestShortFunding">The latest short funding.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <param name="oraclePrice">The oracle price.</param>
        /// <param name="baseDecimals">The decimals of the base token.</param>
        /// <param name="quoteDecimals">The decimals of the quote token.</param>
        /// <returns>The fundin rate per hourly period.</returns>
        public static decimal CalculateFundingRate(decimal oldestLongFunding, decimal oldestShortFunding,
            decimal latestLongFunding, decimal latestShortFunding, decimal oraclePrice, PerpMarket perpMarket,
            byte baseDecimals, byte quoteDecimals)
        {
            if (perpMarket == null) return 0m;

            var startFunding = (oldestLongFunding + oldestShortFunding) / 2;
            var endFunding = (latestLongFunding + latestShortFunding) / 2;
            var fundingDiff = endFunding - startFunding;

            var fundingInQuoteDecimals = fundingDiff / (decimal) Math.Pow(10, quoteDecimals);
            var basePriceInBaseLots = oraclePrice * perpMarket.BaseLotsToNumber(1m, baseDecimals);

            return (fundingInQuoteDecimals / basePriceInBaseLots) * 100;
        }

        /// <summary>
        /// Calculates the funding rate from the historical long and short funding of a given market.
        /// </summary>
        /// <param name="fundingRates">The historical funding rates data.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <param name="baseDecimals">The decimals of the base token.</param>
        /// <param name="quoteDecimals">The decimals of the quote token.</param>
        /// <returns>The fundin rate per hourly period.</returns>
        public static decimal CalculateFundingRate(IList<FundingRateStats> fundingRates, PerpMarket perpMarket,
            byte baseDecimals, byte quoteDecimals)
        {
            var oldestStat = fundingRates[^1];
            var latestStat = fundingRates[0];

            if (perpMarket == null) return 0m;

            var startFunding = (oldestStat.LongFunding + oldestStat.ShortFunding) / 2; 
            var endFunding = (latestStat.LongFunding + latestStat.ShortFunding) / 2;
            var fundingDiff = endFunding - startFunding;

            var avgPrice = (latestStat.BaseOraclePrice + oldestStat.BaseOraclePrice) / 2;

            var fundingInQuoteDecimals = fundingDiff / (decimal) Math.Pow(10, quoteDecimals);
            var basePriceInBaseLots = avgPrice * perpMarket.BaseLotsToNumber(1m, baseDecimals);

            return (fundingInQuoteDecimals / basePriceInBaseLots) * 100;
        }

        /// <summary>
        /// Calculates the funding rate from the historical long and short funding of a given market.
        /// </summary>
        /// <param name="fundingRates">The historical funding rates data.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <returns>The fundin rate per hourly period.</returns>
        public static decimal CalculateFundingRate(IList<FundingRateStats> fundingRates, PerpMarket perpMarket)
        {
            if (perpMarket == null) return 0m;
            if (perpMarket.Config == null) return 0m;

            return CalculateFundingRate(fundingRates, perpMarket, perpMarket.Config.BaseDecimals, perpMarket.Config.QuoteDecimals);
        }

        /// <summary>
        /// Calculates the funding rate from the indexed long and short funding data.
        /// </summary>
        /// <param name="oldestLongFunding">The oldest long funding.</param>
        /// <param name="oldestShortFunding">The oldest short funding.</param>
        /// <param name="latestLongFunding">the latest long funding.</param>
        /// <param name="latestShortFunding">The latest short funding.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <param name="oraclePrice">The oracle price.</param>
        /// <returns></returns>
        public static decimal CalculateFundingRate(decimal oldestLongFunding, decimal oldestShortFunding,
            decimal latestLongFunding, decimal latestShortFunding, decimal oraclePrice, PerpMarket perpMarket)
        {
            if (perpMarket == null) return 0m;
            if (perpMarket.Config == null) return 0m;

            return CalculateFundingRate(oldestLongFunding, oldestShortFunding, latestLongFunding, latestShortFunding,
                oraclePrice, perpMarket, perpMarket.Config.BaseDecimals, perpMarket.Config.QuoteDecimals);
        }
    }
}
