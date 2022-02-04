using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.IO;
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
            Assert.AreEqual(13, mangoGroup.ParsedResult.Oracles.Count);
            Assert.AreEqual(0, mangoGroup.ParsedResult.PerpMarketAccounts.Count);
            Assert.AreEqual(15, mangoGroup.ParsedResult.PerpetualMarkets.Count);
            Assert.AreEqual(15, mangoGroup.ParsedResult.SpotMarkets.Count);
            Assert.AreEqual(16, mangoGroup.ParsedResult.Tokens.Count);
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
            Assert.AreEqual(DataType.MangoCache, mangoCache.ParsedResult.Metadata.DataType); // finish test once merged due to incorrect decoding
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
    }
}
