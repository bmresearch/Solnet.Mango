using BlockMountain.TradingView;
using BlockMountain.TradingView.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Test
{
    [TestClass]
    public class MangoHistoricalDataServiceTest
    {
        private const string MangoStatsBaseUrl = "https://mango-stats-v3.herokuapp.com/";
        private const string EventHistoryApiCandlesBaseUrl = "https://event-history-api-candles.herokuapp.com/";
        private const string EventHistoryBaseUrl = "https://event-history-api.herokuapp.com/";

        /// <summary>
        /// Finish the test by asserting the http request went as expected.
        /// </summary>
        /// <param name="mockHandler">The mocked http message handler.</param>
        private static void FinishHttpClientTest(Mock<HttpMessageHandler> mockHandler, string url)
        {
            mockHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && req.RequestUri.ToString() == url
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        /// <summary>
        /// Setup the test with the request and response data.
        /// </summary>
        /// <param name="responseContent">The response content.</param>
        private static Mock<HttpMessageHandler> SetupHttpClientTest(string responseContent, string url)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        message => message.Method == HttpMethod.Get && message.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();
            return messageHandlerMock;
        }

        /// <summary>
        /// Setup the test with the request and response data.
        /// </summary>
        private static Mock<HttpMessageHandler> SetupHttpClientUnsuccessfulTest()
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        message => message.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                })
                .Verifiable();
            return messageHandlerMock;
        }


        private static Mock<ITradingViewProvider> SetupTvGetHistoryTest(string responseContent, DateTime d1, DateTime d2, string symbol, string res)
        {
            var mock = new Mock<ITradingViewProvider>(MockBehavior.Strict);
            mock.Setup(x => x.GetHistoryAsync(d1, d2, symbol, res))
                .ReturnsAsync(() =>
                {
                    var res = JsonSerializer.Deserialize<TvBarResponse>(responseContent);
                    return res;
                })
                .Verifiable();

            return mock;
        }

        private static Mock<ITradingViewProvider> SetupTvGetConfigurationTest(string responseContent)
        {
            var mock = new Mock<ITradingViewProvider>(MockBehavior.Strict);
            mock.Setup(x => x.GetConfigurationAsync())
                .ReturnsAsync(() =>
                {
                    var res = JsonSerializer.Deserialize<TvConfiguration>(responseContent);
                    return res;
                })
                .Verifiable();

            return mock;
        }

        private static Mock<ITradingViewProvider> SetupTvGetSymbolTest(string responseContent, string symbol)
        {
            var mock = new Mock<ITradingViewProvider>(MockBehavior.Strict);
            mock.Setup(x => x.GetSymbolAsync(symbol))
                .ReturnsAsync(() =>
                {
                    var res = JsonSerializer.Deserialize<TvSymbolInfo>(responseContent);
                    return res;
                })
                .Verifiable();

            return mock;
        }

        [TestMethod]
        public void GetMarginLendingStats()
        {
            string responseData = File.ReadAllText("Resources/MarginLendingStats.json");
            var url = MangoStatsBaseUrl + "spot?mangoGroup=mainnet.1";
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientTest(responseData, url);

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var marginLendingStats = sut.GetMarginLendingStats();

            Assert.IsNotNull(marginLendingStats);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetMarginLendingStatsUnsuccessful()
        {
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientUnsuccessfulTest();
            var url = MangoStatsBaseUrl + "spot?mangoGroup=mainnet.1";

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var marginLendingStats = sut.GetMarginLendingStats();

            Assert.IsNull(marginLendingStats);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetPerpStats()
        {
            string responseData = File.ReadAllText("Resources/PerpStats.json");
            var url = MangoStatsBaseUrl + "perp?mangoGroup=mainnet.1";
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientTest(responseData, url);

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var perpStats = sut.GetPerpStats();

            Assert.IsNotNull(perpStats);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetPerpStatsUnsuccessful()
        {
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientUnsuccessfulTest();
            var url = MangoStatsBaseUrl + "perp?mangoGroup=mainnet.1";

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var perpStats = sut.GetPerpStats();

            Assert.IsNull(perpStats);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetFundingRate()
        {
            string responseData = File.ReadAllText("Resources/FundingRates.json");
            var url = MangoStatsBaseUrl + "perp/funding_rate?mangoGroup=mainnet.1&market=SOL-PERP";
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientTest(responseData, url);

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var funding = sut.GetHistoricalFundingRates("SOL-PERP");

            Assert.IsNotNull(funding);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetFundingRateUnsuccessful()
        {
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientUnsuccessfulTest();
            var url = MangoStatsBaseUrl + "perp/funding_rate?mangoGroup=mainnet.1&market=SOL-PERP";

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var funding = sut.GetHistoricalFundingRates("SOL-PERP");

            Assert.IsNull(funding);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetVolumeInfo()
        {
            string responseData = File.ReadAllText("Resources/VolumeInfo.json");
            var url = EventHistoryBaseUrl + "stats/perps/2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi";
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientTest(responseData, url);

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetVolume("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            Assert.IsNotNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetVolumeInfoUnsuccessful()
        {
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientUnsuccessfulTest();
            var url = EventHistoryBaseUrl + "stats/perps/2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi";

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetVolume("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            Assert.IsNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetRecentTrades()
        {
            string responseData = File.ReadAllText("Resources/RecentTrades.json");
            var url = EventHistoryApiCandlesBaseUrl + "trades/address/2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi";
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientTest(responseData, url);

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetRecentTrades("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            Assert.IsNotNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetRecentTradesUnsuccessful()
        {
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientUnsuccessfulTest();
            var url = EventHistoryApiCandlesBaseUrl + "trades/address/2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi";

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetRecentTrades("2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi");

            Assert.IsNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetOpenOrders()
        {
            string responseData = File.ReadAllText("Resources/OpenOrders.json");
            var url = EventHistoryBaseUrl + "trades/open_orders/DBZUDrcXEPNdLaNJZ973w1joCnsa1k4a8hUFVvgCuzGf";
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientTest(responseData, url);

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetOpenOrders("DBZUDrcXEPNdLaNJZ973w1joCnsa1k4a8hUFVvgCuzGf");

            Assert.IsNotNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetOpenOrdersUnsuccessful()
        {
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientUnsuccessfulTest();
            var url = EventHistoryBaseUrl + "trades/open_orders/DBZUDrcXEPNdLaNJZ973w1joCnsa1k4a8hUFVvgCuzGf";

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetOpenOrders("DBZUDrcXEPNdLaNJZ973w1joCnsa1k4a8hUFVvgCuzGf");

            Assert.IsNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }
        
        [TestMethod]
        public void GetPerpTrades()
        {
            string responseData = File.ReadAllText("Resources/PerpTrades.json");
            var url = EventHistoryBaseUrl + "perp_trades/CGcrpkxyx92vjyQApsr1jTN6M5PeERKSEaH1zskzccRG";
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientTest(responseData, url);

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetPerpTrades("CGcrpkxyx92vjyQApsr1jTN6M5PeERKSEaH1zskzccRG");

            Assert.IsNotNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetPerpTradesUnsuccessful()
        {
            Mock<HttpMessageHandler> messageHandlerMock = SetupHttpClientUnsuccessfulTest();
            var url = EventHistoryBaseUrl + "perp_trades/CGcrpkxyx92vjyQApsr1jTN6M5PeERKSEaH1zskzccRG";

            HttpClient httpClient = new(messageHandlerMock.Object)
            {
                BaseAddress = new Uri(url),
            };

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, httpClient: httpClient);

            var recentTrades = sut.GetPerpTrades("CGcrpkxyx92vjyQApsr1jTN6M5PeERKSEaH1zskzccRG");

            Assert.IsNull(recentTrades);

            FinishHttpClientTest(messageHandlerMock, url);
        }

        [TestMethod]
        public void GetConfig()
        {
            string responseContent = File.ReadAllText("Resources/TvConfig.json");
            Mock<ITradingViewProvider> mock = SetupTvGetConfigurationTest(responseContent);

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, tradingViewProvider: mock.Object);

            var config = sut.GetConfiguration();

            Assert.IsNotNull(config);
            Assert.IsTrue(config.SupportSearch);
            Assert.AreEqual(11, config.SupportedResolutions.Length);
        }

        [TestMethod]
        public void GetHistory()
        {
            string responseContent = File.ReadAllText("Resources/TvHistory.json");
            Mock<ITradingViewProvider> mock = SetupTvGetHistoryTest(responseContent, 
                DateTime.UnixEpoch.AddSeconds(1644574929), 
                DateTime.UnixEpoch.AddSeconds(1644610929), 
                "SOL/USDC", 
                "60");

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, tradingViewProvider: mock.Object);

            var history = sut.GetHistory(DateTime.UnixEpoch.AddSeconds(1644574929), DateTime.UnixEpoch.AddSeconds(1644610929), "SOL/USDC", "60");

            Assert.IsNotNull(history);
            Assert.AreEqual(TvStatus.Ok, history.Status);
            Assert.AreEqual(11, history.Close.Length);
            Assert.AreEqual(11, history.High.Length);
            Assert.AreEqual(11, history.Low.Length);
            Assert.AreEqual(11, history.Open.Length);
            Assert.AreEqual(11, history.Volume.Length);
        }

        [TestMethod]
        public void GetSymbol()
        {
            string responseContent = File.ReadAllText("Resources/TvSymbol.json");
            Mock<ITradingViewProvider> mock = SetupTvGetSymbolTest(responseContent, "BTC-PERP");

            var sut = new MangoHistoricalDataService(new MangoHistoricalDataServiceConfig()
            {
                MangoGroup = "mainnet.1"
            }, tradingViewProvider: mock.Object);

            var symbolInfo = sut.GetSymbol("BTC-PERP");

            Assert.IsNotNull(symbolInfo);
            Assert.AreEqual(11, symbolInfo.SupportedResolutions.Length);
            Assert.AreEqual("BTCPERP", symbolInfo.Name);
            Assert.AreEqual("BTCPERP", symbolInfo.Ticker);
            Assert.AreEqual("BTCPERP", symbolInfo.Description);
            Assert.AreEqual("24x7", symbolInfo.Session);
            Assert.AreEqual("Etc/UTC", symbolInfo.Timezone);
            Assert.AreEqual("Spot", symbolInfo.Type);
        }
    }
}
