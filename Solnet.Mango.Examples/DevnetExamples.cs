using Solnet.KeyStore;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Mango.Examples
{
    public class DevnetExamples : IRunnableExample
    {
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient("https://mango.devnet.rpcpool.com");

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient("wss://mango.devnet.rpcpool.com");

        private readonly IMangoClient _mangoClient;
        private readonly Wallet.Wallet _wallet;
        private readonly MangoProgram _mango;

        private List<Account> _signers;

        private AccountInfo _accountInfo;

        public DevnetExamples()
        {
            Console.WriteLine($"Initializing {ToString()}");

            var keyStore = new SolanaKeyStoreService();

            // get the wallet
            _wallet = keyStore.RestoreKeystoreFromFile("C:\\Users\\warde\\hoakwp.json");

            _signers = new();
            _mango = MangoProgram.CreateDevNet();
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            var mangoGroup = _mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);

            var createAccountIx = CreateMangoAccountIx();

            ExampleHelpers.DecodeAndLogMessage(createAccountIx);

            var txBytes = SignAndAssembleTx(createAccountIx);

            var sig = ExampleHelpers.SubmitTxSendAndLog(RpcClient, txBytes);
            ExampleHelpers.PollConfirmedTx(RpcClient, sig).Wait();
        }

        private byte[] CreateMangoAccountIx()
        {
            var blockhash = RpcClient.GetRecentBlockHash();

            var minBalance = RpcClient.GetMinimumBalanceForRentExemption(MangoAccount.Layout.Length);

            var mangoAccount = _mango.DeriveMangoAccountAddress(_wallet.Account, 1);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CreateMangoAccount(Constants.DevNetMangoGroup, mangoAccount, _wallet.Account, 1))
                .AddInstruction(_mango.AddMangoAccountInfo(Constants.DevNetMangoGroup, mangoAccount, _wallet.Account, "Solnet Test v1"));

            return txBuilder.CompileMessage();
        }

        private void CreateInitMangoAccount()
        {
            var createAccountIx = CreateInitMangoAccountIx();

            ExampleHelpers.DecodeAndLogMessage(createAccountIx);

            var sig = RpcClient.SendTransaction(createAccountIx);

            ExampleHelpers.PollConfirmedTx(RpcClient, sig.Result).Wait();
        }

        private byte[] CreateInitMangoAccountIx()
        {
            var blockhash = RpcClient.GetRecentBlockHash();

            var minBalance = RpcClient.GetMinimumBalanceForRentExemption(MangoAccount.Layout.Length);

            Account acc = new Account();
            _signers.Add(acc);
            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(SystemProgram.CreateAccount(
                    _wallet.Account,
                    acc,
                    minBalance.Result,
                    MangoAccount.Layout.Length,
                    _mango.ProgramIdKey))
                .AddInstruction(_mango.InitializeMangoAccount(Constants.DevNetMangoGroup, acc, _wallet.Account))
                .AddInstruction(_mango.AddMangoAccountInfo(Constants.DevNetMangoGroup, acc, _wallet.Account, "Solnet Test v1"));

            return txBuilder.CompileMessage();
        }

        private void RequestAirdrop()
        {
            var airdropTxSig = RpcClient.RequestAirdrop(_wallet.Account.PublicKey, SolHelper.LAMPORTS_PER_SOL).Result;

            ExampleHelpers.PollConfirmedTx(RpcClient, airdropTxSig).Wait();
        }
        private byte[] SignAndAssembleTx(byte[] msg)
        {
            var sigs = new List<byte[]>()
            {
                _wallet.Account.Sign(msg),
            };
            sigs.AddRange(_signers.Select(x => x.Sign(msg)));
            _signers.Clear();
            var tx = Transaction.Populate(Message.Deserialize(msg), sigs);
            return tx.Serialize();
        }
    }
}
