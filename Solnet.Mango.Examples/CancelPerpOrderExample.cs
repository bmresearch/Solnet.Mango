using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class CancelPerpOrderExample : IRunnableExample
    {
        private static readonly PublicKey Owner = new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
        private static readonly PublicKey MangoGroup = new("98pjRuQjK3qA6gXts96PqZT4Ze5QmnCmt3QYjhbUSPue");
        private static readonly PublicKey MangoCache = new("EBDRoayCDDUvDgCimta45ajQeXbexv7aKqJubruqpyvu");
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient("https://solana-api.projectserum.com");

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient("wss://solana-api.projectserum.com");

        private readonly IMangoClient _mangoClient;

        private readonly Wallet.Wallet _wallet;

        public CancelPerpOrderExample()
        {

            SolanaKeyStoreService keyStore = new();
            // get the wallet
            _wallet = keyStore.RestoreKeystoreFromFile("/path/to/keystore.json");


            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public async void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = await _mangoClient.GetMangoGroupAsync(MangoGroup);
            List<PerpMarket> perpMarkets = new();

            List<OrderBook> orderBooks = new();
            foreach (PerpMarketInfo t in mangoGroup.ParsedResult.PerpetualMarkets)
            {
                if (t.Market == SystemProgram.ProgramIdKey.Key)
                {
                    orderBooks.Add(null);
                    perpMarkets.Add(null);
                    continue;
                }
                AccountResultWrapper<PerpMarket> perpMarket = await _mangoClient.GetPerpMarketAsync(t.Market);
                perpMarkets.Add(perpMarket.ParsedResult);
                orderBooks.Add(_mangoClient.GetOrderBook(perpMarket.ParsedResult).ParsedResult);
            }

            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = await _mangoClient.GetMangoAccountsAsync(Owner);
            for (int i = 0; i < mangoAccounts.ParsedResult.Count; i++)
            {
                Console.WriteLine(
                    $"Account: {mangoAccounts.OriginalRequest.Result[i].PublicKey} Owner: {mangoAccounts.ParsedResult[i].Owner}");

                var orders = mangoAccounts.ParsedResult[i].GetOrders();
                Console.WriteLine($"Open Orders: {orders.Count}");
                if (orders.Count == 0) continue;

                RequestResult<ResponseValue<BlockHash>> blockhash = await RpcClient.GetRecentBlockHashAsync();
                TransactionBuilder txBuilder = new TransactionBuilder()
                    .SetFeePayer(Owner)
                    .SetRecentBlockHash(blockhash.Result.Value.Blockhash);

                foreach (var order in orders)
                {
                    var orderBook = orderBooks[order.MarketIndex];
                    var bids = orderBook.Bids.GetOrders();
                    var asks = orderBook.Asks.GetOrders();
                    var orderBookOrder = order.Side == Serum.Models.Side.Buy ?
                        bids.FirstOrDefault(x => x.OrderId == order.OrderId) : asks.FirstOrDefault(x => x.OrderId == order.OrderId);
                    Console.WriteLine($"Order {order.OrderId} - {order.Side} - Size: {orderBookOrder.RawQuantity} - Price: {orderBookOrder.RawQuantity} - Market: {order.MarketIndex} - Client Id: {order.ClientOrderId}");

                    txBuilder.AddInstruction(MangoProgram.CancelPerpOrder(
                        MangoGroup, 
                        new(mangoAccounts.OriginalRequest.Result[i].PublicKey),
                        Owner,
                        mangoGroup.ParsedResult.PerpetualMarkets[order.MarketIndex].Market,
                        perpMarkets[order.MarketIndex].Bids,
                        perpMarkets[order.MarketIndex].Asks,
                        order.OrderId,
                        false
                        ));
                }

                byte[] msg = txBuilder.CompileMessage();

                ExampleHelpers.DecodeAndLogMessage(msg);

                byte[] txBytes = txBuilder.Build(new List<Wallet.Account>() { _wallet.Account });

                RequestResult<string> res = await RpcClient.SendTransactionAsync(txBytes);

                Console.ReadLine();
            }


            Console.ReadLine();
        }
    }
}
