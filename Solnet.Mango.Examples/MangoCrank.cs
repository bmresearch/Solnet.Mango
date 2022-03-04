using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Solnet.KeyStore;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class MangoCrank : IRunnableExample
    {
        private readonly IRpcClient _rpcClient;
        private readonly IStreamingRpcClient _streamingRpcClient;
        private readonly ILogger _logger;
        private readonly IMangoClient _mangoClient;
        private readonly Wallet.Wallet _wallet;
        private readonly MangoProgram _mango;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<Task> _tasks;

        private MangoGroup _mangoGroup;
        private string _blockHash;

        public MangoCrank()
        {
            Console.WriteLine($"Initializing {ToString()}");

            //init stuff
            _tasks = new();
            _cancellationTokenSource = new ();
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

            // the programs
            _mango = MangoProgram.CreateDevNet();

            // the clients
            _rpcClient = Rpc.ClientFactory.GetClient(Cluster.DevNet, _logger);
            _streamingRpcClient = Rpc.ClientFactory.GetStreamingClient(Cluster.DevNet, _logger);
            _mangoClient = ClientFactory.GetClient(_rpcClient, _streamingRpcClient, _logger, _mango.ProgramIdKey);

            // get the wallet
            var keyStore = new SolanaKeyStoreService();
            _wallet = keyStore.RestoreKeystoreFromFile("C:\\Users\\warde\\hoakwp.json");

        }

        private Task CacheRecentBlockHash()
        {
            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.LogInformation("Fetching recent block hash.");
                    var blockHash = await _rpcClient.GetRecentBlockHashAsync(Rpc.Types.Commitment.Confirmed);
                    if (!blockHash.WasSuccessful)
                    {
                        _logger.LogInformation($"Could not fetch recent block hash. Reason: {blockHash.Reason}");
                        await Task.Delay(5000);
                        continue;
                    }

                    _blockHash = blockHash.Result.Value.Blockhash;
                    _logger.LogInformation($"Caching recent block hash: {_blockHash}.");
                    await Task.Delay(2500);
                }
            }, _cancellationTokenSource.Token);
        }

        private Task Crank()
        {
            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (string.IsNullOrEmpty(_blockHash))
                    {
                        _logger.LogInformation($"Awaiting block hash before starting crank.");
                        await Task.Delay(5000);
                        continue;
                    }

                    _logger.LogInformation($"Building new transaction with block hash: {_blockHash}.");

                    var txBuilder = new TransactionBuilder()
                    .SetRecentBlockHash(_blockHash)
                    .SetFeePayer(_wallet.Account)
                    .AddInstruction(CachePricesIx())
                    .AddInstruction(CacheRootBanksIx());

                    var msg = txBuilder.CompileMessage();
                    ExampleHelpers.DecodeAndLogMessage(msg);
                    var signature = _wallet.Account.Sign(msg);
                    txBuilder.AddSignature(signature);

                    var txBytes = txBuilder.Serialize();

                    var txSig = ExampleHelpers.SubmitTxSendAndLog(_rpcClient, txBytes);

                    txBuilder = new TransactionBuilder()
                    .SetRecentBlockHash(_blockHash)
                    .SetFeePayer(_wallet.Account)
                    .AddInstruction(CachePerpMarketsIx());

                    msg = txBuilder.CompileMessage();
                    ExampleHelpers.DecodeAndLogMessage(msg);
                    signature = _wallet.Account.Sign(msg);
                    txBuilder.AddSignature(signature);

                    txBytes = txBuilder.Serialize();

                    txSig = ExampleHelpers.SubmitTxSendAndLog(_rpcClient, txBytes);
                    await ExampleHelpers.PollTx(_rpcClient, txSig, Rpc.Types.Commitment.Confirmed);
                    await Task.Delay(10000);
                }
            }, _cancellationTokenSource.Token);
        }


        public async void Run()
        {
            ulong balance = _rpcClient.GetBalance(_wallet.Account.PublicKey).Result.Value;
            Console.WriteLine($"Account {_wallet.Account.PublicKey}\tBalance {(decimal)balance / SolHelper.LAMPORTS_PER_SOL}");

            /* 
             * Fetch data before starting the crank
             */

            var mangoGroup = await _mangoClient.GetMangoGroupAsync(Constants.DevNetMangoGroup);
            _mangoGroup = mangoGroup.ParsedResult;

            var cacheHashesTask = CacheRecentBlockHash();
            var crankTask = Crank();

            _tasks.Add(cacheHashesTask);
            _tasks.Add(crankTask);

            Console.ReadKey();

            _cancellationTokenSource.Cancel();

            Task.WaitAll(_tasks.ToArray());

            Console.WriteLine($"All tasks terminated.");
            Console.ReadKey();
        }

        /// <summary>
        /// Create the <see cref="MangoProgramInstructions.Values.CachePrices"/> instruction.
        /// </summary>
        private TransactionInstruction CachePricesIx()
            => _mango.CachePrices(Constants.DevNetMangoGroup, _mangoGroup.MangoCache,
                _mangoGroup.Oracles.Where(x => !x.Equals(SystemProgram.ProgramIdKey)).ToList());

        /// <summary>
        /// Create the <see cref="MangoProgramInstructions.Values.CachePrices"/> instruction.
        /// </summary>
        private TransactionInstruction CacheRootBanksIx()
            => _mango.CacheRootBanks(Constants.DevNetMangoGroup, _mangoGroup.MangoCache,
                _mangoGroup.Tokens.Where(x => !x.Mint.Equals(SystemProgram.ProgramIdKey)).Select(x => x.RootBank).ToList());

        /// <summary>
        /// Create the <see cref="MangoProgramInstructions.Values.CachePrices"/> instruction.
        /// </summary>
        private TransactionInstruction CachePerpMarketsIx()
            => _mango.CachePerpMarkets(Constants.DevNetMangoGroup, _mangoGroup.MangoCache,
                _mangoGroup.PerpetualMarkets.Where(x => !x.Market.Equals(SystemProgram.ProgramIdKey)).Select(x => x.Market).ToList());
    }
}
