using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Solnet.Mango.Historical;
using Solnet.Mango.Models.Events;
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

            _mangoHistoricalDataService = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig
            {
                MangoGroup = "mainnet.1",
            }, _logger);
        }

        public void Run()
        {
            var now = DateTime.Now;
            var ohlcv = _mangoHistoricalDataService.GetHistory(now.Subtract(TimeSpan.FromDays(30)), now, "SOL-PERP", "1D");

            //var spotStats = _mangoHistoricalDataService.GetMarginLendingStats();

            //var perpStats = _mangoHistoricalDataService.GetPerpStats();

            //var fundingRate = _mangoHistoricalDataService.GetHistoricalFundingRates("SOL-PERP");

            //var volumeInfo = _mangoHistoricalDataService.GetVolume("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            //var recentTrades = _mangoHistoricalDataService.GetRecentTrades("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            //var recentPerpTrades = _mangoHistoricalDataService.GetPerpTrades("CGcrpkxyx92vjyQApsr1jTN6M5PeERKSEaH1zskzccRG");

            //var openOrders = _mangoHistoricalDataService.GetOpenOrders("DBZUDrcXEPNdLaNJZ973w1joCnsa1k4a8hUFVvgCuzGf");

            _mangoHistoricalDataService.ConnectionStateChanged += _mangoHistoricalDataService_ConnectionStateChanged;

            _mangoHistoricalDataService.SubscribeFillsAsync(
                (snapshot) =>
            {
                foreach (var evt in snapshot.DecodedEvents)
                {
                    Console.WriteLine($"{snapshot.Market} - {evt.Price} {evt.Quantity}");
                }
            }, (evt) =>
            {
                Console.WriteLine($"{evt.Market} - {evt.DecodedEvent.Price} {evt.DecodedEvent.Quantity}");
            });


            Task.Delay(60_000).Wait();

            _mangoHistoricalDataService.UnsubscribeFills();

            Console.ReadLine();
        }

        private void _mangoHistoricalDataService_ConnectionStateChanged(object sender, System.Net.WebSockets.WebSocketState e)
        {
            _logger?.LogDebug(new EventId(), $"Web Socket State changed to {e}");
        }
    }
}
