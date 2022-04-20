using BlockMountain.TradingView;
using BlockMountain.TradingView.Models;
using Solnet.Mango.Historical.Models;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical
{
    /// <summary>
    /// Specifies functionality for the <see cref="MangoHistoricalDataService"/>.
    /// </summary>
    public interface IMangoHistoricalDataService
    {
        /// <summary>
        /// The web socket connection state.
        /// </summary>
        public WebSocketState State { get; }

        /// <summary>
        /// Subscribe to the feed of fill events.
        /// </summary>
        /// <param name="snapshotAction">An action that is called to receive the fills snapshot upon connection.</param>
        /// <param name="eventAction">An action that is called whenever there is a new fill event.</param>
        void SubscribeFills(Action<FillsSnapshot> snapshotAction, Action<FillsEvent> eventAction);

        /// <summary>
        /// Subscribe to the feed of fill events. This is an asynchronous operation.
        /// </summary>
        /// <param name="snapshotAction">An action that is called to receive the fills snapshot upon connection.</param>
        /// <param name="eventAction">An action that is called whenever there is a new fill event.</param>
        /// <returns>A task which performs the action.</returns>
        Task SubscribeFillsAsync(Action<FillsSnapshot> snapshotAction, Action<FillsEvent> eventAction);

        /// <summary>
        /// Unsubscribes to the feed of fill events and disconnects from the web socket server.
        /// </summary>
        void UnsubscribeFills();

        /// <summary>
        /// Unsubscribes to the feed of fill events and disconnects from the web socket server. This is an asynchronous operation.
        /// </summary>
        /// <returns>A task which performs the action.</returns>
        Task UnsubscribeFillsAsync();

        /// <summary>
        /// An event raised whenever the web socket connection state changes.
        /// </summary>
        public event EventHandler<WebSocketState> ConnectionStateChanged;

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
        /// Gets the recent trades of a given mango account. This is an asynchronous operation.
        /// </summary>
        /// <param name="mangoAccountAddress">The mango account address.</param>
        /// <returns>A task which may return the perp trades.</returns>
        Task<Response<List<PerpTrade>>> GetPerpTradesAsync(string mangoAccountAddress);

        /// <summary>
        /// Gets the recent trades of a given mango account.
        /// </summary>
        /// <param name="mangoAccountAddress">The mango account address.</param>
        /// <returns>The perp trades.</returns>
        Response<List<PerpTrade>> GetPerpTrades(string mangoAccountAddress);

        /// <summary>
        /// Gets the open orders of a given open orders account. This is an asynchronous operation.
        /// </summary>
        /// <param name="openOrdersAccountAddress">The open orders account address.</param>
        /// <returns>A task which may return the spot open orders.</returns>
        Task<Response<List<SpotOpenOrder>>> GetOpenOrdersAsync(string openOrdersAccountAddress);

        /// <summary>
        /// Gets the open orders of a given open orders account.
        /// </summary>
        /// <param name="openOrdersAccountAddress">The open orders account address.</param>
        /// <returns>The spot open orders.</returns>
        Response<List<SpotOpenOrder>> GetOpenOrders(string openOrdersAccountAddress);

        /// <summary>
        /// Gets the historical funding rate for a given market name. This is an asynchronous operation.
        /// </summary>
        /// <param name="marketName">The market name.</param>
        /// <returns>A task which may return a list of historical funding rates.</returns>
        Task<IList<FundingRateStats>> GetHistoricalFundingRatesAsync(string marketName);

        /// <summary>
        /// Gets the historical funding rate for a given market name. 
        /// </summary>
        /// <param name="marketName">The market name.</param>
        /// <returns>A list of historical funding rates.</returns>
        IList<FundingRateStats> GetHistoricalFundingRates(string marketName);

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
