using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Solnet.Mango.Models;
using Solnet.Mango.Models.Events;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Pyth;
using Solnet.Pyth.Models;
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
        private readonly IRpcClient RpcClient;

        private readonly IStreamingRpcClient StreamingRpcClient;

        private readonly IMangoClient _mangoClient;
        private readonly IPythClient _pythClient;
        private readonly ILogger _logger;

        public SubscribeEventQueueExample()
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
            }).CreateLogger<IRpcClient>();

            StreamingRpcClient = Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);
            RpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);
            StreamingRpcClient.ConnectAsync().Wait();
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
            _pythClient = Pyth.ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public async void Run()
        {
            AccountResultWrapper<MappingAccount> mappingAccount = await
                _pythClient.GetMappingAccountAsync(Pyth.Constants.MappingAccount);

            /*  Optionally perform a single request to get the product account
            foreach (PublicKey productAccountKey in mappingAccount.ParsedResult.ProductAccountKeys)
            {
                var productAccount = pythClient.GetProductAccount(productAccountKey);
            }
            */

            MultipleAccountsResultWrapper<List<ProductAccount>> productAccounts = await
                _pythClient.GetProductAccountsAsync(mappingAccount.ParsedResult);

            var _productAccount = productAccounts.ParsedResult.First(x => x.Product.Symbol.Contains("SOL"));
            var _productAccountIndex = productAccounts.ParsedResult.IndexOf(_productAccount);

            var _priceAccount = await _pythClient.GetPriceDataAccountsAsync(productAccounts.ParsedResult);

            await Task.Delay(200);

            AccountResultWrapper<MangoGroup> mangoGroup =
                await _mangoClient.GetMangoGroupAsync(Models.Constants.MangoGroup);
            Console.WriteLine($"Type: {mangoGroup.ParsedResult.Metadata.DataType}");

            var _oracle = mangoGroup.ParsedResult.Oracles.First(x => x.Key == _productAccount.PriceAccount.Key);
            var _perpetualMarketIndex = mangoGroup.ParsedResult.Oracles.IndexOf(_oracle);

            await Task.Delay(200);

            var marketInfo = mangoGroup.ParsedResult.PerpetualMarkets[_perpetualMarketIndex];
            var perpMarket = _mangoClient.GetPerpMarket(marketInfo.Market);
            var tokenInfo = mangoGroup.ParsedResult.Tokens[_perpetualMarketIndex];
            await Task.Delay(200);

            Console.WriteLine(perpMarket.ParsedResult);

            _mangoClient.SubscribeEventQueue((_, queue, _) =>
            {
                foreach (Event evt in queue.Events.Reverse())
                {
                    switch (evt)
                    {
                        case FillEvent fill:
                            var price = perpMarket.ParsedResult.PriceLotsToNumber(new(fill.Price), tokenInfo.Decimals, mangoGroup.ParsedResult.GetQuoteTokenInfo().Decimals);
                            var size = perpMarket.ParsedResult.BaseLotsToNumber(fill.Quantity, tokenInfo.Decimals);
                            Console.WriteLine(
                                $"{DateTime.UnixEpoch.AddSeconds((long)fill.Timestamp)} - Fill - Maker: {fill.Maker} Taker: {fill.Taker} " +
                                $"Size: {size} " +
                                $"Price: {price} " +
                                $"Notional: ${size * price}");
                            break;
                        case OutEvent outEvt:
                            Console.WriteLine(
                                $"{DateTime.UnixEpoch.AddSeconds((long)outEvt.Timestamp)} - Out - Owner: {outEvt.Owner} Size: {outEvt.Quantity}");
                            break;
                        case LiquidateEvent liq:
                            var lprice = perpMarket.ParsedResult.PriceLotsToNumber(liq.Price, tokenInfo.Decimals, mangoGroup.ParsedResult.GetQuoteTokenInfo().Decimals);
                            var lsize = perpMarket.ParsedResult.BaseLotsToNumber(liq.Quantity, tokenInfo.Decimals);
                            Console.WriteLine(
                                $"{DateTime.UnixEpoch.AddSeconds((long)liq.Timestamp)} - Liquidation - Victor: {liq.Liquidator} Victim: {liq.Liquidated} Price: {lprice} Size {lsize}");
                            break;
                    }
                }
            }, perpMarket.ParsedResult.EventQueue,
                Commitment.Confirmed);

            //AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Models.Constants.MangoGroup);

            //foreach (PerpMarketInfo t in mangoGroup.ParsedResult.PerpetualMarkets.Where(t =>
            //    t.Market.Key != SystemProgram.ProgramIdKey.Key))
            //{
            //    Console.WriteLine($"Perp Market: {t.Market}\n" +
            //                      $"Maintenance\n\tAssetWeight: {t.MaintenanceAssetWeight.ToDecimal()}\n" +
            //                      $"\tLiabilityWeight: {t.MaintenanceLiabilityWeight.ToDecimal()}\n" +
            //                      $"Initialization\n\tAssetWeight: {t.InitializationAssetWeight.ToDecimal()}\n" +
            //                      $"\tLiabilityWeight: {t.InitializationLiabilityWeight.ToDecimal()}\n" +
            //                      $"Fees\n\tMaker: {t.MakerFee.ToDecimal()}\tTaker: {t.TakerFee.ToDecimal()}");

            //    AccountResultWrapper<PerpMarket> perpMarket = _mangoClient.GetPerpMarket(t.Market);
            //    Console.WriteLine($"Bids: {perpMarket.ParsedResult.Bids}\n" +
            //                      $"Asks: {perpMarket.ParsedResult.Asks}\n" +
            //                      $"EventQueue: {perpMarket.ParsedResult.EventQueue}\n" +
            //                      $"Quote Lot Size: {perpMarket.ParsedResult.QuoteLotSize}\n" +
            //                      $"Base Lot Size: {perpMarket.ParsedResult.BaseLotSize}\n" +
            //                      $"Long Funding: {perpMarket.ParsedResult.LongFunding.ToDecimal()}\n" +
            //                      $"Short Funding: {perpMarket.ParsedResult.ShortFunding.ToDecimal()}\n" +
            //                      $"Open Interest: {perpMarket.ParsedResult.OpenInterest}\n");

            //    Task.Delay(150).Wait();
            //}

            Console.ReadLine();
        }
    }
}