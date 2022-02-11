using BlockMountain.TradingView;
using BlockMountain.TradingView.Models;
using Solnet.Mango.Historical.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical
{
    /// <summary>
    /// Specifies functionality for the <see cref="MangoHistoricalDataService"/>.
    /// </summary>
    public interface IMangoHistoricalDataService
    {
        /// <summary>
        /// Gets the margin lending stats. This is an asynchronous operation.
        /// </summary>
        /// <returns>A task which may return list of margin lending stats.</returns>
        Task<IList<MarginLendingStats>> GetMarginLendingStatsAsync();

        /// <summary>
        /// Gets the margin lending stats.
        /// </summary>
        /// <returns>A list of margin lending stats.</returns>
        IList<MarginLendingStats> GetMarginLendingStats();

        /// <summary>
        /// Gets the perp stats. This is an asynchronous operation.
        /// </summary>
        /// <returns>A task which may return list of perp stats.</returns>
        Task<IList<PerpStats>> GetPerpStatsAsync();

        /// <summary>
        /// Gets the perp stats.
        /// </summary>
        /// <returns>A list of perp stats.</returns>
        IList<PerpStats> GetPerpStats();

        /// <summary>
        /// Gets recent trades for the given market address. This is an asynchronous operation.
        /// </summary>
        /// <param name="marketAddress">The market address.</param>
        /// <returns>A task which may return a list of recent trades.</returns>
        Task<Response<IList<TradeInfo>>> GetRecentTradesAsync(string marketAddress);

        /// <summary>
        /// Gets recent trades for the given market address.
        /// </summary>
        /// <param name="marketAddress">The market address.</param>
        /// <returns>A list of recent trades.</returns>
        Response<IList<TradeInfo>> GetRecentTrades(string marketAddress);

        /// <summary>
        /// Gets the volume for the given market address. This is an asynchronous operation.
        /// </summary>
        /// <param name="marketAddress">The market address.</param>
        /// <returns>A task which may return the volume info.</returns>
        Task<Response<VolumeInfo>> GetVolumeAsync(string marketAddress);

        /// <summary>
        /// Gets the volume for the given market address.
        /// </summary>
        /// <param name="marketAddress">The market address.</param>
        /// <returns>The volume info.</returns>
        Response<VolumeInfo> GetVolume(string marketAddress);

        /// <summary>
        /// Gets the historical funding rate for a given market address. This is an asynchronous operation.
        /// </summary>
        /// <param name="marketAddress">The market address.</param>
        /// <returns>A task which may return a list of historical funding rates.</returns>
        Task<IList<FundingRateStats>> GetHistoricalFundingRatesAsync(string marketAddress);

        /// <summary>
        /// Gets the historical funding rate for a given market address. 
        /// </summary>
        /// <param name="marketAddress">The market address.</param>
        /// <returns>A list of historical funding rates.</returns>
        IList<FundingRateStats> GetHistoricalFundingRates(string marketAddress);

        /// <inheritdoc cref="TradingViewProvider.GetHistoryAsync(DateTime, DateTime, string, string)"/>
        Task<TvBarResponse> GetHistoryAsync(DateTime from, DateTime to, string symbol, string resolution);

        /// <inheritdoc cref="TradingViewProvider.GetHistory(DateTime, DateTime, string, string)"/>
        TvBarResponse GetHistory(DateTime from, DateTime to, string symbol, string resolution);

        /// <inheritdoc cref="TradingViewProvider.GetConfigurationAsync"/>
        Task<TvConfiguration> GetConfigurationAsync();

        /// <inheritdoc cref="TradingViewProvider.GetConfiguration"/>
        TvConfiguration GetConfiguration();

        /// <inheritdoc cref="TradingViewProvider.GetSymbolAsync(string)"/>
        Task<TvSymbolInfo> GetSymbolAsync(string symbol);

        /// <inheritdoc cref="TradingViewProvider.GetSymbol(string)"/>
        TvSymbolInfo GetSymbol(string symbol);

    }
}
