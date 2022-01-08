using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Programs.Models;
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
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup);
            MangoCache mangoCache = _mangoClient.GetMangoCache(Constants.MangoCache).ParsedResult;
            mangoGroup.ParsedResult.LoadRootBanks(RpcClient);

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
                        $"Token: {mangoGroup.ParsedResult.Tokens[token].Mint}\t" +
                        $"Deposits: {mangoAccounts.ParsedResult[i].GetUiDeposit(mangoGroup.ParsedResult.RootBankAccounts[token], mangoGroup.ParsedResult, token):N6}\t" +
                        $"Borrows: {mangoAccounts.ParsedResult[i].GetUiBorrow(mangoGroup.ParsedResult.RootBankAccounts[token], mangoGroup.ParsedResult, token):N6}\t" +
                        $"MaxWithBorrow: {mangoAccounts.ParsedResult[i].GetMaxWithBorrowForToken(mangoGroup.ParsedResult, mangoCache, token):N6}\t" +
                        $"Net: {mangoAccounts.ParsedResult[i].GetUiNet(mangoCache.RootBankCaches[token], mangoGroup.ParsedResult, token):N6}\t");
                }

                Console.WriteLine(
                    $"Account Equity: {mangoAccounts.ParsedResult[i].GetEquity(mangoGroup.ParsedResult, mangoCache):N6}\n" +
                    $"Account Maintenance Health Ratio: {mangoAccounts.ParsedResult[i].GetHealthRatio(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance):N6}\n" +
                    $"Account Maintenance Health: {mangoAccounts.ParsedResult[i].GetHealth(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance):N6}\n" +
                    $"Account Initialization Health: {mangoAccounts.ParsedResult[i].GetHealth(mangoGroup.ParsedResult, mangoCache, HealthType.Initialization):N6}\n" +
                    $"Account Maintenance Health Ratio: {mangoAccounts.ParsedResult[i].GetHealthRatio(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance):N6}\n" +
                    $"Account Initialization Health Ratio: {mangoAccounts.ParsedResult[i].GetHealthRatio(mangoGroup.ParsedResult, mangoCache, HealthType.Initialization):N6}\n" +
                    $"Leverage: {mangoAccounts.ParsedResult[i].GetLeverage(mangoGroup.ParsedResult, mangoCache):N6}\n" +
                    $"Assets Value: {mangoAccounts.ParsedResult[i].GetAssetsValue(mangoGroup.ParsedResult, mangoCache):N6}\n" +
                    $"Liabilities Value: {mangoAccounts.ParsedResult[i].GetLiabilitiesValue(mangoGroup.ParsedResult, mangoCache):N6}\n" +
                    $"Assets Maintenance Value: {mangoAccounts.ParsedResult[i].GetAssetsValue(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance):N6}\n" +
                    $"Liabilities Maintenance Value: {mangoAccounts.ParsedResult[i].GetLiabilitiesValue(mangoGroup.ParsedResult, mangoCache, HealthType.Maintenance):N6}\n" +
                    $"Assets Initialization Value: {mangoAccounts.ParsedResult[i].GetAssetsValue(mangoGroup.ParsedResult, mangoCache, HealthType.Initialization):N6}\n" +
                    $"Liabilities Initialization Value: {mangoAccounts.ParsedResult[i].GetLiabilitiesValue(mangoGroup.ParsedResult, mangoCache, HealthType.Initialization):N6}\n");
            }

            Console.ReadLine();
        }
    }
}