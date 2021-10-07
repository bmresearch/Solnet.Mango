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
        private static readonly PublicKey Owner = new("skynetDj29GH6o6bAqoixCpDuYtWqi1rm8ZNx1hB3vq");
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
            }

            AccountResultWrapper<MangoCache> mangoCache = _mangoClient.GetMangoCache(MangoCache);

            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = _mangoClient.GetMangoAccounts(Owner);
            for (int i = 0; i < mangoAccounts.ParsedResult.Count; i++)
            {
                Console.WriteLine(
                    $"Account: {mangoAccounts.OriginalRequest.Result[i].PublicKey} Owner: {mangoAccounts.ParsedResult[i].Owner}");
                for(int token = 0; token < mangoGroup.ParsedResult.Tokens.Count; token++)
                {
                    if (mangoGroup.ParsedResult.Tokens[token].RootBank.Key == SystemProgram.ProgramIdKey.Key) continue;
                    var rootBank = _mangoClient.GetRootBank(mangoGroup.ParsedResult.Tokens[token].RootBank);
                    Console.WriteLine(
                        $"Token: {mangoGroup.ParsedResult.Tokens[token].Mint} " +
                        $"Deposits: {mangoAccounts.ParsedResult[i].GetUiDeposit(rootBank.ParsedResult, mangoGroup.ParsedResult, token):N4} " +
                        $"Borrows: {mangoAccounts.ParsedResult[i].GetUiBorrow(rootBank.ParsedResult, mangoGroup.ParsedResult, token):N4} ");
                    Task.Delay(250).Wait();
                }
            }

            Console.ReadLine();
        }
    }
}