using Solnet.Mango.Models;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs;
using Solnet.Programs.Models;
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

        public SubscribeMangoCacheExample()
        {
            StreamingRpcClient.ConnectAsync().Wait();
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup);
            MangoCache mangoCache = _mangoClient.GetMangoCache(Constants.MangoCache).ParsedResult;
            mangoGroup.ParsedResult.LoadRootBanks(RpcClient);

            foreach (RootBank rootBank in mangoGroup.ParsedResult.RootBankAccounts.Where(rootBank => rootBank != null))
            {
                rootBank.LoadNodeBanks(RpcClient);
                Task.Delay(100).Wait();
            }

            _mangoClient.SubscribeMangoCache((_, cache, _) =>
                {
                    Console.Clear();
                    mangoCache = cache;
                    foreach (PriceCache priceCache in
                        cache.PriceCaches.Where(priceCache => priceCache.Price.Value != 0))
                    {
                        Console.WriteLine(
                            $"{DateTime.UnixEpoch.AddSeconds((long)priceCache.LastUpdated)} - Price: {priceCache.Price.Value:C2}");
                    }

                    foreach (PerpMarketCache perpCache in cache.PerpetualMarketCaches.Where(perpCache =>
                        perpCache.LongFunding.Value != 0))
                    {
                        Console.WriteLine(
                            $"{DateTime.UnixEpoch.AddSeconds((long)perpCache.LastUpdated)} - Long Funding:\t{perpCache.LongFunding.Value}\tShort Funding:\t{perpCache.LongFunding.Value}");
                    }

                    foreach (RootBankCache rootBankCache in cache.RootBankCaches.Where(rootBankCache =>
                        rootBankCache.BorrowIndex.Value != 0))
                    {
                        Console.WriteLine(
                            $"{DateTime.UnixEpoch.AddSeconds((long)rootBankCache.LastUpdated)} - Borrow Index:\t{rootBankCache.BorrowIndex.Value}\tDeposit Index:\t{rootBankCache.DepositIndex.Value}");
                    }
                }, Constants.MangoCache,
                Commitment.Confirmed);

            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = _mangoClient.GetMangoAccounts(Owner);
            for (int i = 0; i < mangoAccounts.ParsedResult.Count; i++)
            {
                Console.WriteLine(
                    $"Account: {mangoAccounts.OriginalRequest.Result[i].PublicKey} Owner: {mangoAccounts.ParsedResult[i].Owner}");
                mangoAccounts.ParsedResult[i].LoadOpenOrdersAccounts(RpcClient);
                for (int token = 0; token < mangoGroup.ParsedResult.Tokens.Count; token++)
                {
                    if (mangoGroup.ParsedResult.Tokens[token].RootBank.Key == SystemProgram.ProgramIdKey.Key) continue;
                    Console.WriteLine(
                        $"Token: {mangoGroup.ParsedResult.Tokens[token].Mint} " +
                        $"Deposits: {mangoAccounts.ParsedResult[i].GetUiDeposit(mangoGroup.ParsedResult.RootBankAccounts[token], mangoGroup.ParsedResult, token):N4} " +
                        $"Borrows: {mangoAccounts.ParsedResult[i].GetUiBorrow(mangoGroup.ParsedResult.RootBankAccounts[token], mangoGroup.ParsedResult, token):N4} " +
                        $"MaxWithBorrow: {mangoAccounts.ParsedResult[i].GetMaxWithBorrowForToken(mangoGroup.ParsedResult, mangoCache, token)}");
                }

                Console.WriteLine(
                    $"Account Maint Health: {mangoAccounts.ParsedResult[i].GetHealthRatio(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance)}\n" +
                    $"Account Init Health: {mangoAccounts.ParsedResult[i].GetHealthRatio(mangoGroup.ParsedResult, mangoCache, HealthType.Initialization)}\n" +
                    $"Leverage: {mangoAccounts.ParsedResult[i].GetLeverage(mangoGroup.ParsedResult, mangoCache)}\n" +
                    $"Assets Value Maint: {mangoAccounts.ParsedResult[i].GetAssetsValue(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance)}\n" +
                    $"Liabilities Value Maint: {mangoAccounts.ParsedResult[i].GetLiabilitiesValue(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance)}\n" +
                    $"Assets Value Init: {mangoAccounts.ParsedResult[i].GetAssetsValue(mangoGroup.ParsedResult, mangoCache, HealthType.Initialization)}\n" +
                    $"Liabilities Value Init: {mangoAccounts.ParsedResult[i].GetLiabilitiesValue(mangoGroup.ParsedResult, mangoCache, HealthType.Initialization)}\n");
                _mangoClient.SubscribeMangoAccount((subscription, account, arg3) =>
                {
                    for (int token = 0; token < mangoGroup.ParsedResult.Tokens.Count; token++)
                    {
                        if (mangoGroup.ParsedResult.Tokens[token].RootBank.Key == SystemProgram.ProgramIdKey.Key)
                            continue;
                        Console.WriteLine(
                            $"Token: {mangoGroup.ParsedResult.Tokens[token].Mint} " +
                            $"Deposits: {account.GetUiDeposit(mangoGroup.ParsedResult.RootBankAccounts[token], mangoGroup.ParsedResult, token):N4} " +
                            $"Borrows: {account.GetUiBorrow(mangoGroup.ParsedResult.RootBankAccounts[token], mangoGroup.ParsedResult, token):N4} ");
                        Console.WriteLine(
                            $"Account Health: {mangoAccounts.ParsedResult[i].GetHealthRatio(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance)}");
                    }
                }, mangoAccounts.OriginalRequest.Result[i].PublicKey);
            }

            Console.ReadLine();
        }
    }
}