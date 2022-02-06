using Microsoft.Extensions.Logging;
using Solnet.Mango.Historical.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical
{
    /// <summary>
    /// 
    /// </summary>
    public class MangoHistoricalDataService : IMangoHistoricalDataService
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly string MangoStatsBaseUrl = "https://mango-stats-v3.herokuapp.com";

        /// <summary>
        /// 
        /// </summary>
        private static readonly string EventHistoryBaseUrl = "https://event-history-api.herokuapp.com";

        /// <summary>
        /// 
        /// </summary>
        private static readonly string EventHistoryApiCandlesBaseUrl = "https://event-history-api-candles.herokuapp.com";

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        private MangoHistoricalDataServiceConfig _config;

        /// <summary>
        /// 
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        private JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public MangoHistoricalDataService(MangoHistoricalDataServiceConfig config, ILogger logger)
        {
            _logger = logger;
            _config = config;
            _httpClient = new HttpClient();
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
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
                MangoStatsBaseUrl + $"/perp/funding_rate?mangoGroup={_config.MangoGroup}&market={marketAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<List<FundingRateStats>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetHistoryForSymbol(DateTime, DateTime, string, string)"/>
        public TvHistory GetHistoryForSymbol(DateTime from, DateTime to, string symbol, string resolution)
            => GetHistoryForSymbolAsync(from, to, symbol, resolution).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetHistoryForSymbolAsync(DateTime, DateTime, string, string)"/>
        public async Task<TvHistory> GetHistoryForSymbolAsync(DateTime from, DateTime to, string symbol, string resolution)
        {
            var url = _config.EventHistoryCandlesBaseUrl != null ?
                _config.EventHistoryCandlesBaseUrl :
                EventHistoryApiCandlesBaseUrl +
                $"/tv/history?symbol={symbol}" +
                $"&resolution={resolution}" +
                $"&from={(from - DateTime.UnixEpoch).TotalSeconds}" +
                $"&to={(to - DateTime.UnixEpoch).TotalSeconds}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            var tvHistory = await HandleResponse<TvHistory>(res);

            if (_config.ParseOhlcv)
            {
                if (tvHistory != null && tvHistory.ErrorMessage == null
                                   && tvHistory.High.Length == tvHistory.Low.Length
                                   && tvHistory.High.Length == tvHistory.Close.Length
                                   && tvHistory.High.Length == tvHistory.Open.Length
                                   && tvHistory.High.Length == tvHistory.Volume.Length)
                {
                    var resultOhlcvs = new OHLCV[tvHistory.High.Length];
                    for (int i = 0; i < tvHistory.High.Length; i++)
                    {
                        resultOhlcvs[i] = new OHLCV()
                        {
                            High = tvHistory.High[i],
                            Close = tvHistory.Close[i],
                            Open = tvHistory.Open[i],
                            Low = tvHistory.Low[i],
                            Volume = tvHistory.Volume[i],
                            Timestamp = DateTime.UnixEpoch.AddSeconds(tvHistory.Timestamp[i])
                        };
                    }
                    tvHistory.ParsedOHLCVs = resultOhlcvs;
                }
            }

            return tvHistory;
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetPerpStats"/>
        public IList<PerpStats> GetPerpStats() => GetPerpStatsAsync().Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetPerpStatsAsync"/>
        public async Task<IList<PerpStats>> GetPerpStatsAsync()
        {
            var url = _config.MangoStatsBaseUrl != null ? 
                _config.MangoStatsBaseUrl : 
                MangoStatsBaseUrl + $"/perp?mangoGroup={_config.MangoGroup}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<List<PerpStats>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetProviderConfig"/>
        public TvProviderConfig GetProviderConfig() => GetProviderConfigAsync().Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetProviderConfigAsync"/>
        public async Task<TvProviderConfig> GetProviderConfigAsync()
        {
            var url = _config.EventHistoryCandlesBaseUrl != null ?
                _config.EventHistoryCandlesBaseUrl :
                EventHistoryApiCandlesBaseUrl + $"/tv/config";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<TvProviderConfig>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetRecentTrades(string)"/>
        public Response<IList<TradeInfo>> GetRecentTrades(string marketAddress)
            => GetRecentTradesAsync(marketAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetRecentTradesAsync(string)"/>
        public async Task<Response<IList<TradeInfo>>> GetRecentTradesAsync(string marketAddress)
        {
            var url = _config.EventHistoryCandlesBaseUrl != null ?
                _config.EventHistoryCandlesBaseUrl :
                EventHistoryApiCandlesBaseUrl + $"/trades/address/{marketAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<Response<IList<TradeInfo>>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetSpotStats"/>
        public IList<SpotStats> GetSpotStats() => GetSpotStatsAsync().Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetSpotStatsAsync"/>
        public async Task<IList<SpotStats>> GetSpotStatsAsync()
        {
            var url = _config.MangoStatsBaseUrl != null ?
                _config.MangoStatsBaseUrl :
                MangoStatsBaseUrl + $"/spot?mangoGroup={_config.MangoGroup}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<List<SpotStats>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetSymbol"/>
        public TvSymbol GetSymbol(string symbol) => GetSymbolAsync(symbol).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetSymbolAsync"/>
        public async Task<TvSymbol> GetSymbolAsync(string symbol)
        {
            var url = _config.EventHistoryCandlesBaseUrl != null ?
                _config.EventHistoryCandlesBaseUrl :
                EventHistoryApiCandlesBaseUrl + $"/tv/symbols?symbol={symbol}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<TvSymbol> (res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetVolume(string)"/>
        public Response<VolumeInfo> GetVolume(string marketAddress) => GetVolumeAsync(marketAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetVolumeAsync(string)"/>
        public async Task<Response<VolumeInfo>> GetVolumeAsync(string marketAddress)
        {
            var url = _config.EventHistoryBaseUrl != null ?
                _config.EventHistoryBaseUrl :
                EventHistoryBaseUrl + $"/stats/perps/{marketAddress}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<Response<VolumeInfo>>(res);
        }

        /// <summary>
        /// Handle the response to the request.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>The task which returns the <see cref="RequestResult{T}"/>.</returns>
        private async Task<T> HandleResponse<T>(HttpResponseMessage message)
        {
            string data = await message.Content.ReadAsStringAsync();
            _logger?.LogInformation(new EventId(0, "REC"), $"Result: {data}");
            return JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions);
        }
    }
}
