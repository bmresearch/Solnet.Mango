using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Solnet.Mango.Historical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class HistoricalDataExample : IRunnableExample
    {
        private IMangoHistoricalDataService _mangoHistoricalDataService;
        private ILogger _logger;

        public HistoricalDataExample()
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

            _mangoHistoricalDataService = new MangoHistoricalDataService( new MangoHistoricalDataServiceConfig 
            {
                MangoGroup = "mainnet.1",
            }, _logger);
        }

        public void Run()
        {
            var spotStats = _mangoHistoricalDataService.GetMarginLendingStats();
            var perpStats = _mangoHistoricalDataService.GetPerpStats();

            var fundingRate = _mangoHistoricalDataService.GetHistoricalFundingRates("SOL-PERP");

            var volumeInfo = _mangoHistoricalDataService.GetVolume("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            var recentTrades = _mangoHistoricalDataService.GetRecentTrades("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            var recentPerpTrades = _mangoHistoricalDataService.GetPerpTrades("CGcrpkxyx92vjyQApsr1jTN6M5PeERKSEaH1zskzccRG");

            var openOrders = _mangoHistoricalDataService.GetOpenOrders("DBZUDrcXEPNdLaNJZ973w1joCnsa1k4a8hUFVvgCuzGf");

            Console.ReadLine();
        }
    }
}
