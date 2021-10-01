using Solnet.KeyStore;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Serum;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class DepositExample : IRunnableExample
    {
        private static readonly PublicKey Owner = new PublicKey("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");
        private static readonly PublicKey MangoGroup = new PublicKey("98pjRuQjK3qA6gXts96PqZT4Ze5QmnCmt3QYjhbUSPue");
        private static readonly PublicKey MangoCache = new("EBDRoayCDDUvDgCimta45ajQeXbexv7aKqJubruqpyvu");

        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);

        private readonly Wallet.Wallet _wallet;

        private readonly IMangoClient _mangoClient;

        public DepositExample()
        {
            Console.WriteLine($"Initializing {ToString()}");
            // init stuff
            SolanaKeyStoreService keyStore = new();

            // get the wallet
            _wallet = keyStore.RestoreKeystoreFromFile("/home/murlux/hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh.json");
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public async void Run()
        {
            ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts = await _mangoClient.GetMangoAccountsAsync(Owner);
            Console.WriteLine($"Type: {mangoAccounts.ParsedResult[0].Metadata.DataType}");
            Console.WriteLine($"PublicKey: {mangoAccounts.OriginalRequest.Result[0].PublicKey}");

            await Task.Delay(200);

            AccountResultWrapper<MangoGroup> mangoGroup = await _mangoClient.GetMangoGroupAsync(MangoGroup);
            Console.WriteLine($"Type: {mangoGroup.ParsedResult.Metadata.DataType}");

            await Task.Delay(200);

            TokenInfo wrappedSolTokenInfo =
                mangoGroup.ParsedResult.Tokens.First(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            Console.WriteLine($"RootBankKey: {wrappedSolTokenInfo.RootBank}");
            AccountResultWrapper<RootBank> wrappedSolRootBank = await _mangoClient.GetRootBankAsync(wrappedSolTokenInfo.RootBank);
            Console.WriteLine($"Type: {wrappedSolRootBank.ParsedResult.Metadata.DataType}");

            await Task.Delay(200);

            PublicKey nodeBankKey =
                wrappedSolRootBank.ParsedResult.NodeBanks.First(x => x.Key != SystemProgram.ProgramIdKey.Key);
            Console.WriteLine($"NodeBankKey: {nodeBankKey}");
            AccountResultWrapper<NodeBank> nodeBank = await _mangoClient.GetNodeBankAsync(nodeBankKey);
            Console.WriteLine($"Type: {nodeBank.ParsedResult.Metadata.DataType}");

            await Task.Delay(200);

            RequestResult<ResponseValue<BlockHash>> blockhash = await RpcClient.GetRecentBlockHashAsync();

            RequestResult<ulong> minBalance = await RpcClient.GetMinimumBalanceForRentExemptionAsync(TokenProgram.TokenAccountDataSize);

            Account acc = new Account();

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(Owner)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(SystemProgram.CreateAccount(
                    Owner,
                    acc,
                    minBalance.Result + 1_000_000,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(acc, MarketUtils.WrappedSolMint, Owner))
                .AddInstruction(MangoProgram.Deposit(
                    MangoGroup,
                    new PublicKey(mangoAccounts.OriginalRequest.Result[1].PublicKey),
                    Owner,
                    MangoCache,
                    wrappedSolTokenInfo.RootBank,
                    nodeBankKey,
                    nodeBank.ParsedResult.Vault,
                    acc, 1_000_000
                ))
                .AddInstruction(TokenProgram.CloseAccount(acc, Owner, Owner, TokenProgram.ProgramIdKey));

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
                            current +
                            $"\t\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                });
            Console.WriteLine(aggregate);

            byte[] txBytes = txBuilder.Build(new List<Account> { _wallet.Account, acc });

            RequestResult<string> res = await RpcClient.SendTransactionAsync(txBytes);

            Console.WriteLine(res.Result);

            Console.ReadKey();
        }
    }
}