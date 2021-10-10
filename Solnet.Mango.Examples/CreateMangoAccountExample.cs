using Solnet.KeyStore;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Mango.Examples
{
    public class CreateMangoAccountExample : IRunnableExample
    {
        private static readonly PublicKey Owner = new PublicKey("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");

        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);
        private static readonly IStreamingRpcClient StreamingRpcClient = Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);

        private readonly Wallet.Wallet _wallet;

        private IMangoClient _mangoClient;

        public CreateMangoAccountExample()
        {
            Console.WriteLine($"Initializing {ToString()}");
            // init stuff
            SolanaKeyStoreService keyStore = new();

            // get the wallet
            _wallet = keyStore.RestoreKeystoreFromFile("/home/murlux/hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh.json");
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }


        public void Run()
        {
            RequestResult<ResponseValue<BlockHash>> blockhash = RpcClient.GetRecentBlockHash();

            RequestResult<ulong> minBalance = RpcClient.GetMinimumBalanceForRentExemption(MangoAccount.Layout.Length);

            Account acc = new Account();
            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(Owner)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(SystemProgram.CreateAccount(
                    Owner,
                    acc,
                    minBalance.Result,
                    MangoAccount.Layout.Length,
                    MangoProgram.ProgramIdKeyV3))
                .AddInstruction(MangoProgram.InitializeMangoAccount(Constants.MangoGroup, acc, Owner))
                .AddInstruction(MangoProgram.AddMangoAccountInfo(Constants.MangoGroup, acc, Owner, "Solnet Test"));

            byte[] msg = txBuilder.CompileMessage();

            Console.WriteLine("Message Data: " + Convert.ToBase64String(msg));

            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(Message.Deserialize(msg));

            string aggregate = ix.Aggregate(
                "Decoded Instructions:",
                (s, instruction) =>
                {
                    s += $"\n\tProgram: {instruction.ProgramName}\n\t\t\t Instruction: {instruction.InstructionName}\n";
                    return instruction.Values.Aggregate(
                        s,
                        (current, entry) =>
                            current + $"\t\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                });
            Console.WriteLine(aggregate);

            byte[] txBytes = txBuilder.Build(new List<Account> { _wallet.Account, acc });

            RequestResult<string> res = RpcClient.SendTransaction(txBytes);

            Console.WriteLine(res.Result);

        }
    }
}