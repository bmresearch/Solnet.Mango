using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Solnet.KeyStore;
using Solnet.Mango.Models;
using Solnet.Mango.Models.Banks;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Rpc.Utilities;
using Solnet.Serum;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TokenInfo = Solnet.Mango.Models.TokenInfo;

namespace Solnet.Mango.Examples
{
    public class DevnetExamples : IRunnableExample
    {
        private readonly IRpcClient _rpcClient;
        private readonly IStreamingRpcClient _streamingRpcClient;
        private readonly ILogger _logger;
        private readonly IMangoClient _mangoClient;
        private readonly ISerumClient _serumClient;
        private readonly Wallet.Wallet _wallet;
        private readonly MangoProgram _mango;
        private readonly SerumProgram _serum;

        private List<Account> _signers;

        public DevnetExamples()
        {
            Console.WriteLine($"Initializing {ToString()}");

            //init stuff
            _signers = new();
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
            _serum = SerumProgram.CreateDevNet();

            // the clients
            _rpcClient = Rpc.ClientFactory.GetClient(Cluster.DevNet, _logger);
            _streamingRpcClient = Rpc.ClientFactory.GetStreamingClient(Cluster.DevNet, _logger);
            _mangoClient = ClientFactory.GetClient(_rpcClient, _streamingRpcClient, _logger, _mango.ProgramIdKey);
            _serumClient = Serum.ClientFactory.GetClient(_rpcClient, _streamingRpcClient, _logger);

            // get the wallet
            var keyStore = new SolanaKeyStoreService();
            _wallet = keyStore.RestoreKeystoreFromFile("C:\\Users\\warde\\hoakwp.json");

        }

        public void Run()
        {
            ulong balance = _rpcClient.GetBalance(_wallet.Account.PublicKey).Result.Value;
            Console.WriteLine($"Account {_wallet.Account.PublicKey}\tBalance {(decimal)balance / SolHelper.LAMPORTS_PER_SOL}");

            /* 
             * Fetch data, depending on examples you may need to comment out certain RPC calls
             * i.e if creating an Advanced Orders Account you can't perform the request  for it beforehand
             * same goes if testing creation of Mango Account
             */

            var mangoAccountAddress = _mango.DeriveMangoAccountAddress(_wallet.Account, 1);

            var mangoGroup = _mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);
            mangoGroup.ParsedResult.LoadPerpMarkets(_mangoClient);

            mangoGroup.ParsedResult.LoadRootBanks(_mangoClient, _logger);
            var mangoCache = _mangoClient.GetMangoCache(mangoGroup.ParsedResult.MangoCache);

            var mangoAccount = _mangoClient.GetMangoAccount(mangoAccountAddress);
            mangoAccount.ParsedResult.LoadOpenOrdersAccounts(_rpcClient, _logger);

            var advancedOrders = _mangoClient.GetAdvancedOrdersAccount(mangoAccount.ParsedResult.AdvancedOrdersAccount);

            /* 
             * Example transaction submissions 
             * 
             */
            ExampleHelpers.LogAccountStatus(mangoGroup.ParsedResult, mangoCache.ParsedResult, mangoAccount.ParsedResult, advancedOrders.ParsedResult);

            var msg = BuyWithTimeInForceSOLPERP(mangoGroup.ParsedResult, mangoAccount.ParsedResult, mangoAccountAddress);

            // create account, add account info and set it's referral
            //var msg = CreateMangoAccountAddInfoAndSetReferral(mangoAccountAddress, "Mango Sharp Ref Test", "Mango Sharp");

            // register a referrer id
            //var msg = RegisterReferrerIdRecordIx(mangoAccountAddress, "Mango Sharp");

            // remove perp trigger buy
            //var msg = RemoveAdvancedOrderIx(mangoAccountAddress, mangoAccount.ParsedResult.AdvancedOrdersAccount, advancedOrders.ParsedResult);

            // set perp trigger buy
            //var msg = SetTriggerOrderSOLPERP(mangoGroup.ParsedResult, mangoAccount.ParsedResult, mangoAccountAddress);

            // close LONG on SOL-PERP
            // var msg = ClosePositionSOLPERP(mangoGroup.ParsedResult, mangoAccount.ParsedResult, mangoAccountAddress);

            // settle perp fees
            // var msg = SettleFeesSOLPERP(mangoGroup.ParsedResult, mangoAccount.ParsedResult, mangoAccountAddress);

            // var msg = AddAdvancedOrderIx(mangoGroup.ParsedResult, mangoAccount.ParsedResult, perp);

            // deposit to an account
            // var msg = Deposit(mangoGroup.ParsedResult, mangoAccountAddress, MarketUtils.WrappedSolMint, 58);

