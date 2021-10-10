using Solnet.KeyStore;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Pyth;
using Solnet.Pyth.Models;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Serum;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants = Solnet.Mango.Models.Constants;

namespace Solnet.Mango.Examples
{
    public class PlacePerpOrderExample : IRunnableExample
    {
        private static readonly PublicKey Owner = new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");

        private static readonly IRpcClient RpcClient =
            Solnet.Rpc.ClientFactory.GetClient("https://solana-api.projectserum.com");

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient("wss://solana-api.projectserum.com");

        private readonly IPythClient _pythClient;

        private readonly Wallet.Wallet _wallet;

        private readonly IMangoClient _mangoClient;

        public PlacePerpOrderExample()
        {
            Console.WriteLine($"Initializing {ToString()}");
            // init stuff
            SolanaKeyStoreService keyStore = new();

            // get the wallet
            _wallet = keyStore.RestoreKeystoreFromFile("/path/to/keystore.json");
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
            _pythClient = Pyth.ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public async void Run()
        {
            Pyth.Models.AccountResultWrapper<MappingAccount> mappingAccount = await
                _pythClient.GetMappingAccountAsync(Pyth.Constants.MappingAccount);

            /*  Optionally perform a single request to get the product account
            foreach (PublicKey productAccountKey in mappingAccount.ParsedResult.ProductAccountKeys)
            {
                var productAccount = pythClient.GetProductAccount(productAccountKey);
            }
            */

            Pyth.Models.MultipleAccountsResultWrapper<List<ProductAccount>> productAccounts = await
                _pythClient.GetProductAccountsAsync(mappingAccount.ParsedResult);

            var _productAccount = productAccounts.ParsedResult.First(x => x.Product.Symbol.Contains("SOL"));
            var _productAccountIndex = productAccounts.ParsedResult.IndexOf(_productAccount);

            var _priceAccount = await _pythClient.GetPriceDataAccountsAsync(productAccounts.ParsedResult);

            Models.ProgramAccountsResultWrapper<List<MangoAccount>> mangoAccounts =
                await _mangoClient.GetMangoAccountsAsync(Owner);
            Console.WriteLine($"Type: {mangoAccounts.ParsedResult[0].Metadata.DataType}");
            Console.WriteLine($"PublicKey: {mangoAccounts.OriginalRequest.Result[0].PublicKey}");

            await Task.Delay(200);

            Models.AccountResultWrapper<MangoGroup> mangoGroup =
                await _mangoClient.GetMangoGroupAsync(Constants.MangoGroup);
            Console.WriteLine($"Type: {mangoGroup.ParsedResult.Metadata.DataType}");

            var _oracle = mangoGroup.ParsedResult.Oracles.First(x => x.Key == _productAccount.PriceAccount.Key);
            var _perpetualMarketIndex = mangoGroup.ParsedResult.Oracles.IndexOf(_oracle);

            await Task.Delay(200);

            var perpMarket =
                _mangoClient.GetPerpMarket(mangoGroup.ParsedResult.PerpetualMarkets[_perpetualMarketIndex].Market);

            await Task.Delay(200);

            RequestResult<ResponseValue<BlockHash>> blockhash = await RpcClient.GetRecentBlockHashAsync();

            var baseTokenInfo = mangoGroup.ParsedResult.Tokens[_perpetualMarketIndex];
            var qTokenInfo = mangoGroup.ParsedResult.Tokens[Models.Constants.MaxTokens - 1];
            var baseUnit = Math.Pow(10, baseTokenInfo.Decimals);
            var quoteUnit = Math.Pow(10, qTokenInfo.Decimals);

            var nativePrice = (long)((171f * quoteUnit) * perpMarket.ParsedResult.BaseLotSize) / (long)
                (perpMarket.ParsedResult.QuoteLotSize * baseUnit);

            var nativeQuantity = (long)(0.1f * baseUnit / perpMarket.ParsedResult.BaseLotSize);


            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(Owner)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(MangoProgram.PlacePerpOrder(
                    Constants.MangoGroup,
                    new(mangoAccounts.OriginalRequest.Result[1].PublicKey),
                    Owner,
                    Constants.MangoCache,
                    mangoGroup.ParsedResult.PerpetualMarkets[_perpetualMarketIndex].Market,
                    perpMarket.ParsedResult.Bids,
                    perpMarket.ParsedResult.Asks,
                    perpMarket.ParsedResult.EventQueue,
                    mangoAccounts.ParsedResult[1].SpotOpenOrders,
                    Serum.Models.Side.Sell,
                    Serum.Models.OrderType.Limit,
                    nativePrice,
                    nativeQuantity,
                    1_000_002ul
                ));

            byte[] msg = txBuilder.CompileMessage();

            ExampleHelpers.DecodeAndLogMessage(msg);

            byte[] txBytes = txBuilder.Build(new List<Wallet.Account>() { _wallet.Account });

            RequestResult<string> res =
                await RpcClient.SendTransactionAsync(txBytes, commitment: Rpc.Types.Commitment.Confirmed);

            Console.WriteLine(res.Result);

            await Task.Delay(500);


            blockhash = await RpcClient.GetRecentBlockHashAsync();
            txBuilder = new TransactionBuilder()
                .SetFeePayer(Owner)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(MangoProgram.CancelPerpOrderByClientId(
                    Constants.MangoGroup,
                    new(mangoAccounts.OriginalRequest.Result[1].PublicKey),
                    Owner,
                    mangoGroup.ParsedResult.PerpetualMarkets[_perpetualMarketIndex].Market,
                    perpMarket.ParsedResult.Bids,
                    perpMarket.ParsedResult.Asks,
                    1_000_002ul,
                    false
                ));

            msg = txBuilder.CompileMessage();

            ExampleHelpers.DecodeAndLogMessage(msg);

            txBytes = txBuilder.Build(new List<Wallet.Account>() { _wallet.Account });

            res = await RpcClient.SendTransactionAsync(txBytes, skipPreflight: true,
                commitment: Rpc.Types.Commitment.Confirmed);

            Console.WriteLine(res.Result);

            /**/
            Console.ReadKey();
        }
    }
}