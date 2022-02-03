using Solnet.Mango.Models;
using Solnet.Mango.Models.Matching;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Wallet;
using System;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class GetOrderBookSideExample : IRunnableExample
    {
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);
        private static readonly IStreamingRpcClient StreamingRpcClient = Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);
        private readonly IMangoClient _mangoClient;

        public GetOrderBookSideExample()
        {
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup);

            foreach (PerpMarketInfo t in mangoGroup.ParsedResult.PerpetualMarkets)
            {
                if (t.Market.Key == SystemProgram.ProgramIdKey.Key) continue;
                Console.WriteLine($"Perp Market: {t.Market}\n" +
                                  $"Maintenance\n\tAssetWeight: {t.MaintenanceAssetWeight.ToDecimal()}\n" +
                                  $"\tLiabilityWeight: {t.MaintenanceLiabilityWeight.ToDecimal()}\n" +
                                  $"Initialization\n\tAssetWeight: {t.InitializationAssetWeight.ToDecimal()}\n" +
                                  $"\tLiabilityWeight: {t.InitializationLiabilityWeight.ToDecimal()}\n" +
                                  $"Fees\n\tMaker: {t.MakerFee.ToDecimal()}\tTaker: {t.TakerFee.ToDecimal()}");

                AccountResultWrapper<PerpMarket> perpMarket = _mangoClient.GetPerpMarket(t.Market);
                Console.WriteLine($"Bids: {perpMarket.ParsedResult.Bids}\n" +
                                  $"Asks: {perpMarket.ParsedResult.Asks}\n" +
                                  $"EventQueue: {perpMarket.ParsedResult.EventQueue}\n" +
                                  $"Quote Lot Size: {perpMarket.ParsedResult.QuoteLotSize}\n" +
                                  $"Base Lot Size: {perpMarket.ParsedResult.BaseLotSize}\n" +
                                  $"Long Funding: {perpMarket.ParsedResult.LongFunding.ToDecimal()}\n" +
                                  $"Short Funding: {perpMarket.ParsedResult.ShortFunding.ToDecimal()}\n" +
                                  $"Open Interest: {perpMarket.ParsedResult.OpenInterest}\n");

                Task.Delay(200).Wait();

                //var bids = _mangoClient.GetOrderBookSide(perpMarket.ParsedResult.Bids);
                //var asks = _mangoClient.GetOrderBookSide(perpMarket.ParsedResult.Asks);

                MultipleAccountsResultWrapper<OrderBook> book = _mangoClient.GetOrderBook(perpMarket.ParsedResult);

                Console.ReadKey();
            }
        }
    }
}