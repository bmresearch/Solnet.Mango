using Solnet.Mango.Historical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical
{
    /// <summary>
    /// Specifies functionality for the <see cref="MangoHistoricalDataService"/>.
    /// </summary>
    public interface IMangoHistoricalDataService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<MarginLendingStats>> GetSpotStatsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<MarginLendingStats> GetSpotStats();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<PerpStats>> GetPerpStatsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<PerpStats> GetPerpStats();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marketAddress"></param>
        /// <returns></returns>
        Task<Response<IList<TradeInfo>>> GetRecentTradesAsync(string marketAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marketAddress"></param>
        /// <returns></returns>
        Response<IList<TradeInfo>> GetRecentTrades(string marketAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marketAddress"></param>
        /// <returns></returns>
        Task<Response<VolumeInfo>> GetVolumeAsync(string marketAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marketAddress"></param>
        /// <returns></returns>
        Response<VolumeInfo> GetVolume(string marketAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marketAddress"></param>
        /// <returns></returns>
        Task<IList<FundingRateStats>> GetHistoricalFundingRatesAsync(string marketAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marketAddress"></param>
        /// <returns></returns>
        IList<FundingRateStats> GetHistoricalFundingRates(string marketAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<TvProviderConfig> GetProviderConfigAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TvProviderConfig GetProviderConfig();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<TvSymbol> GetSymbolAsync(string symbol);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TvSymbol GetSymbol(string symbol);

        /// <summary>
        /// Get historical data for specified time range. This is an asynchronous operation.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="symbol"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        Task<TvHistory> GetHistoryForSymbolAsync(DateTime from, DateTime to, string symbol, string resolution);

        /// <summary>
        /// Get historical data for specified time range.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="symbol"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        TvHistory GetHistoryForSymbol(DateTime from, DateTime to, string symbol, string resolution);

    }
}