            // withdraw from an account
            //var msg = Withdraw(mangoGroup.ParsedResult, mangoAccount.ParsedResult, mangoAccountAddress, new("8FRFC6MoGGkMFQwngccyu69VnYbzykGeez7ignHVAFSN"), 2500);

            ExampleHelpers.DecodeAndLogMessage(msg);

            Console.ReadLine();
            var txBytes = SignAndAssembleTx(msg);

            Console.ReadLine();
            var sig = ExampleHelpers.SubmitTxSendAndLog(_rpcClient, txBytes);
            ExampleHelpers.PollTx(_rpcClient, sig, Commitment.Confirmed).Wait();
        }

        private byte[] CancelSellSpotOrderSOLUSDC(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;

            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            var market = _serumClient.GetMarket(mangoGroup.SpotMarkets[tokenIndex].Market);

            var order = mangoAccount.OpenOrdersAccounts[tokenIndex].Orders.First();

            return CancelSpotOrderByOrderIdIx(mangoGroup, mangoAccount, mangoAccountAddress, market,
                mangoGroup.SpotMarkets[tokenIndex].Market, order.OrderId, order.IsBid ? Side.Buy : Side.Sell);
        }

        private byte[] SettleFeesSOLPERP(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;

            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            var market = _mangoClient.GetPerpMarket(mangoGroup.PerpetualMarkets[tokenIndex].Market);

            Console.WriteLine($"Fees Accrued: {MangoUtils.HumanizeNative(market.ParsedResult.FeesAccrued, mangoGroup.GetQuoteTokenInfo().Decimals)}");

            return SettleFeesIx(mangoGroup, mangoGroup.PerpetualMarkets[tokenIndex].Market, mangoAccountAddress);
        }

        private byte[] ClosePositionSOLPERP(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;
            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            TokenInfo quoteTokenInfo =
                mangoGroup.GetQuoteTokenInfo();

            var market = _mangoClient.GetPerpMarket(mangoGroup.PerpetualMarkets[tokenIndex].Market);

            return PlacePerpOrder(mangoGroup, mangoAccount, mangoAccountAddress, wrappedSolTokenInfo, quoteTokenInfo,
                market.ParsedResult, mangoGroup.PerpetualMarkets[tokenIndex].Market, Side.Sell, PerpOrderType.ImmediateOrCancel,
                (float)mangoAccount.PerpetualAccounts[tokenIndex].GetUiBasePosition(market.ParsedResult, wrappedSolTokenInfo.Decimals), 105, true);
        }

        private byte[] SetTriggerOrderSOLPERP(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo baseTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (baseTokenInfo == null) return null;
            int tokenIndex = mangoGroup.GetTokenIndex(baseTokenInfo.Mint);

            TokenInfo quoteTokenInfo =
                mangoGroup.GetQuoteTokenInfo();

            var market = _mangoClient.GetPerpMarket(mangoGroup.PerpetualMarkets[tokenIndex].Market);

            var advancedOrdersAddress = _mango.DeriveAdvancedOrdersAccountAddress(mangoAccountAddress);

            return AddAdvancedOrderIx(mangoGroup, mangoAccount, market.ParsedResult, mangoGroup.PerpetualMarkets[tokenIndex].Market,
                baseTokenInfo, quoteTokenInfo, mangoAccountAddress, advancedOrdersAddress, Side.Buy, PerpOrderType.Market, TriggerCondition.Below,
                100, 100, 100, false);
        }

        private byte[] MarketBuySOLPERP(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;
            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            TokenInfo quoteTokenInfo =
                mangoGroup.GetQuoteTokenInfo();

            var market = _mangoClient.GetPerpMarket(mangoGroup.PerpetualMarkets[tokenIndex].Market);

            return PlacePerpOrder(mangoGroup, mangoAccount, mangoAccountAddress, wrappedSolTokenInfo, quoteTokenInfo,
                market.ParsedResult, mangoGroup.PerpetualMarkets[tokenIndex].Market, Side.Buy, PerpOrderType.ImmediateOrCancel, 100, 112, false);
        }

        private byte[] BuyWithTimeInForceSOLPERP(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress, PublicKey referrer = null)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;
            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            TokenInfo quoteTokenInfo =
                mangoGroup.GetQuoteTokenInfo();

            var market = _mangoClient.GetPerpMarket(mangoGroup.PerpetualMarkets[tokenIndex].Market);

