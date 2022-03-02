using BlockMountain.TradingView;
using BlockMountain.TradingView.Models;
using Microsoft.Extensions.Logging;
using Solnet.Mango.Historical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical
{
    /// <summary>
    /// Implements a service for mango historical data.
    /// </summary>
    public class MangoHistoricalDataService : IMangoHistoricalDataService
    {
        /// <summary>
        /// The base url for the mango stats endpoints.
        /// </summary>
        private static readonly string MangoStatsBaseUrl = "https://mango-stats-v3.herokuapp.com/";

        /// <summary>
        /// The base url for the event history endpoints.
        /// </summary>
        private static readonly string EventHistoryBaseUrl = "https://event-history-api.herokuapp.com/";

        /// <summary>
        /// The base url for the candlestick data endpoints.
        /// </summary>
        private static readonly string EventHistoryApiCandlesBaseUrl = "https://event-history-api-candles.herokuapp.com/";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The config.
        /// </summary>
        private MangoHistoricalDataServiceConfig _config;

        /// <summary>
        /// The http client.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// The json serializer options.
        /// </summary>
        private JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// The trading view data provider.
        /// </summary>
        private ITradingViewProvider _tradingViewProvider;

        /// <summary>
        /// Initialize the mango historical data service.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="logger">A logger instance.</param>
        /// <param name="httpClient">An http client.</param>
        /// <param name="tradingViewProvider">A trading view provider.</param>
        public MangoHistoricalDataService(MangoHistoricalDataServiceConfig config, ILogger logger = null,
            HttpClient httpClient = default, ITradingViewProvider tradingViewProvider = null)
        {
            _logger = logger;
            _config = config;
            _httpClient = httpClient ?? new HttpClient();
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            _tradingViewProvider = tradingViewProvider ?? TradingViewProviderFactory.GetProvider(new TradingViewProviderConfig 
            { 
                BaseUrl = EventHistoryApiCandlesBaseUrl,
            }, logger);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetHistoricalFundingRates(string)"/>
        public IList<FundingRateStats> GetHistoricalFundingRates(string marketAddress) 
            => GetHistoricalFundingRatesAsync(marketAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetHistoricalFundingRatesAsync(string)"/>
        public async Task<IList<FundingRateStats>> GetHistoricalFundingRatesAsync(string marketAddress)
        {
            if (string.IsNullOrEmpty(marketAddress))
                throw new ArgumentNullException(nameof(marketAddress));

            var url = _config.MangoStatsBaseUrl != null ?
                _config.MangoStatsBaseUrl :
                MangoStatsBaseUrl + 
                $"perp/funding_rate?mangoGroup={_config.MangoGroup}" +
                $"&market={marketAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<List<FundingRateStats>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetPerpStats"/>
        public IList<PerpStats> GetPerpStats() => GetPerpStatsAsync().Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetPerpStatsAsync"/>
        public async Task<IList<PerpStats>> GetPerpStatsAsync()
        {
            var url = _config.MangoStatsBaseUrl != null ? 
                _config.MangoStatsBaseUrl : 
                MangoStatsBaseUrl + 
                $"perp?mangoGroup={_config.MangoGroup}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<List<PerpStats>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetRecentTrades(string)"/>
        public Response<IList<TradeInfo>> GetRecentTrades(string marketAddress)
            => GetRecentTradesAsync(marketAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetRecentTradesAsync(string)"/>
        public async Task<Response<IList<TradeInfo>>> GetRecentTradesAsync(string marketAddress)
        {
            var url = _config.EventHistoryCandlesBaseUrl != null ?
                _config.EventHistoryCandlesBaseUrl :
                EventHistoryApiCandlesBaseUrl + 
                $"trades/address/{marketAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<Response<IList<TradeInfo>>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetMarginLendingStats"/>
        public IList<MarginLendingStats> GetMarginLendingStats() => GetMarginLendingStatsAsync().Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetMarginLendingStatsAsync"/>
        public async Task<IList<MarginLendingStats>> GetMarginLendingStatsAsync()
        {
            var url = _config.MangoStatsBaseUrl != null ?
                _config.MangoStatsBaseUrl :
                MangoStatsBaseUrl + 
                $"spot?mangoGroup={_config.MangoGroup}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<List<MarginLendingStats>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetVolume(string)"/>
        public Response<VolumeInfo> GetVolume(string marketAddress) => GetVolumeAsync(marketAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetVolumeAsync(string)"/>
        public async Task<Response<VolumeInfo>> GetVolumeAsync(string marketAddress)
        {
            var url = _config.EventHistoryBaseUrl != null ?
                _config.EventHistoryBaseUrl :
                EventHistoryBaseUrl + 
                $"stats/perps/{marketAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<Response<VolumeInfo>>(res);
        }        
        
        /// <inheritdoc cref="IMangoHistoricalDataService.GetPerpTrades(string)"/>
        public Response<List<PerpTrade>> GetPerpTrades(string mangoAccountAddress) => GetPerpTradesAsync(mangoAccountAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetPerpTradesAsync(string)"/>
        public async Task<Response<List<PerpTrade>>> GetPerpTradesAsync(string mangoAccountAddress)
        {
            var url = _config.EventHistoryBaseUrl != null ?
                _config.EventHistoryBaseUrl :
                EventHistoryBaseUrl + 
                $"perp_trades/{mangoAccountAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            var parsed = await HandleResponse<Response<List<PerpTrade>>>(res);

            parsed.Data = parsed.Data.Where(x => x != null).ToList();

            return parsed;
        }
        
        /// <inheritdoc cref="IMangoHistoricalDataService.GetOpenOrders(string)"/>
        public Response<List<SpotOpenOrder>> GetOpenOrders(string openOrdersAccountAddress) => GetOpenOrdersAsync(openOrdersAccountAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetOpenOrdersAsync(string)"/>
        public async Task<Response<List<SpotOpenOrder>>> GetOpenOrdersAsync(string openOrdersAccountAddress)
        {
            var url = _config.EventHistoryBaseUrl != null ?
                _config.EventHistoryBaseUrl :
                EventHistoryBaseUrl + 
                $"trades/open_orders/{openOrdersAccountAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<Response<List<SpotOpenOrder>>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetHistoryAsync(DateTime, DateTime, string, string)"/>
        public async Task<TvBarResponse> GetHistoryAsync(DateTime from, DateTime to, string symbol, string resolution)
            => await _tradingViewProvider.GetHistoryAsync(from, to, symbol, resolution);

        /// <inheritdoc cref="IMangoHistoricalDataService.GetHistory(DateTime, DateTime, string, string)"/>
        public TvBarResponse GetHistory(DateTime from, DateTime to, string symbol, string resolution)
            => GetHistoryAsync(from, to, symbol, resolution).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetConfigurationAsync"/>
        public async Task<TvConfiguration> GetConfigurationAsync()
            => await _tradingViewProvider.GetConfigurationAsync();

        /// <inheritdoc cref="IMangoHistoricalDataService.GetConfiguration"/>
        public TvConfiguration GetConfiguration()
            => GetConfigurationAsync().Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetSymbolAsync(string)"/>
        public async Task<TvSymbolInfo> GetSymbolAsync(string symbol)
            => await _tradingViewProvider.GetSymbolAsync(symbol);

        /// <inheritdoc cref="IMangoHistoricalDataService.GetSymbol(string)"/>
        public TvSymbolInfo GetSymbol(string symbol)
            => GetSymbolAsync(symbol).Result;

        /// <summary>
        /// Handle the response to the request.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>The task which returns the handled response type.</returns>
        private async Task<T> HandleResponse<T>(HttpResponseMessage message)
        {
            string data = await message.Content.ReadAsStringAsync();
            _logger?.LogInformation(new EventId(0, "REC"), $"Result: {data}");
            return JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions);
        }
    }
}
