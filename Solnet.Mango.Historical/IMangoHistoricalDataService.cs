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
        Task<IList<SpotStats>> GetSpotStatsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<SpotStats> GetSpotStats();

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


    }
}
