using Solnet.Mango.Models;
using Solnet.Mango.Models.Events;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class SubscribeEventQueueExample : IRunnableExample
    {
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);

        private readonly IMangoClient _mangoClient;

        public SubscribeEventQueueExample()
        {
            StreamingRpcClient.ConnectAsync().Wait();
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup);

            foreach (PerpMarketInfo t in mangoGroup.ParsedResult.PerpetualMarkets.Where(t =>
                t.Market.Key != SystemProgram.ProgramIdKey.Key))
            {
                Console.WriteLine($"Perp Market: {t.Market}\n" +
                                  $"Maintenance\n\tAssetWeight: {t.MaintenanceAssetWeight.Value}\n" +
                                  $"\tLiabilityWeight: {t.MaintenanceLiabilityWeight.Value}\n" +
                                  $"Initialization\n\tAssetWeight: {t.InitializationAssetWeight.Value}\n" +
                                  $"\tLiabilityWeight: {t.InitializationLiabilityWeight.Value}\n" +
                                  $"Fees\n\tMaker: {t.MakerFee.Value}\tTaker: {t.TakerFee.Value}");

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

                _mangoClient.SubscribeEventQueue((_, queue, _) =>
                    {
                        foreach (Event evt in queue.Events)
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
                                    Console.WriteLine(
                                        $"{DateTime.UnixEpoch.AddSeconds((long)liq.Timestamp)} - Liquidation -");
                                    break;
                            }
                        }
                    }, perpMarket.ParsedResult.EventQueue,
                    Commitment.Confirmed);
            }

            Console.ReadLine();
        }
    }
}