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
            mangoGroup.ParsedResult.LoadRootBanks(_mangoClient);

            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = _mangoClient.GetMangoAccounts(Owner);
            for (int i = 0; i < mangoAccounts.ParsedResult.Count; i++)
            {
                Console.WriteLine(
                    $"Account: {mangoAccounts.OriginalRequest.Result[i].PublicKey} Owner: {mangoAccounts.ParsedResult[i].Owner}");
                mangoAccounts.ParsedResult[i].LoadOpenOrdersAccounts(RpcClient);
                ExampleHelpers.LogAccountStatus(_mangoClient, mangoGroup.ParsedResult, mangoCache, mangoAccounts.ParsedResult[i]);
            }

            Console.ReadLine();
        }
    }
}