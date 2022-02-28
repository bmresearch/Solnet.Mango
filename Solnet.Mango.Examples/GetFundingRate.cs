using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Solnet.Mango.Historical;
using Solnet.Mango.Historical.Models;
using Solnet.Mango.Models;
using Solnet.Mango.Models.Configs;
using Solnet.Rpc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Solnet.Mango.Examples
{
    public class GetFundingRate : IRunnableExample
    {
        private readonly IRpcClient _rpcClient;
        private readonly ILogger _logger;
        private readonly IMangoClient _mangoClient;
        private readonly MangoProgram _mango;
        private readonly IMangoHistoricalDataService _mangoHistoricalDataService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GetFundingRate()
        {
            _logger = LoggerFactory.Create(x =>
            {
                x.AddSimpleConsole(o =>
                {
                    o.UseUtcTimestamp = true;
                    o.IncludeScopes = true;
                    o.ColorBehavior = LoggerColorBehavior.Enabled;
                    o.TimestampFormat = "HH:mm:ss ";
                })
                .SetMinimumLevel(LogLevel.Debug);
            }).CreateLogger<IMangoHistoricalDataService>();

            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            _mangoHistoricalDataService = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig
            {
                MangoGroup = "mainnet.1",
            }, _logger);

            // the programs
            _mango = MangoProgram.CreateMainNet();

            // the clients
            _rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet, _logger);
            _mangoClient = ClientFactory.GetClient(_rpcClient, logger: _logger, programId: _mango.ProgramIdKey);
        }

        public void Run()
        {
            var mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup).ParsedResult;
            mangoGroup.LoadPerpMarkets(_mangoClient, _logger);

            var fundingRates = new Dictionary<string, decimal>();

            foreach (var market in mangoGroup.Config.PerpMarkets)
            {
                if (fundingRates.Keys.Contains(market.Name)) break;

                var fundingStats = _mangoHistoricalDataService.GetHistoricalFundingRates(market.Name);

                var funding = PerpStats.CalculateFundingRate(
                    fundingStats,
                    mangoGroup.PerpMarketAccounts[market.MarketIndex]
                    );

                fundingRates.Add(market.Name, funding);
            }

            foreach(var item in fundingRates)
            {
                Console.WriteLine($"Market: {item.Key,10} Funding Rate: {item.Value,8:N4}% 1H ({item.Value * 24 * 365,8:N2}% APR)");
            }

            Console.ReadLine();
        }
    }
}