            return PlacePerpOrder2(mangoGroup, mangoAccount, mangoAccountAddress, wrappedSolTokenInfo, quoteTokenInfo,
                market.ParsedResult, mangoGroup.PerpetualMarkets[tokenIndex].Market, Side.Buy, PerpOrderType.Limit, 100, 75, false,
                120, 1, referrer);
        }

        private byte[] MarketSellSpotSOLUSDC(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;
            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            var market = _serumClient.GetMarket(mangoGroup.SpotMarkets[tokenIndex].Market);

            var order = new OrderBuilder()
                .SetSide(Side.Sell)
                .SetOrderType(OrderType.ImmediateOrCancel)
                .SetSelfTradeBehavior(SelfTradeBehavior.AbortTransaction)
                .SetClientOrderId(1_000_000UL)
                .SetPrice(110)
                .SetQuantity(10)
                .Build();

            return PlaceSpotOrderIx(mangoGroup, mangoAccount, mangoAccountAddress, market, mangoGroup.SpotMarkets[tokenIndex].Market, order);
        }

        private byte[] CloseSpotOpenOrdersSOLUSDC(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;

            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            return CloseSpotOpenOrdersIx(mangoGroup,
                mangoAccountAddress,
                mangoGroup.SpotMarkets[tokenIndex].Market,
                mangoAccount.SpotOpenOrders[tokenIndex]);
        }

        private byte[] CreateSpotOpenOrdersSOLUSDC(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress)
        {
            TokenInfo wrappedSolTokenInfo =
                mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return null;

            int tokenIndex = mangoGroup.GetTokenIndex(wrappedSolTokenInfo.Mint);

            return CreateSpotOpenOrdersIx(mangoGroup, mangoAccountAddress, mangoGroup.SpotMarkets[tokenIndex].Market);
        }

        private byte[] Withdraw(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress, PublicKey tokenMint, double withdrawAmount)
        {
            TokenInfo tokenInfo = mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Key == tokenMint);
            if (tokenInfo == null) return null;
            var tokenIndex = mangoGroup.GetTokenIndex(tokenInfo.Mint);
            AccountResultWrapper<RootBank> tokenRootBank = _mangoClient.GetRootBank(tokenInfo.RootBank);
            Console.WriteLine($"TokenMint: {tokenInfo.Mint}");
            Console.WriteLine($"RootBankKey: {tokenInfo.RootBank}");
            Console.WriteLine($"Type: {tokenRootBank.ParsedResult.Metadata.DataType}");

            Task.Delay(200).Wait();

            PublicKey nodeBankKey = tokenRootBank.ParsedResult.NodeBanks.FirstOrDefault(x => x.Key != SystemProgram.ProgramIdKey.Key);
            if (nodeBankKey == null) return null;
            AccountResultWrapper<NodeBank> nodeBank = _mangoClient.GetNodeBank(nodeBankKey);
            Console.WriteLine($"NodeBankKey: {nodeBankKey}");
            Console.WriteLine($"Type: {nodeBank.ParsedResult.Metadata.DataType}");

            var balance = mangoAccount.GetUiDeposit(mangoGroup.RootBankAccounts[tokenIndex], mangoGroup, tokenIndex).ToDecimal();
            var amount = (ulong)withdrawAmount * (ulong)Math.Pow(10, tokenInfo.Decimals);

            Console.WriteLine($"Balance: {balance}\t\tWithdraw Amount: {amount}");

            var tokenAccounts = _rpcClient.GetTokenAccountsByOwner(_wallet.Account.PublicKey, tokenMint);

            var tokenAccount = tokenAccounts.Result.Value != null ? tokenAccounts.Result.Value.Select(x => x.PublicKey).FirstOrDefault() : null;

            return WithdrawIx(mangoGroup, mangoAccountAddress, tokenInfo.RootBank, nodeBankKey, nodeBank.ParsedResult.Vault, mangoAccount.SpotOpenOrders, amount,
                tokenAccount != null ? new(tokenAccount) : null);
        }

        private byte[] Deposit(MangoGroup mangoGroup, PublicKey mangoAccount, PublicKey tokenMint, double depositAmount)
        {
            TokenInfo tokenInfo = mangoGroup.Tokens.First(x => x.Mint.Key == tokenMint);
            if (tokenInfo == null) return null;
            var tokenIndex = mangoGroup.GetTokenIndex(tokenInfo.Mint);
            AccountResultWrapper<RootBank> rootBank = _mangoClient.GetRootBank(tokenInfo.RootBank);
            Console.WriteLine($"TokenMint: {tokenInfo.Mint}");
            Console.WriteLine($"RootBankKey: {tokenInfo.RootBank}");
            Console.WriteLine($"Type: {rootBank.ParsedResult.Metadata.DataType}");

            Task.Delay(200).Wait();

            PublicKey nodeBankKey = rootBank.ParsedResult.NodeBanks.First(x => x.Key != SystemProgram.ProgramIdKey.Key);
            if (nodeBankKey == null) return null;
            AccountResultWrapper<NodeBank> nodeBank = _mangoClient.GetNodeBank(nodeBankKey);
            Console.WriteLine($"NodeBankKey: {nodeBankKey}");
            Console.WriteLine($"Type: {nodeBank.ParsedResult.Metadata.DataType}");

            ulong amount = (ulong)depositAmount * (ulong)Math.Pow(10, tokenInfo.Decimals);

            Console.WriteLine($"Deposit Amount: {amount}");
            var tokenAccounts = _rpcClient.GetTokenAccountsByOwner(_wallet.Account.PublicKey, tokenMint);

            var tokenAccount = tokenAccounts.Result.Value != null ? tokenAccounts.Result.Value.Select(x => x.PublicKey).FirstOrDefault() : null;

            return DepositIx(mangoGroup, mangoAccount, tokenInfo.RootBank, nodeBankKey, nodeBank.ParsedResult.Vault, amount,
                tokenAccount != null ? new(tokenAccount) : null);
        }

        private byte[] CreateMangoAccountAddInfoAndSetReferral(PublicKey mangoAccountAddress, string accountInfo, string referrerId)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var referrerMemory = _mango.DeriveReferrerMemory(mangoAccountAddress);
            var referrerIdRecord = _mango.DeriveReferrerIdRecord(referrerId);
            var referrerIdRecordAccount = _mangoClient.GetReferrerIdRecordAccount(referrerIdRecord);

            Console.WriteLine($"Referrer Id: {referrerId} Referrer Id Record: {referrerIdRecord} Referrer Mango Account: {referrerIdRecordAccount.ParsedResult.Referrer}");

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CreateMangoAccount(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    4))
                .AddInstruction(_mango.AddMangoAccountInfo(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    accountInfo))
                .AddInstruction(_mango.SetReferrerMemory(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    referrerMemory, 
                    referrerIdRecordAccount.ParsedResult.Referrer,
                    _wallet.Account));

            return txBuilder.CompileMessage();
        }

        private byte[] RegisterReferrerIdRecordIx(PublicKey mangoAccountAddress, string referrerId)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var referrerIdRecord = _mango.DeriveReferrerIdRecord(referrerId);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.RegisterReferrerId(
                    Constants.DevNetMangoGroup,
                    referrerIdRecord,
                    mangoAccountAddress,
                    _wallet.Account,
                    referrerId));

            return txBuilder.CompileMessage();
        }

        private byte[] SetReferrerMemoryIx(PublicKey mangoAccountAddress, PublicKey referrerMangoAccount)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var referrerMemory = _mango.DeriveReferrerMemory(mangoAccountAddress);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.SetReferrerMemory(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    referrerMemory,
                    referrerMangoAccount,
                    _wallet.Account));

            return txBuilder.CompileMessage();
        }

        private byte[] RemoveAdvancedOrderIx(PublicKey mangoAccountAddress, PublicKey advancedOrdersAddress, AdvancedOrdersAccount advancedOrders)
        {

            var blockhash = _rpcClient.GetRecentBlockHash();

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash);

            for (int i = 0; i < advancedOrders.AdvancedOrders.Count; i++)
            {
                txBuilder
                .AddInstruction(_mango.RemoveAdvancedOrder(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    advancedOrdersAddress,
                    (byte)i
                    ));

                var msgSize = txBuilder.CompileMessage().Length;
                if (msgSize < 1150) continue;
                break;
            }

            return txBuilder.CompileMessage();
        }

        private byte[] SetDelegateIx(PublicKey mangoAccountAddress)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.SetDelegate(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    new("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj")
                    ));

            return txBuilder.CompileMessage();
        }

        private byte[] AddAdvancedOrderIx(MangoGroup mangoGroup, MangoAccount mangoAccount, PerpMarket market, PublicKey perpMarket,
            TokenInfo baseTokenInfo, TokenInfo quoteTokenInfo, PublicKey mangoAccountAddress, PublicKey advancedOrdersAddress, Side side,
            PerpOrderType perpOrderType, TriggerCondition triggerCondition, float size,
            float price, float triggerPrice, bool reduceOnly, ulong clientOrderId = 1UL)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var (nativePrice, nativeQuantity) = market.UiToNativePriceQuantity(price, size, baseTokenInfo.Decimals, quoteTokenInfo.Decimals);

            var nativeTriggerPrice = MangoUtils.TriggerPriceToNative((decimal) triggerPrice, baseTokenInfo.Decimals, quoteTokenInfo.Decimals);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.AddPerpTriggerOrder(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    advancedOrdersAddress,
                    mangoGroup.MangoCache,
                    perpMarket,
                    mangoAccount.SpotOpenOrders,
                    perpOrderType,
                    side,
                    nativePrice,
                    nativeQuantity,
                    triggerCondition,
                    nativeTriggerPrice,
                    clientOrderId,
                    reduceOnly
                    ));

            return txBuilder.CompileMessage();
        }

        private byte[] SettleFeesIx(MangoGroup mangoGroup, PublicKey perpMarket, PublicKey mangoAccountAddress)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var quoteTokenInfo = mangoGroup.GetQuoteTokenInfo();
            if (quoteTokenInfo == null) return null;
            var quoteRootBankIndex = mangoGroup.GetRootBankIndex(quoteTokenInfo.RootBank);
            var quoteRootBank = mangoGroup.RootBankAccounts[quoteRootBankIndex];
            var quoteNodeBankKey = quoteRootBank.NodeBanks.FirstOrDefault(x => x.Key != SystemProgram.ProgramIdKey.Key);
            if (quoteNodeBankKey == null) return null;
            var quoteNodeBankIndex = quoteRootBank.GetNodeBankIndex(quoteNodeBankKey);
            var quoteNodeBank = quoteRootBank.NodeBankAccounts[quoteNodeBankIndex];

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.SettleFees(
                    Constants.DevNetMangoGroup,
                    mangoGroup.MangoCache,
                    perpMarket,
                    mangoAccountAddress,
                    quoteTokenInfo.RootBank,
                    quoteNodeBankKey,
                    quoteNodeBank.Vault,
                    mangoGroup.FeesVault,
                    mangoGroup.SignerKey
                    ));

            return txBuilder.CompileMessage();

        }

        private byte[] CancelSpotOrderByOrderIdIx(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress,
            Market market, PublicKey spotMarket, BigInteger orderId, Side side)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var baseTokenInfo = mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Equals(market.BaseMint));
            if (baseTokenInfo == null) return null;
            int baseTokenIndex = mangoGroup.GetTokenIndex(baseTokenInfo.Mint);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CancelSpotOrder(
                    Constants.DevNetMangoGroup,
                    _wallet.Account,
                    mangoAccountAddress,
                    spotMarket,
                    market.Bids,
                    market.Asks,
                    mangoAccount.SpotOpenOrders[baseTokenIndex],
                    mangoGroup.SignerKey,
                    market.EventQueue,
                    orderId,
                    side
                    ));

            return txBuilder.CompileMessage();
        }

        private byte[] CancelPerpOrderByClientIdIx(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress,
            PerpMarket market, PublicKey perpMarket, ulong clientOrderId, Side side)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();
            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CancelPerpOrderByClientId(
                    Constants.DevNetMangoGroup,
                    _wallet.Account,
                    mangoAccountAddress,
                    perpMarket,
                    market.Bids,
                    market.Asks,
                    clientOrderId,
                    false));

            return txBuilder.CompileMessage();
        }

        private byte[] CancelPerpOrderByOrderIdIx(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress,
            PerpMarket market, PublicKey perpMarket, BigInteger orderId, Side side)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CancelPerpOrder(
                    Constants.DevNetMangoGroup,
                    _wallet.Account,
                    mangoAccountAddress,
                    perpMarket,
                    market.Bids,
                    market.Asks,
                    orderId,
                    false));

            return txBuilder.CompileMessage();
        }

        private byte[] PlacePerpOrder(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress,
            TokenInfo baseTokenInfo, TokenInfo quoteTokenInfo, PerpMarket market, PublicKey perpMarket, Side side,
            PerpOrderType perpOrderType, float size, float price, bool reduceOnly, ulong clientOrderId = 1UL)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var (nativePrice, nativeQuantity) = market.UiToNativePriceQuantity(price, size, baseTokenInfo.Decimals, quoteTokenInfo.Decimals);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.PlacePerpOrder(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    mangoGroup.MangoCache,
                    perpMarket,
                    market.Bids,
                    market.Asks,
                    market.EventQueue,
                    mangoAccount.SpotOpenOrders,
                    side,
                    perpOrderType,
                    nativePrice,
                    nativeQuantity,
                    clientOrderId,
                    reduceOnly));

            return txBuilder.CompileMessage();
        }
        private byte[] PlacePerpOrder2(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress,
            TokenInfo baseTokenInfo, TokenInfo quoteTokenInfo, PerpMarket market, PublicKey perpMarket, Side side,
            PerpOrderType perpOrderType, float size, float price, bool reduceOnly, byte timeInForce = byte.MinValue,
            ulong clientOrderId = 1UL, PublicKey referrerMangoAccount = null)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var (nativePrice, nativeQuantity) = market.UiToNativePriceQuantity(price, size, baseTokenInfo.Decimals, quoteTokenInfo.Decimals);

            var maxQuote = size * price;

            var maxQuoteQuantity = market.UiQuoteToLots(maxQuote, quoteTokenInfo.Decimals);

            var ts = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var expiryTs = ts + timeInForce;

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.PlacePerpOrder2(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    mangoGroup.MangoCache,
                    perpMarket,
                    market.Bids,
                    market.Asks,
                    market.EventQueue,
                    mangoAccount.SpotOpenOrders,
                    side,
                    perpOrderType,
                    nativePrice,
                    nativeQuantity,
                    clientOrderId,
                    expiryTs,
                    maxQuoteQuantity,
                    reduceOnly,
                    referrerMangoAccount
                    ));

            return txBuilder.CompileMessage();
        }

        private byte[] PlaceSpotOrderIx(MangoGroup mangoGroup, MangoAccount mangoAccount, PublicKey mangoAccountAddress, Market market, PublicKey spotMarket, Order order)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var baseTokenInfo = mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Equals(market.BaseMint));
            if (baseTokenInfo == null) return null;
            int baseTokenIndex = mangoGroup.GetTokenIndex(baseTokenInfo.Mint);
            var baseRootBankIndex = mangoGroup.GetRootBankIndex(baseTokenInfo.RootBank);
            var baseRootBank = mangoGroup.RootBankAccounts[baseRootBankIndex];
            var baseNodeBankKey = baseRootBank.NodeBanks.FirstOrDefault(x => x.Key != SystemProgram.ProgramIdKey.Key);
            if (baseNodeBankKey == null) return null;
            var baseNodeBankIndex = baseRootBank.GetNodeBankIndex(baseNodeBankKey);
            var baseNodeBank = baseRootBank.NodeBankAccounts[baseNodeBankIndex];

            var quoteTokenInfo = mangoGroup.Tokens.FirstOrDefault(x => x.Mint.Equals(market.QuoteMint));
            if (quoteTokenInfo == null) return null;
            var quoteRootBankIndex = mangoGroup.GetRootBankIndex(quoteTokenInfo.RootBank);
            var quoteRootBank = mangoGroup.RootBankAccounts[quoteRootBankIndex];
            var quoteNodeBankKey = quoteRootBank.NodeBanks.FirstOrDefault(x => x.Key != SystemProgram.ProgramIdKey.Key);
            if (quoteNodeBankKey == null) return null;
            var quoteNodeBankIndex = quoteRootBank.GetNodeBankIndex(quoteNodeBankKey);
            var quoteNodeBank = quoteRootBank.NodeBankAccounts[quoteNodeBankIndex];

            order.ConvertOrderValues(baseTokenInfo.Decimals, quoteTokenInfo.Decimals, market);

            var dexSigner = SerumProgramData.DeriveVaultSignerAddress(market, _serum.ProgramIdKey);
            var openOrders = mangoAccount.SpotOpenOrders.Where((t, i) => t != null && mangoAccount.InMarginBasket[i]).ToList();
            if (openOrders.Count == 0) openOrders.Add(mangoAccount.SpotOpenOrders[baseTokenIndex]);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.PlaceSpotOrder(
                    Constants.DevNetMangoGroup,
                    mangoAccountAddress,
                    _wallet.Account,
                    mangoGroup.MangoCache,
                    spotMarket,
                    market.Bids,
                    market.Asks,
                    market.RequestQueue,
                    market.EventQueue,
                    market.BaseVault,
                    market.QuoteVault,
                    baseTokenInfo.RootBank,
                    baseNodeBankKey,
                    baseNodeBank.Vault,
                    quoteTokenInfo.RootBank,
                    quoteNodeBankKey,
                    quoteNodeBank.Vault,
                    mangoGroup.SignerKey,
                    dexSigner,
                    mangoGroup.SerumVault,
                    mangoAccount.SpotOpenOrders,
                    baseTokenIndex,
                    order
                ))
                .AddInstruction(_mango.SettleFunds(
                    Constants.DevNetMangoGroup,
                    mangoGroup.MangoCache,
                    mangoAccountAddress,
                    _wallet.Account,
                    spotMarket,
                    mangoAccount.SpotOpenOrders[baseTokenIndex],
                    mangoGroup.SignerKey,
                    market.BaseVault,
                    market.QuoteVault,
                    baseTokenInfo.RootBank,
                    baseNodeBankKey,
                    quoteTokenInfo.RootBank,
                    quoteNodeBankKey,
                    baseNodeBank.Vault,
                    quoteNodeBank.Vault,
                    dexSigner));

            return txBuilder.CompileMessage();
        }


        /// <summary>
        /// Creates the instruction to close the spot open orders account.
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="spotMarket"></param>
        /// <param name="openOrders"></param>
        /// <returns></returns>
        private byte[] CloseSpotOpenOrdersIx(MangoGroup mangoGroup, PublicKey mangoAccount, PublicKey spotMarket, PublicKey openOrders)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CloseSpotOpenOrders(
                    Constants.DevNetMangoGroup,
                    mangoAccount,
                    _wallet.Account,
                    openOrders,
                    spotMarket,
                    mangoGroup.SignerKey));

            return txBuilder.CompileMessage();
        }

        /// <summary>
        /// Creates the instructions to create a spot open orders account
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="spotMarket"></param>
        /// <returns></returns>
        private byte[] CreateSpotOpenOrdersIx(MangoGroup mangoGroup, PublicKey mangoAccount, PublicKey spotMarket)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var minBalance = _rpcClient.GetMinimumBalanceForRentExemption(OpenOrdersAccount.Layout.SpanLength);

            Account acc = new Account();
            _signers.Add(acc);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(SystemProgram.CreateAccount(
                    _wallet.Account,
                    acc,
                    minBalance.Result,
                    OpenOrdersAccount.Layout.SpanLength,
                    _serum.ProgramIdKey))
                .AddInstruction(_mango.InitSpotOpenOrders(
                    Constants.DevNetMangoGroup,
                    mangoAccount,
                    _wallet.Account,
                    acc,
                    spotMarket,
                    mangoGroup.SignerKey));

            return txBuilder.CompileMessage();
        }

        /// <summary>
        /// Creates the instructions to withdraw funds from a mango account (defaults to SOL)
        /// </summary>
        /// <remarks>
        /// If you desire to withdraw another token you do not need to create, initialize and close the token account, it is used to wrap SOL.
        /// </remarks>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="vault"></param>
        /// <param name="openOrders"></param>
        /// <returns></returns>
        private byte[] WithdrawIx(MangoGroup mangoGroup, PublicKey mangoAccount, PublicKey rootBank, PublicKey nodeBank, PublicKey vault,
            List<PublicKey> openOrders, ulong balance, PublicKey tokenAccount = null)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            /// in case we need to create wSOL account
            Account acc = new Account();

            var destAcc = acc.PublicKey;
            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash);

            /// need to create wSOL token account
            if (rootBank == mangoGroup.Tokens[mangoGroup.GetTokenIndex(MarketUtils.WrappedSolMint)].RootBank)
            {
                var minBalance = _rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize);
                _signers.Add(acc);
                txBuilder.AddInstruction(SystemProgram.CreateAccount(
                    _wallet.Account,
                    destAcc,
                    minBalance.Result,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(destAcc, MarketUtils.WrappedSolMint, _wallet.Account));
            }

            if (tokenAccount == null && rootBank != mangoGroup.Tokens[mangoGroup.GetTokenIndex(MarketUtils.WrappedSolMint)].RootBank)
            {
                var tokenInfo = mangoGroup.Tokens.First(x => x.RootBank == rootBank);
                var tokenAccountAddress = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(_wallet.Account, tokenInfo.Mint);
                destAcc = tokenAccountAddress;
                txBuilder.AddInstruction(AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(_wallet.Account, _wallet.Account, tokenInfo.Mint));
            }

            txBuilder
                .AddInstruction(_mango.Withdraw(
                    Constants.DevNetMangoGroup,
                    mangoAccount,
                    _wallet.Account,
                    mangoGroup.MangoCache,
                    rootBank,
                    nodeBank,
                    vault,
                    destAcc,
                    mangoGroup.SignerKey,
                    openOrders,
                    balance,
                    false
                ));

            /// need to close wSOL token account
            if (rootBank == mangoGroup.Tokens[mangoGroup.GetTokenIndex(MarketUtils.WrappedSolMint)].RootBank)
                txBuilder.AddInstruction(TokenProgram.CloseAccount(destAcc, _wallet.Account, _wallet.Account, TokenProgram.ProgramIdKey));

            return txBuilder.CompileMessage();
        }

        /// <summary>
        /// Creates the instructions to deposit funds to a mango account (defaults to SOL)
        /// </summary>
        /// <remarks>
        /// If you desire to deposit another token you do not need to create, initialize and close the token account, it is used to wrap SOL.
        /// </remarks>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="vault"></param>
        /// <returns></returns>
        private byte[] DepositIx(MangoGroup mangoGroup, PublicKey mangoAccount, PublicKey rootBank, PublicKey nodeBank, PublicKey vault,
            ulong amount, PublicKey tokenAccount = null)
        {

            var blockhash = _rpcClient.GetRecentBlockHash();

            var minBalance = _rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize);

            /// in case we need to create wSOL account
            Account acc = new Account();
            _signers.Add(acc);

            var destAcc = acc.PublicKey;

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash);

            /// need to create wSOL token account
            if (rootBank == mangoGroup.Tokens[mangoGroup.GetTokenIndex(MarketUtils.WrappedSolMint)].RootBank)
            {
                txBuilder.AddInstruction(SystemProgram.CreateAccount(
                    _wallet.Account,
                    destAcc,
                    minBalance.Result +
                    amount,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(destAcc, MarketUtils.WrappedSolMint, _wallet.Account));
            }

            if (tokenAccount != null && rootBank != mangoGroup.Tokens[mangoGroup.GetTokenIndex(MarketUtils.WrappedSolMint)].RootBank)
            {
                destAcc = tokenAccount;
            }

            txBuilder.AddInstruction(_mango.Deposit(
                Constants.DevNetMangoGroup,
                mangoAccount,
                _wallet.Account,
                mangoGroup.MangoCache,
                rootBank,
                nodeBank,
                vault,
                destAcc,
                amount
            ));

            /// need to close wSOL token account
            if (rootBank == mangoGroup.Tokens[mangoGroup.GetTokenIndex(MarketUtils.WrappedSolMint)].RootBank)
                txBuilder.AddInstruction(TokenProgram.CloseAccount(destAcc, _wallet.Account, _wallet.Account, TokenProgram.ProgramIdKey));

            return txBuilder.CompileMessage();
        }

        /// <summary>
        /// Creates the instructions to create an advanced account
        /// </summary>
        /// <param name="mangoAccount"></param>
        /// <returns></returns>
        private byte[] CreateAdvancedOrdersIx(PublicKey mangoAccount)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();
            var advancedOrders = _mango.DeriveAdvancedOrdersAccountAddress(mangoAccount);

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.InitAdvancedOrders(Constants.DevNetMangoGroup,
                mangoAccount,
                _wallet.Account,
                advancedOrders));

            return txBuilder.CompileMessage();
        }

        /// <summary>
        /// Creates the instructions to close a mango account
        /// </summary>
        /// <param name="mangoAccount"></param>
        /// <returns></returns>
        private byte[] CloseMangoAccountIx(PublicKey mangoAccount)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CloseMangoAccount(Constants.DevNetMangoGroup, mangoAccount, _wallet.Account.PublicKey));

            return txBuilder.CompileMessage();
        }

        /// <summary>
        /// Creates the instructions to create a mango account
        /// </summary>
        /// <param name="mangoAccount"></param>
        /// <returns></returns>
        private byte[] CreateMangoAccountIx(PublicKey mangoAccount, int index)
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            TransactionBuilder txBuilder = new TransactionBuilder()
                .SetFeePayer(_wallet.Account)
                .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
                .AddInstruction(_mango.CreateMangoAccount(Constants.DevNetMangoGroup, mangoAccount, _wallet.Account, (ulong)index))
                .AddInstruction(_mango.AddMangoAccountInfo(Constants.DevNetMangoGroup, mangoAccount, _wallet.Account, "Solnet Test v1"));

            return txBuilder.CompileMessage();
        }

        /// <summary>
        /// Creates and initializes a mango account using the old way.
        /// </summary>
        /// <remarks>
        /// DEPRECATED - THIS WAS DONE FOR TESTING PURPOSES ONLY, PLEASE DON'T USE THIS.
        /// </remarks>
        private void CreateInitMangoAccount()
        {
            var msg = CreateInitMangoAccountIx();

            ExampleHelpers.DecodeAndLogMessage(msg);

            var txBytes = SignAndAssembleTx(msg);

            var sig = _rpcClient.SendTransaction(txBytes);

            ExampleHelpers.PollTx(_rpcClient, sig.Result, Commitment.Confirmed).Wait();
        }

        /// <summary>
        /// Creates instructions to create and initialize a mango account using the old way.
        /// </summary>
        /// <remarks>
        /// DEPRECATED - THIS WAS DONE FOR TESTING PURPOSES ONLY, PLEASE DON'T USE THIS.
        /// </remarks>
        /// <returns>The message to sign.</returns>
        private byte[] CreateInitMangoAccountIx()
        {
            var blockhash = _rpcClient.GetRecentBlockHash();

            var minBalance = _rpcClient.GetMinimumBalanceForRentExemption(MangoAccount.Layout.Length);

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

        /// <summary>
        /// Signs and assembles a transaction with the given message, signing with extra necessary signers in case the transaction creates an account.
        /// </summary>
        /// <param name="msg">The message to sign.</param>
        /// <returns>The serialized assembled transaction.</returns>
        private byte[] SignAndAssembleTx(byte[] msg)
        {
            var signers = new List<Account>()
            {
                _wallet.Account,
            };

            // this is done for more complex cases where we create an OOA or Wrap SOL
            // the account created is added to `_signers` which then gets added to `signers` and also signs the tx
            signers.AddRange(_signers);
            _signers.Clear();

            var signatures = new List<byte[]>();
            foreach (var signer in signers)
            {
                signatures.Add(signer.Sign(msg));
            }
            var tx = Transaction.Populate(Message.Deserialize(msg), signatures);
            return tx.Serialize();
        }
    }
}
