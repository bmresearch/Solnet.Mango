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
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient("https://mango.devnet.rpcpool.com");

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient("wss://mango.devnet.rpcpool.com");

        private readonly IMangoClient _mangoClient;

        public GetMangoAccountExample()
        {
            Console.WriteLine($"Initializing {ToString()}");
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient, programId: MangoProgram.DevNetProgramIdKeyV3);
        }

        public void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);
            MangoCache mangoCache = _mangoClient.GetMangoCache(mangoGroup.ParsedResult.MangoCache).ParsedResult;
            mangoGroup.ParsedResult.LoadRootBanks(RpcClient);

            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = _mangoClient.GetMangoAccounts(Owner);
            for (int i = 0; i < mangoAccounts.ParsedResult.Count; i++)
            {
                Console.WriteLine(
                    $"Account: {mangoAccounts.OriginalRequest.Result[i].PublicKey} Owner: {mangoAccounts.ParsedResult[i].Owner}");
                mangoAccounts.ParsedResult[i].LoadOpenOrdersAccounts(RpcClient);
                LogAccountStatus(mangoGroup.ParsedResult, mangoCache, mangoAccounts.ParsedResult[i]);
            }

            Console.ReadLine();
        }

        private void LogAccountStatus(MangoGroup mangoGroup, MangoCache mangoCache, MangoAccount mangoAccount)
        {
            if (mangoGroup.RootBankAccounts.Count != 0)
            {
                for (int token = 0; token < mangoGroup.Tokens.Count; token++)
                {
                    if (mangoGroup.Tokens[token].RootBank.Key == SystemProgram.ProgramIdKey.Key) continue;
                    Console.WriteLine(
                        $"Token: {mangoGroup.Tokens[token].Mint}\t" +
                        $"Deposits: {mangoAccount.GetUiDeposit(mangoGroup.RootBankAccounts[token], mangoGroup, token).ToDecimal():N6}\t" +
                        $"Borrows: {mangoAccount.GetUiBorrow(mangoGroup.RootBankAccounts[token], mangoGroup, token).ToDecimal():N6}\t" +
                        $"MaxWithBorrow: {mangoAccount.GetMaxWithBorrowForToken(mangoGroup, mangoCache, token).ToDecimal():N6}\t" +
                        $"Net: {mangoAccount.GetUiNet(mangoCache.RootBankCaches[token], mangoGroup, token).ToDecimal():N6}\t");
                }
            }

            Console.WriteLine(
                $"Account Value: {mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Account Maintenance Health: {mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Account Initialization Health: {mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n" +
                $"Account Maintenance Health Ratio: {mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Account Initialization Health Ratio: {mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n" +
                $"Leverage: {mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Assets Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Liabilities Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Assets Maintenance Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Liabilities Maintenance Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Assets Initialization Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n" +
                $"Liabilities Initialization Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n");
        }
    }
}