using Solnet.Mango.Models;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class GetPerpOpenOrdersExample : IRunnableExample
    {
        private static readonly PublicKey Owner = new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient("https://solana-api.projectserum.com");

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient("wss://solana-api.projectserum.com");

        private readonly IMangoClient _mangoClient;

        public GetPerpOpenOrdersExample()
        {
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup);
            List<OrderBook> orderBooks = new();
            foreach (PerpMarketInfo t in mangoGroup.ParsedResult.PerpetualMarkets)
            {
                if (t.Market == SystemProgram.ProgramIdKey.Key)
                {
                    orderBooks.Add(null);
                    continue;
                }
                AccountResultWrapper<PerpMarket> perpMarket = _mangoClient.GetPerpMarket(t.Market);
                orderBooks.Add(_mangoClient.GetOrderBook(perpMarket.ParsedResult).ParsedResult);
            }

            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = _mangoClient.GetMangoAccounts(Owner);
            for (int i = 0; i < mangoAccounts.ParsedResult.Count; i++)
            {
                Console.WriteLine(
                    $"Account: {mangoAccounts.OriginalRequest.Result[i].PublicKey} Owner: {mangoAccounts.ParsedResult[i].Owner}");

                var orders = mangoAccounts.ParsedResult[i].GetOrders();
                Console.WriteLine($"Open Orders: {orders.Count}");

                foreach (var order in orders)
                {
                    var orderBook = orderBooks[order.MarketIndex];
                    var bids = orderBook.Bids.GetOrders();
                    var asks = orderBook.Asks.GetOrders();
                    var orderBookOrder = order.Side == Serum.Models.Side.Buy ?
                        bids.FirstOrDefault(x => x.OrderId == order.OrderId) : asks.FirstOrDefault(x => x.OrderId == order.OrderId);
                    Console.WriteLine($"Order {order.OrderId} - {order.Side} - Size: {orderBookOrder.RawQuantity} - Price: {orderBookOrder.RawQuantity} - Market: {order.MarketIndex} - Client Id: {order.ClientOrderId}");
                }
            }


            Console.ReadLine();
        }
    }
}