using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class MangoClientTest
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private Queue<string> ResponseQueue;

        [TestInitialize]
        public void SetupTest()
        {
            ResponseQueue = new Queue<string>();
        }

        private void EnqueueResponseFromFile(string pathToFile)
        {
            string data = File.ReadAllText(pathToFile);
            ResponseQueue.Enqueue(data);
        }

        /// <summary>
        /// Setup the JSON RPC test with the request and response data.
        /// </summary>
        /// <param name="responseContent">The response content.</param>
        /// <param name="address">The address parameter for <c>GetAccountInfo</c>.</param>
        /// <param name="commitment">The commitment parameter for the <c>GetAccountInfo</c>.</param>
        /// <param name="network">The network address for the <c>GetAccountInfo</c> request.</param>
        private static Mock<IRpcClient> SetupGetAccountInfo(string responseContent, string address, string network,
            Commitment commitment = Commitment.Finalized)
        {
            var rpcMock = new Mock<IRpcClient>(MockBehavior.Strict) { };
            rpcMock
                .Setup(s => s.NodeAddress)
                .Returns(new Uri(network))
                .Verifiable();
            rpcMock
                .Setup(s => s.GetAccountInfoAsync(
                        It.Is<string>(s1 => s1 == address),
                        It.Is<Commitment>(c => c == commitment),
                        It.IsAny<BinaryEncoding>()))
                .ReturnsAsync( () =>
                {
                    var res = new RequestResult<ResponseValue<AccountInfo>>(
                        new HttpResponseMessage(HttpStatusCode.OK),
                        JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(responseContent, JsonSerializerOptions))
                    {
                        WasRequestSuccessfullyHandled = true
                    };

                    return res;
                })
                .Verifiable();
            return rpcMock;
        }

        /// <summary>
        /// Setup the JSON RPC test with the request and response data.
        /// </summary>
        /// <param name="responseContent">The response content.</param>
        /// <param name="address">The address parameter for <c>GetProgramAccounts</c> request.</param>
        /// <param name="commitment">The commitment parameter for the <c>GetProgramAccounts</c> request.</param>
        /// <param name="dataSize">The datasize for <c>GetProgramAccounts</c> request.</param>
        /// <param name="filters">The filters for the <c>GetProgramAccounts</c> request.</param>
        /// <param name="network">The network address for the <c>GetProgramAccounts</c> request.</param>
        private static Mock<IRpcClient> SetupGetProgramAccounts(string responseContent, string network, string ownerAddress,
            Commitment commitment = Commitment.Finalized)
        {
            var rpcMock = new Mock<IRpcClient>(MockBehavior.Strict) { };
            rpcMock
                .Setup(s => s.NodeAddress)
                .Returns(new Uri(network))
                .Verifiable();
            rpcMock
                .Setup(s => s.GetProgramAccountsAsync(
                    It.Is<string>(s1 => s1 == MangoProgram.DevNetProgramIdKeyV3),
                    It.Is<Commitment>(c => c == commitment),
                    It.Is<int?>(i => i.Value == MangoAccount.Layout.Length),
                    It.Is<List<MemCmp>>(filters => filters.Find(cmp => cmp.Offset == 40).Bytes == ownerAddress)))
                .ReturnsAsync(() =>
                {
                    var res = new RequestResult<List<AccountKeyPair>>(
                        new HttpResponseMessage(HttpStatusCode.OK),
                        JsonSerializer.Deserialize<List<AccountKeyPair>>(responseContent, JsonSerializerOptions))
                    {
                        WasRequestSuccessfullyHandled = true
                    };

                    return res;
                })
                .Verifiable();
            return rpcMock;
        }

        /// <summary>
        /// Setup the JSON RPC test with the request and response data.
        /// </summary>
        /// <param name="responseContent">The response content.</param>
        /// <param name="address">The address parameter for <c>GetAccountInfo</c>.</param>
        /// <param name="commitment">The commitment parameter for the <c>GetAccountInfo</c>.</param>
        /// <param name="network">The network address for the <c>GetAccountInfo</c> request.</param>
        private static Mock<IRpcClient> SetupGetMultipleAccounts(Mock<IRpcClient> rpcMock, string responseContent, List<PublicKey> addresses, 
            Commitment commitment = Commitment.Finalized)
        {
            rpcMock.Setup(s => s.GetMultipleAccountsAsync(
               It.Is<List<string>>(s => addresses.TrueForAll(x => s.Contains(x))),
               It.Is<Commitment>(c => c == commitment)))
            .ReturnsAsync(() =>
            {
                var res = new RequestResult<ResponseValue<List<AccountInfo>>>(
                        new HttpResponseMessage(HttpStatusCode.OK),
                        JsonSerializer.Deserialize<ResponseValue<List<AccountInfo>>>(responseContent, JsonSerializerOptions))
                    {
                        WasRequestSuccessfullyHandled = true
                    };
                return res;
            })
            .Verifiable();
            return rpcMock;
        }

        /// <summary>
        /// Setup the JSON RPC test with the request and response data.
        /// </summary>
        /// <param name="responseContent">The response content.</param>
        /// <param name="address">The address parameter for <c>GetAccountInfo</c>.</param>
        /// <param name="commitment">The commitment parameter for the <c>GetAccountInfo</c>.</param>
        /// <param name="network">The network address for the <c>GetAccountInfo</c> request.</param>
        private Mock<IRpcClient> SetupGetMultipleAccountsFromQueue(Mock<IRpcClient> rpcMock, List<PublicKey> addresses,
            Commitment commitment = Commitment.Finalized)
        {
            rpcMock.Setup(s => s.GetMultipleAccountsAsync(
               It.Is<List<string>>(s => addresses.TrueForAll(x => s.Contains(x))),
               It.Is<Commitment>(c => c == commitment)))
            .ReturnsAsync(() =>
            {
                var res = new RequestResult<ResponseValue<List<AccountInfo>>>(
                        new HttpResponseMessage(HttpStatusCode.OK),
                        JsonSerializer.Deserialize<ResponseValue<List<AccountInfo>>>(ResponseQueue.Dequeue(), JsonSerializerOptions))
                {
                    WasRequestSuccessfullyHandled = true
                };
                return res;
            })
            .Verifiable();
            return rpcMock;
        }

        [TestMethod]
        public void GetMangoGroup()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoGroupAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, Constants.DevNetMangoGroup, "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoGroup = mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);

            Assert.IsNotNull(mangoGroup.ParsedResult);
            Assert.IsTrue(mangoGroup.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoGroup, mangoGroup.ParsedResult.Metadata.DataType);
            Assert.AreEqual("2wNsQMoLPRcZxLxzQqwp1L5mMRr6corysh19NeVscFcG", mangoGroup.ParsedResult.Admin);
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", mangoGroup.ParsedResult.DexProgramId);
            Assert.AreEqual("54PcMYTAZd8uRaYyb3Cwgctcfc1LchGMaqVrmxgr3yVs", mangoGroup.ParsedResult.FeesVault);
            Assert.AreEqual("8TQzMiz2GBPY1bBctpDFA5bSJMz6bN5A4AdoNceaEE9M", mangoGroup.ParsedResult.InsuranceVault);
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", mangoGroup.ParsedResult.MangoCache);
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", mangoGroup.ParsedResult.DexProgramId);
            Assert.AreEqual("9HaBncfZ3JxKLzdPP44RrcXr3nv5FypHC8zn4NPNDkbG", mangoGroup.ParsedResult.MegaSerumVault);
            Assert.AreEqual("7nS8AgndAVYCfSTaXabwqcBWBc3xCLwwcPiMWzuTbfrf", mangoGroup.ParsedResult.SerumVault);
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", mangoGroup.ParsedResult.SignerKey);
            Assert.AreEqual(1UL, mangoGroup.ParsedResult.SignerNonce);
            Assert.AreEqual(10UL, mangoGroup.ParsedResult.ValidInterval);
            Assert.AreEqual(51U, mangoGroup.ParsedResult.NumMangoAccounts);
            Assert.AreEqual(100000U, mangoGroup.ParsedResult.MaxMangoAccounts);
            Assert.AreEqual(13UL, mangoGroup.ParsedResult.NumOracles);
            Assert.AreEqual(15, mangoGroup.ParsedResult.Oracles.Count);
            Assert.AreEqual(0, mangoGroup.ParsedResult.PerpMarketAccounts.Count);
            Assert.AreEqual(15, mangoGroup.ParsedResult.PerpetualMarkets.Count);
            Assert.AreEqual(15, mangoGroup.ParsedResult.SpotMarkets.Count);
            Assert.AreEqual(16, mangoGroup.ParsedResult.Tokens.Count);
        }

        [TestMethod]
        public void GetMangoGroupAndLoadRootBanks()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoGroupAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, Constants.DevNetMangoGroup, "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoGroup = mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);

            Assert.IsNotNull(mangoGroup.ParsedResult);
            Assert.IsTrue(mangoGroup.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoGroup, mangoGroup.ParsedResult.Metadata.DataType);
            Assert.AreEqual(0, mangoGroup.ParsedResult.RootBankAccounts.Count);

            // response to get multiple accounts to load root banks
            string openOrdersResponse = File.ReadAllText("Resources/MangoClient/LoadRootBanksGetMultiple.json");
            // queue responses to get multiple accounts which load the node banks for each root bank
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank1.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank2.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank3.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank4.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank5.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank6.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank7.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank8.json");
            EnqueueResponseFromFile("Resources/MangoClient/LoadRootBanksGetMultipleNodeBank9.json");

            var filteredRootBanks = mangoGroup.ParsedResult.Tokens
                .Where(x => !x.RootBank.Equals(SystemProgram.ProgramIdKey))
                .Select(x => x.RootBank).ToList();

            // setup call to get multiple accounts to load root banks
            SetupGetMultipleAccounts(rpc, openOrdersResponse, filteredRootBanks);
            // setup calls to get multiple accounts to the node banks for each root bank
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("6rkPNJTXF37X6Pf5ct5Y6E91PozpZpZNNU1AGATomKjD") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("4X3nP921qyh6BKJSAohKGNCykSXahFFwg1LxtC993Fai") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("3FPjawEtvrwvwtAetaURTbkkucu9BJofxWZUNPGHJtHg") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("9wkpWmkSUSn9fitLhVh12cLbiDa5Bbhf6ZBGmPtcdMqN") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("JBHBTED3ttzk5u3U24txdjBFadm4Dnohb7g2pwcxU4rx") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("ERkKh9yUKzJ3kkHWhMNd3xGaync11TpzQiDFukEatHEQ") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("2k89sUjCE2ZSm4MPhXM9JV1zFEV2SjgEzvvJN6EsMFWa") });
            SetupGetMultipleAccountsFromQueue(rpc, new List<PublicKey>() { new("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM") });

            var res = mangoGroup.ParsedResult.LoadRootBanks(mangoClient);

            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(16, mangoGroup.ParsedResult.RootBankAccounts.Count);
            Assert.IsNotNull(mangoGroup.ParsedResult.RootBankAccounts[3]);
        }

        [TestMethod]
        public void GetMangoGroupAndLoadPerpMarkets()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoGroupAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, Constants.DevNetMangoGroup, "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoGroup = mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);

            Assert.IsNotNull(mangoGroup.ParsedResult);
            Assert.IsTrue(mangoGroup.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoGroup, mangoGroup.ParsedResult.Metadata.DataType);
            Assert.AreEqual(0, mangoGroup.ParsedResult.PerpMarketAccounts.Count);

            // response to get multiple accounts to load perp markets
            string openOrdersResponse = File.ReadAllText("Resources/MangoClient/LoadPerpMarketsGetMultiple.json");

            var filteredPerpMarkets = mangoGroup.ParsedResult.PerpetualMarkets
                .Where(x => !x.Market.Equals(SystemProgram.ProgramIdKey))
                .Select(x => x.Market).ToList();

            // setup call to get multiple accounts to load root banks
            SetupGetMultipleAccounts(rpc, openOrdersResponse, filteredPerpMarkets);

            var res = mangoGroup.ParsedResult.LoadPerpMarkets(mangoClient);

            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(15, mangoGroup.ParsedResult.PerpMarketAccounts.Count);
            Assert.IsNotNull(mangoGroup.ParsedResult.PerpMarketAccounts[3]);
        }

        [TestMethod]
        public void GetPerpMarkets()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoGroupAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, Constants.DevNetMangoGroup, "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoGroup = mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);

            Assert.IsNotNull(mangoGroup.ParsedResult);
            Assert.IsTrue(mangoGroup.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoGroup, mangoGroup.ParsedResult.Metadata.DataType);
            Assert.AreEqual(0, mangoGroup.ParsedResult.PerpMarketAccounts.Count);

            // response to get multiple accounts to load perp markets
            string openOrdersResponse = File.ReadAllText("Resources/MangoClient/LoadPerpMarketsGetMultiple.json");

            var filteredPerpMarkets = mangoGroup.ParsedResult.PerpetualMarkets
                .Where(x => !x.Market.Equals(SystemProgram.ProgramIdKey))
                .Select(x => x.Market).ToList();

            // setup call to get multiple accounts to load root banks
            SetupGetMultipleAccounts(rpc, openOrdersResponse, filteredPerpMarkets);

            var res = mangoClient.GetPerpMarkets(filteredPerpMarkets);

            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(10, res.ParsedResult.Count);
        }

        [TestMethod]
        public void GetMangoCache()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoCacheAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoCache = mangoClient.GetMangoCache("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z");

            Assert.IsNotNull(mangoCache.ParsedResult);
            Assert.IsTrue(mangoCache.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoCache, mangoCache.ParsedResult.Metadata.DataType);
            Assert.AreEqual(15, mangoCache.ParsedResult.PerpetualMarketCaches.Count);
            Assert.AreEqual(15, mangoCache.ParsedResult.PriceCaches.Count);
            Assert.AreEqual(16, mangoCache.ParsedResult.RootBankCaches.Count);
        }

        [TestMethod]
        public void GetMangoAccount()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoAccountAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoAccount = mangoClient.GetMangoAccount("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s");

            Assert.IsNotNull(mangoAccount.ParsedResult);
            Assert.IsTrue(mangoAccount.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoAccount, mangoAccount.ParsedResult.Metadata.DataType);
            Assert.IsFalse(mangoAccount.ParsedResult.Bankrupt);
            Assert.IsFalse(mangoAccount.ParsedResult.BeingLiquidated);
            Assert.IsFalse(mangoAccount.ParsedResult.NotUpgradeable);
            Assert.AreEqual("\0\0\0\0\0\0\0Solnet Test v1", mangoAccount.ParsedResult.AccountInfo.Trim('\0'));
            Assert.AreEqual(SystemProgram.ProgramIdKey, mangoAccount.ParsedResult.Delegate);
            Assert.AreEqual(Constants.DevNetMangoGroup, mangoAccount.ParsedResult.MangoGroup);
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", mangoAccount.ParsedResult.Owner);
            Assert.AreEqual(16, mangoAccount.ParsedResult.Borrows.Count);
            Assert.AreEqual(16, mangoAccount.ParsedResult.Deposits.Count);
            Assert.AreEqual(15, mangoAccount.ParsedResult.SpotOpenOrders.Count);
            Assert.AreEqual(15, mangoAccount.ParsedResult.PerpetualAccounts.Count);
            Assert.AreEqual(64, mangoAccount.ParsedResult.ClientOrderIds.Count);
            Assert.AreEqual(64, mangoAccount.ParsedResult.OrderIds.Count);
            Assert.AreEqual(64, mangoAccount.ParsedResult.OrderMarket.Count);
            Assert.AreEqual(64, mangoAccount.ParsedResult.OrderSide.Count);
            Assert.AreEqual(0, mangoAccount.ParsedResult.OpenOrdersAccounts.Count);
            Assert.AreEqual(16, mangoAccount.ParsedResult.Deposits.Count);
            Assert.AreEqual(15, mangoAccount.ParsedResult.InMarginBasket.Count);
            Assert.AreEqual(0, mangoAccount.ParsedResult.NumInMarginBasket);
            Assert.AreEqual(0UL, mangoAccount.ParsedResult.MegaSerumAmount);
        }


        [TestMethod]
        public void GetMangoAccounts()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoAccountsProgramAccounts.json");
            var rpc = SetupGetProgramAccounts(response, "https://api.devnet.solana.com", "hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");

            var mangoClient = ClientFactory.GetClient(rpc.Object, programId: MangoProgram.DevNetProgramIdKeyV3);

            var res = mangoClient.GetMangoAccounts("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh");

            Assert.IsNotNull(res);
            Assert.IsTrue(res.WasSuccessful);
        }

            [TestMethod]
        public void GetMangoAccountAndLoadOpenOrders()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoAccountAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoAccount = mangoClient.GetMangoAccount("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s");

            Assert.IsNotNull(mangoAccount.ParsedResult);
            Assert.IsTrue(mangoAccount.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoAccount, mangoAccount.ParsedResult.Metadata.DataType);

            string openOrdersResponse = File.ReadAllText("Resources/MangoClient/LoadOpenOrdersGetMultiple.json");
            var filteredOpenOrders = mangoAccount.ParsedResult.SpotOpenOrders.Where(x => !x.Equals(SystemProgram.ProgramIdKey)).ToList();
            SetupGetMultipleAccounts(rpc, openOrdersResponse, filteredOpenOrders);

            var res = mangoAccount.ParsedResult.LoadOpenOrdersAccounts(rpc.Object);

            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(15, mangoAccount.ParsedResult.OpenOrdersAccounts.Count);
            Assert.IsNotNull(mangoAccount.ParsedResult.OpenOrdersAccounts[3]);
        }

        [TestMethod]
        public void GetAdvancedOrdersAccount()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetAdvancedOrdersAccountAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "AZofyy49f3sY6bt3F1vgMse92eZdFuCM8jkV1USofneA", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var advancedOrders = mangoClient.GetAdvancedOrdersAccount("AZofyy49f3sY6bt3F1vgMse92eZdFuCM8jkV1USofneA");

            Assert.IsNotNull(advancedOrders.ParsedResult);
            Assert.IsTrue(advancedOrders.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.AdvancedOrders, advancedOrders.ParsedResult.Metadata.DataType);
            Assert.AreEqual(0, advancedOrders.ParsedResult.AdvancedOrders.Count);
        }

        [TestMethod]
        public void GetAdvancedOrdersAccountWithOrders()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetAdvancedOrdersAccountInfoWithOrders.json");
            var rpc = SetupGetAccountInfo(response, "AZofyy49f3sY6bt3F1vgMse92eZdFuCM8jkV1USofneA", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var advancedOrders = mangoClient.GetAdvancedOrdersAccount("AZofyy49f3sY6bt3F1vgMse92eZdFuCM8jkV1USofneA");

            Assert.IsNotNull(advancedOrders.ParsedResult);
            Assert.IsTrue(advancedOrders.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.AdvancedOrders, advancedOrders.ParsedResult.Metadata.DataType);
            Assert.AreEqual(1, advancedOrders.ParsedResult.AdvancedOrders.Count);
        }

        [TestMethod]
        public void GetRootBank()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetRootBankAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "CY4nMV9huW5KCYFxWChrmoLwGCsZiXoiREeo2PMrBm5o", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var rootBank = mangoClient.GetRootBank("CY4nMV9huW5KCYFxWChrmoLwGCsZiXoiREeo2PMrBm5o");

            Assert.IsNotNull(rootBank.ParsedResult);
            Assert.IsTrue(rootBank.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.RootBank, rootBank.ParsedResult.Metadata.DataType);
            Assert.AreEqual(1017392.0746093992799856664533m, rootBank.ParsedResult.BorrowIndex.ToDecimal());
            Assert.AreEqual(1013740.4744292109689389747018m, rootBank.ParsedResult.DepositIndex.ToDecimal());
            Assert.AreEqual(0.0599999999999987210230756318m, rootBank.ParsedResult.OptimalRate.ToDecimal());
            Assert.AreEqual(0.6999999999999992894572642399m, rootBank.ParsedResult.OptimalUtilization.ToDecimal());
            Assert.AreEqual(1.5m, rootBank.ParsedResult.MaxRate.ToDecimal());
            Assert.AreEqual(1UL, rootBank.ParsedResult.NumNodeBanks);
            Assert.AreEqual(7, rootBank.ParsedResult.NodeBanks.Count);
            Assert.AreEqual(0, rootBank.ParsedResult.NodeBankAccounts.Count);
        }

        [TestMethod]
        public void GetNodeBank()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetNodeBankAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "6rkPNJTXF37X6Pf5ct5Y6E91PozpZpZNNU1AGATomKjD", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var nodeBank = mangoClient.GetNodeBank("6rkPNJTXF37X6Pf5ct5Y6E91PozpZpZNNU1AGATomKjD");

            Assert.IsNotNull(nodeBank.ParsedResult);
            Assert.IsTrue(nodeBank.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.NodeBank, nodeBank.ParsedResult.Metadata.DataType);
            Assert.AreEqual(254049.62161569644917236132642m, nodeBank.ParsedResult.Borrows.ToDecimal());
            Assert.AreEqual(337344.10233830148964528916622m, nodeBank.ParsedResult.Deposits.ToDecimal());
            Assert.AreEqual("79Rz9FwjTYSGMbpPBbQMT6kEmqhuGvhqpCPEoALJGmsb", nodeBank.ParsedResult.Vault);
        }

        [TestMethod]
        public void GetOrderBook()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetMangoGroupAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, Constants.DevNetMangoGroup, "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var mangoGroup = mangoClient.GetMangoGroup(Constants.DevNetMangoGroup);

            Assert.IsNotNull(mangoGroup.ParsedResult);
            Assert.IsTrue(mangoGroup.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoGroup, mangoGroup.ParsedResult.Metadata.DataType);
            Assert.AreEqual(0, mangoGroup.ParsedResult.PerpMarketAccounts.Count);

            // response to get multiple accounts to load root banks
            string openOrdersResponse = File.ReadAllText("Resources/MangoClient/LoadPerpMarketsGetMultiple.json");

            var filteredPerpMarkets = mangoGroup.ParsedResult.PerpetualMarkets
                .Where(x => !x.Market.Equals(SystemProgram.ProgramIdKey))
                .Select(x => x.Market).ToList();

            // setup call to get multiple accounts to load perp markets
            SetupGetMultipleAccounts(rpc, openOrdersResponse, filteredPerpMarkets);

            var res = mangoGroup.ParsedResult.LoadPerpMarkets(mangoClient);

            Assert.IsTrue(res.WasSuccessful);
            Assert.AreEqual(15, mangoGroup.ParsedResult.PerpMarketAccounts.Count);
            Assert.IsNotNull(mangoGroup.ParsedResult.PerpMarketAccounts[3]);

            string obResponse = File.ReadAllText("Resources/MangoClient/GetOrderBookMultipleAccounts.json");

            SetupGetMultipleAccounts(rpc, obResponse, new List<PublicKey> { new("4Z9xHcCUMY9QLevHu3JpzxnwiHzzaQACMJERZ1XVJcSa"), new("5Zpfa8VbFKBJQFueomXz82EjbbtP6nFFQmBkHPCxfKpb") });

            var ob = mangoClient.GetOrderBook(mangoGroup.ParsedResult.PerpMarketAccounts[0]);

            Assert.IsNotNull(ob);
            Assert.IsTrue(ob.ParsedResult.Asks.Metadata.IsInitialized);
            Assert.AreEqual(DataType.Asks, ob.ParsedResult.Asks.Metadata.DataType);
            Assert.IsTrue(ob.ParsedResult.Bids.Metadata.IsInitialized);
            Assert.AreEqual(DataType.Bids, ob.ParsedResult.Bids.Metadata.DataType);
            var bids = ob.ParsedResult.GetBids();
            var asks = ob.ParsedResult.GetAsks();
            Assert.AreEqual(9, bids.Count);
            Assert.AreEqual(8, asks.Count);
        }

        [TestMethod]
        public void GetEventQueue()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetEventQueueAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "uaUCSQejWYrDeYSuvn4As4kaCwJ2rLnRQSsSjY3ogZk", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var eq = mangoClient.GetEventQueue("uaUCSQejWYrDeYSuvn4As4kaCwJ2rLnRQSsSjY3ogZk");

            Assert.IsNotNull(eq);
            Assert.IsTrue(eq.ParsedResult.Header.Metadata.IsInitialized);
            Assert.AreEqual(DataType.EventQueue, eq.ParsedResult.Header.Metadata.DataType);
            Assert.AreEqual(256, eq.ParsedResult.Events.Count);
            Assert.AreEqual(0u, eq.ParsedResult.Header.Count);
            Assert.AreEqual(93u, eq.ParsedResult.Header.Head);
            Assert.AreEqual(93u, eq.ParsedResult.Header.NextSequenceNumber);
        }

        [TestMethod]
        public void GetOrderBookSide()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetOrderBookSideAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "4Z9xHcCUMY9QLevHu3JpzxnwiHzzaQACMJERZ1XVJcSa", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var obs = mangoClient.GetOrderBookSide("4Z9xHcCUMY9QLevHu3JpzxnwiHzzaQACMJERZ1XVJcSa");

            Assert.IsNotNull(obs);
            Assert.IsTrue(obs.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.Asks, obs.ParsedResult.Metadata.DataType);
            var asks = obs.ParsedResult.GetOrders();
            Assert.AreEqual(8, asks.Count);
        }

        [TestMethod]
        public void GetPerpMarket()
        {
            string response = File.ReadAllText("Resources/MangoClient/GetPerpMArketAccountInfo.json");
            var rpc = SetupGetAccountInfo(response, "98wPi7vBkiJ1sXLPipQEjrgHYcMBcNUsg9avTyWUi26j", "https://api.devnet.solana.com");

            var mangoClient = ClientFactory.GetClient(rpc.Object);

            var pm = mangoClient.GetPerpMarket("98wPi7vBkiJ1sXLPipQEjrgHYcMBcNUsg9avTyWUi26j");

            Assert.IsNotNull(pm);
            Assert.IsTrue(pm.ParsedResult.Metadata.IsInitialized);
            Assert.AreEqual(DataType.PerpMarket, pm.ParsedResult.Metadata.DataType);
            Assert.AreEqual(Constants.DevNetMangoGroup, pm.ParsedResult.MangoGroup);
        }
    }
}
