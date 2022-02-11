using Solnet.Mango.Models;
using Solnet.Mango.Models.Matching;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs.Models;
using Solnet.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class SubscribeOrderBook : IRunnableExample
    {
        private readonly IMangoClient _mangoClient;
        private static Dictionary<string, string> Markets = new Dictionary<string, string>()
        {
            { "SOL-PERP", "CqxX2QupYiYafBSbA519j4vRVxxecidbh2zwX66Lmqem" },
            { "BTC-PERP", "4nfmQP3KmUqEJ6qJLsS3offKgE96YUB4Rp7UQvm2Fbi9" },
            { "MNGO-PERP", "2TgaaVoHgnSeEtXvWTx13zQeTf4hYWAMEiMQdcG6EwHi" }
        };

        private Dictionary<string, List<OpenOrder>> allAskOrders;
        private Dictionary<string, List<OpenOrder>> allBidOrders;

        private MangoGroup _mangoGroup;

        public SubscribeOrderBook()
        {
            var rpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);
            var streamingRpcClient = Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);
            _mangoClient = ClientFactory.GetClient(rpcClient, streamingRpcClient);
            _mangoClient.ConnectAsync().Wait();
            _mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup).ParsedResult;
            allAskOrders = new Dictionary<string, List<OpenOrder>>();
            allBidOrders = new Dictionary<string, List<OpenOrder>>();
            Console.WriteLine($"Initializing {ToString()}");
        }

        public void Run()
        {
            foreach ((string key, string value) in Markets)
            {
                SubscribeTo(value, key);
            }

            Console.ReadKey();
        }

        public Task SubscribeTo(string address, string name)
        {
            return Task.Run(() =>
            {
                var marketIndex = _mangoGroup.GetPerpMarketIndex(new(address));
                var token = _mangoGroup.Tokens[marketIndex];
                var quoteToken = _mangoGroup.GetQuoteTokenInfo();
                PerpMarket market = _mangoClient.GetPerpMarket(address).ParsedResult;

                Subscription subBids = _mangoClient.SubscribeOrderBookSide((subWrapper, orderBook, _) =>
                {
                    Console.WriteLine($"{name} BidOrderBook Update:: SlabNodes: {orderBook.Nodes.Count}\n");
                    var bidOrders = orderBook.GetOrders();
                    bidOrders.Sort(Comparer<OpenOrder>.Create((order, order1) => order1.RawPrice.CompareTo(order.RawPrice)));

                    allBidOrders.TryAdd(name, bidOrders);
                    bool exists = allAskOrders.TryGetValue(name, out List<OpenOrder> askOrders);
                    Console.Clear();
                    Console.WriteLine($"{name} Market:: {address}");
                    Console.WriteLine($"----------ASKS-----------");
                    if (exists)
                    {
                        for (int i = 9; i >= 0; i--)
                        {
                            Console.WriteLine($"{name} Ask:\t{market.PriceLotsToNumber(new(askOrders[i].RawPrice), token.Decimals, quoteToken.Decimals)}" +
                                $"\tSize:\t{market.BaseLotsToNumber(askOrders[i].RawQuantity, token.Decimals)}");
                        }
                    }
                    Console.WriteLine($"-----------BIDS----------");
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine(
                                $"{name} Bid:\t{market.PriceLotsToNumber(new(bidOrders[i].RawPrice), token.Decimals, quoteToken.Decimals)}\t" +
                                $"Size:\t{market.BaseLotsToNumber(bidOrders[i].RawQuantity, token.Decimals)}");
                    }
                    Console.WriteLine($"-------------------------\n");
                    var bestBid = orderBook.GetBest();
                    Console.Write($"Best Bid: P {bestBid.RawPrice} S {bestBid.RawQuantity}");

                }, market.Bids);

                Subscription subAsks = _mangoClient.SubscribeOrderBookSide((subWrapper, orderBook, _) =>
                {
                    Console.WriteLine($"{name} AskOrderBook Update:: SlabNodes: {orderBook.Nodes.Count}\n");
                    var askOrders = orderBook.GetOrders();
                    askOrders.Sort(Comparer<OpenOrder>.Create((order, order1) => order.RawPrice.CompareTo(order1.RawPrice)));

                    allAskOrders.TryAdd(name, askOrders);
                    bool exists = allAskOrders.TryGetValue(name, out List<OpenOrder> bidOrders);
                    Console.Clear();
                    Console.WriteLine($"{name} Market:: {address}");
                    Console.WriteLine($"----------ASKS-----------");
                    for (int i = 9; i >= 0; i--)
                    {
                        Console.WriteLine($"{name} Ask:\t{market.PriceLotsToNumber(new(askOrders[i].RawPrice), token.Decimals, quoteToken.Decimals)}" +
                            $"\tSize:\t{market.BaseLotsToNumber(askOrders[i].RawQuantity, token.Decimals)}");
                    }
                    Console.WriteLine($"-----------BIDS----------");
                    if (exists)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Console.WriteLine(
                                $"{name} Bid:\t{market.PriceLotsToNumber(new(bidOrders[i].RawPrice), token.Decimals, quoteToken.Decimals)}\t" +
                                $"Size:\t{market.BaseLotsToNumber(bidOrders[i].RawQuantity, token.Decimals)}");
                        }
                    }
                    Console.WriteLine($"-------------------------\n");
                    var bestAsk = orderBook.GetBest();
                    Console.Write($"Best Ask: P {bestAsk.RawPrice} S {bestAsk.RawQuantity}");

                }, market.Asks);
            });
        }
    }
}
