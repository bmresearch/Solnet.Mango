using Solnet.Mango.Models;
using Solnet.Mango.Models.Banks;
using Solnet.Mango.Models.Caches;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Pyth;
using Solnet.Pyth.Models;
using Solnet.Rpc;
using Solnet.Rpc.Types;
using Solnet.Serum;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class SubscribeMangoCacheExample : IRunnableExample
    {
        private static readonly PublicKey Owner = new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);

        private readonly IMangoClient _mangoClient;
        private readonly IPythClient _pythClient;

        public SubscribeMangoCacheExample()
        {
            StreamingRpcClient.ConnectAsync().Wait();
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
            _pythClient = Pyth.ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            var tokenNames = new List<string>();
            AccountResultWrapper<MappingAccount> mappingAccount =
                   _pythClient.GetMappingAccount(Pyth.Constants.MappingAccount);

            MultipleAccountsResultWrapper<List<ProductAccount>> productAccounts =
                _pythClient.GetProductAccounts(mappingAccount.ParsedResult);

            MangoGroup mangoGroup = _mangoClient.GetMangoGroup(Models.Constants.MangoGroup).ParsedResult;
            MangoCache mangoCache = _mangoClient.GetMangoCache(Models.Constants.MangoCache).ParsedResult;
            mangoGroup.LoadRootBanks(_mangoClient);
            mangoGroup.LoadPerpMarkets(_mangoClient);

            foreach (var perpMarket in mangoGroup.PerpetualMarkets)
            {
                var marketIndex = mangoGroup.GetPerpMarketIndex(perpMarket.Market);
                if (perpMarket.Market.Equals(SystemProgram.ProgramIdKey))
                {
                    tokenNames.Add("UNKNOWN"); // probably switchboard ones 
                    continue;
                }
                var productAccount = productAccounts.ParsedResult.FirstOrDefault(x =>
                    x.PriceAccount.Equals(mangoGroup.Oracles[marketIndex]));
                if (productAccount == null)
                {
                    tokenNames.Add("UNKNOWN");
                    continue;
                }

                tokenNames.Add(productAccount.Product.Description.Split("/")[0]);
            }

            tokenNames.Add("USDC");

            _mangoClient.SubscribeMangoCache((_, cache, _) =>
                {
                    Console.Clear();
                    mangoCache = cache;

                    var tBorrows = new I80F48(0);
                    var tDeposits = new I80F48(0);
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine(
                        $"| {"Token",-10} | {"Mint",-10} | {"Price",-15} | {"Borrow Rate",-15} | {"Deposit Rate",-15} |" +
                        $" {"Token Deposits",-20} | {"Deposits Value",-25} | {"Token Borrows",-20} | {"Borrows Value",-25} |"); 
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    for (int i = 0; i < Models.Constants.MaxTokens; i++)
                    {
                        var price = i == Models.Constants.MaxPairs ? I80F48.One : cache.PriceCaches[i].Price;
                        var lastUpdated = i == Models.Constants.MaxPairs ? DateTime.UtcNow : DateTime.UnixEpoch.AddSeconds(cache.PriceCaches[i].LastUpdated);
                        string log = "";
                        if (mangoGroup.Tokens[i].Mint != SystemProgram.ProgramIdKey)
                        {
                            log += $"| {tokenNames[i],-10} | {mangoGroup.Tokens[i].Mint.Key[..10]} | ${price.ToDecimal(),-14:N3} |";
                            var tokenDeposits = mangoGroup.RootBankAccounts[i].GetUiTotalDeposit(mangoGroup.Tokens[i].Decimals);
                            var tokenBorrows = mangoGroup.RootBankAccounts[i].GetUiTotalBorrow(mangoGroup.Tokens[i].Decimals);
                            var uiDeposits = mangoGroup.RootBankAccounts[i].GetUiTotalDeposit(mangoGroup.GetQuoteTokenInfo().Decimals);
                            var uiBorrows = mangoGroup.RootBankAccounts[i].GetUiTotalBorrow(mangoGroup.GetQuoteTokenInfo().Decimals);
                            var borrowsVal = uiBorrows * price;
                            var depositsVal = uiDeposits * price;
                            tBorrows += borrowsVal;
                            tDeposits += depositsVal;
                            log +=
                                $" {mangoGroup.RootBankAccounts[i].GetBorrowRate(mangoGroup.Tokens[i].Decimals).ToDecimal(),-15:P4} |" +
                                $" {mangoGroup.RootBankAccounts[i].GetDepositRate(mangoGroup.Tokens[i].Decimals).ToDecimal(),-15:P4} |";
                            log +=
                                $" {tokenDeposits.ToDecimal(),-20:N4} |" +
                                $" ${depositsVal.ToDecimal(),-24:N4} |" +
                                $" {tokenBorrows.ToDecimal(),-20:N4} |" +
                                $" ${borrowsVal.ToDecimal(),-24:N4} |";
                        }
                        else
                        {
                            log += $"| {"NaNi?!",-10} | {"NaNi?!",-10} | {"NaNi?!",-15} | {"NaNi?!",-15} | {"NaNi?!",-15} | {"NaNi?!",-20} | {"NaNi?!",-25} | {"NaNi?!",-20} | {"NaNi?!",-25} | ";
                        }
                        Console.WriteLine(log);
                    }
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine($"Total Deposits: ${tDeposits.ToDecimal(),-15:N4}\nTotal Borrows: ${tBorrows.ToDecimal(),-15:N4}\t\n");
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine(
                        $"| {"Market",-15} | {"Key",-10} | {"Long Funding",-15} | {"Short Funding",-15} | {"Open Interest",-15} |");
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    for (int i = 0; i < Models.Constants.MaxPairs; i++)
                    {
                        string log = "";
                        if (mangoGroup.PerpetualMarkets[i].Market != SystemProgram.ProgramIdKey)
                        {
                            // TODO: this is inaccurate, change this and write a method which receives the perpStats to properly calculate funding
                            var funding = (cache.PerpetualMarketCaches[i].LongFunding + cache.PerpetualMarketCaches[i].ShortFunding);
                            var fundingInQuoteDecimals = funding / new I80F48(Math.Pow(10, mangoGroup.GetQuoteTokenInfo().Decimals));
                            var basePriceInBaseLots = cache.PriceCaches[i].Price * new I80F48(mangoGroup.PerpMarketAccounts[i].NativeQuantityToUi(1m, mangoGroup.Tokens[i].Decimals));

                            log += 
                                $"| {tokenNames[i] + "-PERP",-15} |" +
                                $" {mangoGroup.PerpetualMarkets[i].Market.Key[..10],-10} |" +
                                $" {cache.PerpetualMarketCaches[i].LongFunding.ToDecimal(),-15:N4} |" +
                                $" {cache.PerpetualMarketCaches[i].ShortFunding.ToDecimal(),-15:N4} |" +
                                $" {mangoGroup.PerpMarketAccounts[i].NativeQuantityToUi(mangoGroup.PerpMarketAccounts[i].OpenInterest, mangoGroup.Tokens[i].Decimals) / 2,-15:N4} |";
                        }
                        else
                        {
                            log += $"| {"NaNi?!",-15} | {"NaNi?!",-10} | {"NaNi?!",-15} | {"NaNi?!",-15} | {"NaNi?!",-15} |";
                        }
                        Console.WriteLine(log);
                    }
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    
                }, Models.Constants.MangoCache,
                Commitment.Confirmed);

            Console.ReadLine();
        }
    }
}