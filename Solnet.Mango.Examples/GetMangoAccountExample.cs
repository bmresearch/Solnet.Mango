using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class GetMangoAccountExample : IRunnableExample
    {
        private static readonly PublicKey Owner = new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
        private static readonly PublicKey MangoGroup = new("98pjRuQjK3qA6gXts96PqZT4Ze5QmnCmt3QYjhbUSPue");
        private static readonly PublicKey MangoCache = new("EBDRoayCDDUvDgCimta45ajQeXbexv7aKqJubruqpyvu");
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);

        private readonly IMangoClient _mangoClient;

        public GetMangoAccountExample()
        {
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(MangoGroup);

            /*
            foreach (var t in mangoGroup.ParsedResult.Tokens)
            {
                if (t.Mint.Key == SystemProgram.ProgramIdKey.Key) continue;
                Console.WriteLine($"Token: {t.Mint}\n\tRootBank: {t.RootBank}");
                var rootBank = _mangoClient.GetRootBank(t.RootBank);
                Console.WriteLine($"\t DepositIndex : {rootBank.ParsedResult.DepositIndex.Value}\n" +
                                  $"\t BorrowIndex: {rootBank.ParsedResult.BorrowIndex.Value}\n" +
                                  $"\t OptimalUtil: {rootBank.ParsedResult.OptimalUtilization.Value}\n" +
                                  $"\t OptimalRate: {rootBank.ParsedResult.OptimalRate.Value}\n" +
                                  $"\t Num Nodes: {rootBank.ParsedResult.NumNodeBanks}");
                foreach (var nodeBank in rootBank.ParsedResult.NodeBanks)
                {
                    Console.WriteLine($"\t Node Bank: {nodeBank}");
                }
            }
            */
            foreach (SpotMarketInfo t in mangoGroup.ParsedResult.SpotMarkets.Where(t =>
                t.Market.Key != SystemProgram.ProgramIdKey.Key))
            {
                Console.WriteLine($"Spot Market: {t.Market}\n" +
                                  $"Maintenance\n\tAssetWeight: {t.MaintenanceAssetWeight.Value}\n" +
                                  $"\tLiabilityWeight: {t.MaintenanceLiabilityWeight.Value}\n" +
                                  $"Initialization\n\tAssetWeight: {t.InitializationAssetWeight.Value}\n" +
                                  $"\tLiabilityWeight: {t.InitializationLiabilityWeight.Value}\n");
            }

            foreach (PerpMarketInfo t in mangoGroup.ParsedResult.PerpetualMarkets.Where(t =>
                t.Market.Key != SystemProgram.ProgramIdKey.Key))
            {
                Console.WriteLine($"Perp Market: {t.Market}\n" +
                                  $"Maintenance\n\tAssetWeight: {t.MaintenanceAssetWeight.Value}\n" +
                                  $"\tLiabilityWeight: {t.MaintenanceLiabilityWeight.Value}\n" +
                                  $"Initialization\n\tAssetWeight: {t.InitializationAssetWeight.Value}\n" +
                                  $"\tLiabilityWeight: {t.InitializationLiabilityWeight.Value}\n" +
                                  $"Fees\n\tMaker: {t.MakerFee.Value}\tTaker: {t.TakerFee.Value}");

                /*
                AccountResultWrapper<PerpMarket> perpMarket = _mangoClient.GetPerpMarket(t.Market);
                Console.WriteLine($"Bids: {perpMarket.ParsedResult.Bids}\n" +
                                  $"Asks: {perpMarket.ParsedResult.Asks}\n" +
                                  $"EventQueue: {perpMarket.ParsedResult.EventQueue}\n" +
                                  $"Quote Lot Size: {perpMarket.ParsedResult.QuoteLotSize}\n" +
                                  $"Base Lot Size: {perpMarket.ParsedResult.BaseLotSize}\n" +
                                  $"Long Funding: {perpMarket.ParsedResult.LongFunding.Value}\n" +
                                  $"Short Funding: {perpMarket.ParsedResult.ShortFunding.Value}\n" +
                                  $"Open Interest: {perpMarket.ParsedResult.OpenInterest}\n");

                Task.Delay(150).Wait();

                AccountResultWrapper<EventQueue> eventQueue =
                    _mangoClient.GetEventQueue(perpMarket.ParsedResult.EventQueue);

                Console.WriteLine($"Events: {eventQueue.ParsedResult.Events.Count}");
                foreach (Event evt in eventQueue.ParsedResult.Events)
                {
                    switch (evt)
                    {
                        case FillEvent fill:
                            Console.WriteLine(
                                $"{DateTime.UnixEpoch.AddSeconds((long)fill.Timestamp)} - Fill - Maker: {fill.Maker} Taker: {fill.Taker} Size: {fill.Quantity} Size: {fill.Price}");
                            break;
                        case OutEvent outEvt:
                            Console.WriteLine(
                                $"{DateTime.UnixEpoch.AddSeconds((long)outEvt.Timestamp)} - Out - Owner: {outEvt.Owner} Size: {outEvt.Quantity}");
                            break;
                        case LiquidateEvent liq:
                            Console.WriteLine($"{DateTime.UnixEpoch.AddSeconds((long)liq.Timestamp)} - Liquidation -");
                            break;
                    }
                }*/
            }

            AccountResultWrapper<MangoCache> mangoCache = _mangoClient.GetMangoCache(MangoCache);

            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = _mangoClient.GetMangoAccounts(Owner);
            for (int i = 0; i < mangoAccounts.ParsedResult.Count; i++)
            {
                Console.WriteLine(
                    $"Account: {mangoAccounts.OriginalRequest.Result[i].PublicKey} Owner: {mangoAccounts.ParsedResult[i].Owner}");
            }

            Console.ReadLine();
        }
    }
}