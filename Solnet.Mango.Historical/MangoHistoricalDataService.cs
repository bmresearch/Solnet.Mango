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
        private static readonly string SerumHistoryBaseUrl = "https://serum-history.herokuapp.com";

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

            var url = MangoStatsBaseUrl + $"/perp/funding_rate?mangoGroup={_config.MangoGroup}&market={marketAddress}";
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
            var url = MangoStatsBaseUrl + $"/perp?mangoGroup={_config.MangoGroup}";
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
            var url = EventHistoryApiCandlesBaseUrl + $"/trades/address/{marketAddress}";
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
            var url = MangoStatsBaseUrl + $"/spot?mangoGroup={_config.MangoGroup}";
            HttpResponseMessage res = await _httpClient.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                return null;
            }

            return await HandleResponse<List<SpotStats>>(res);
        }

        /// <inheritdoc cref="IMangoHistoricalDataService.GetVolume(string)"/>
        public Response<VolumeInfo> GetVolume(string marketAddress) => GetVolumeAsync(marketAddress).Result;

        /// <inheritdoc cref="IMangoHistoricalDataService.GetVolumeAsync(string)"/>
        public async Task<Response<VolumeInfo>> GetVolumeAsync(string marketAddress)
        {
            var url = EventHistoryBaseUrl + $"/stats/perps/{marketAddress}";
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
