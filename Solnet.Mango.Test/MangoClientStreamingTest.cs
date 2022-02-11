using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Solnet.Mango.Models;
using Solnet.Mango.Models.Events;
using Solnet.Mango.Models.Matching;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class MangoClientStreamingTest
    {
        private static readonly string MainNetUrl = "https://api.mainnet-beta.solana.com/";
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private WebSocketState _wsState;
        private Mock<SubscriptionState> _subscriptionStateMock;
        private Action<SubscriptionState, ResponseValue<AccountInfo>> _action;

        private Mock<IStreamingRpcClient> MultipleNotificationsStreamingClientTestSetup<T>(
            out Action<Subscription, T, ulong> action, Action<T> resultCaptureCallback,
            string network, Commitment commitment = Commitment.Finalized)
        {
            Mock<Action<Subscription, T, ulong>> actionMock = new();
            actionMock
                .Setup(_ => _(It.IsAny<Subscription>(), It.IsAny<T>(), It.IsAny<ulong>()))
                .Callback<Subscription, T, ulong>((sub, notification, slot) =>
                {
                    resultCaptureCallback(notification);
                });
            action = actionMock.Object;

            _subscriptionStateMock = new Mock<SubscriptionState>(MockBehavior.Strict);
            Mock<IStreamingRpcClient> streamingRpcMock = new(MockBehavior.Strict);
            streamingRpcMock
                .Setup(s => s.NodeAddress)
                .Returns(new Uri(network))
                .Verifiable();
            streamingRpcMock
                .Setup(s => s.State)
                .Returns(() => _wsState)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.ConnectAsync())
                .Callback(() =>
                {
                    _wsState = WebSocketState.Open;
                })
                .Returns(() => null)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.DisconnectAsync())
                .Callback(() =>
                {
                    _wsState = WebSocketState.Closed;
                })
                .Returns(() => null)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.SubscribeAccountInfoAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<SubscriptionState, ResponseValue<AccountInfo>>>(),
                    It.Is<Commitment>(c => c == commitment)))
                .Callback<string, Action<SubscriptionState, ResponseValue<AccountInfo>>, Commitment>(
                    (_, notificationAction, _) =>
                    {
                        _action = notificationAction;
                    })
                .ReturnsAsync(() => _subscriptionStateMock.Object)
                .Verifiable();
            return streamingRpcMock;
        }

        [TestInitialize]
        public void Setup()
        {
            _wsState = WebSocketState.None;
        }



        [TestMethod]
        public void Connect()
        {
            Mock<IStreamingRpcClient> streamingRpcMock = new(MockBehavior.Strict);
            streamingRpcMock
                .Setup(s => s.NodeAddress)
                .Returns(new Uri(MainNetUrl))
                .Verifiable();
            streamingRpcMock
                .Setup(s => s.State)
                .Returns(() => _wsState)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.ConnectAsync())
                .Callback(() =>
                {
                    _wsState = WebSocketState.Open;
                })
                .Returns(() => null)
                .Verifiable();

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);

            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, _wsState);

            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, _wsState);
        }

        [TestMethod]
        public void Disconnect()
        {
            Mock<IStreamingRpcClient> streamingRpcMock = new(MockBehavior.Strict);
            streamingRpcMock
                .Setup(s => s.NodeAddress)
                .Returns(new Uri(MainNetUrl))
                .Verifiable();
            streamingRpcMock
                .Setup(s => s.State)
                .Returns(() => _wsState)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.ConnectAsync())
                .Callback(() =>
                {
                    _wsState = WebSocketState.Open;
                })
                .Returns(() => null)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.DisconnectAsync())
                .Callback(() =>
                {
                    _wsState = WebSocketState.Closed;
                })
                .Returns(() => null)
                .Verifiable();

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            sut.ConnectAsync();
            sut.DisconnectAsync();

            Assert.AreEqual(WebSocketState.Closed, _wsState);
        }

        [TestMethod]
        public void DisconnectNull()
        {
            Mock<IStreamingRpcClient> streamingRpcMock = new(MockBehavior.Strict);
            streamingRpcMock
                .Setup(s => s.NodeAddress)
                .Returns(new Uri(MainNetUrl))
                .Verifiable();
            streamingRpcMock
                .Setup(s => s.State)
                .Returns(() => _wsState)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.ConnectAsync())
                .Callback(() =>
                {
                    _wsState = WebSocketState.Open;
                })
                .Returns(() => null)
                .Verifiable();

            streamingRpcMock
                .Setup(s => s.DisconnectAsync())
                .Callback(() =>
                {
                    _wsState = WebSocketState.Closed;
                })
                .Returns(() => null)
                .Verifiable();

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            sut.DisconnectAsync();

            Assert.AreEqual(WebSocketState.None, _wsState);
        }

        [TestMethod]
        public void SubscribeEventQueue()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/Streaming/SubscribeEventQueueFirstAccountInfoNotification.json");
            string secondAccountInfoNotification =
                File.ReadAllText("Resources/Streaming/SubscribeEventQueueSecondAccountInfoNotification.json");
            EventQueue resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, EventQueue, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeEventQueue(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(sub);
            Assert.IsNotNull(resultNotification);
            Assert.AreEqual(256, resultNotification.Events.Count);
            Assert.AreEqual(0u, resultNotification.Header.Count);
            Assert.AreEqual(11u, resultNotification.Header.Head);
            Assert.AreEqual(508683u, resultNotification.Header.NextSequenceNumber);
            Assert.AreEqual(DataType.EventQueue, resultNotification.Header.Metadata.DataType);
            Assert.IsTrue(resultNotification.Header.Metadata.IsInitialized);

            notificationContent = JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(secondAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(resultNotification);
            Assert.AreEqual(0, resultNotification.Events.Count);
            Assert.AreEqual(0u, resultNotification.Header.Count);
            Assert.AreEqual(11u, resultNotification.Header.Head);
            Assert.AreEqual(508683u, resultNotification.Header.NextSequenceNumber);
            Assert.AreEqual(DataType.EventQueue, resultNotification.Header.Metadata.DataType);
            Assert.IsTrue(resultNotification.Header.Metadata.IsInitialized);
        }

        [TestMethod]
        public void SubscribeEventQueueNewEvents()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/Streaming/SubscribeEventQueueFirstAccountInfoNotificationNewEvents.json");
            string secondAccountInfoNotification =
                File.ReadAllText("Resources/Streaming/SubscribeEventQueueSecondAccountInfoNotificationNewEvents.json");
            EventQueue resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, EventQueue, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeEventQueue(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(sub);
            Assert.IsNotNull(resultNotification);
            Assert.AreEqual(256, resultNotification.Events.Count);
            Assert.AreEqual(0u, resultNotification.Header.Count);
            Assert.AreEqual(73u, resultNotification.Header.Head);
            Assert.AreEqual(508745u, resultNotification.Header.NextSequenceNumber);
            Assert.AreEqual(DataType.EventQueue, resultNotification.Header.Metadata.DataType);
            Assert.IsTrue(resultNotification.Header.Metadata.IsInitialized);

            notificationContent = JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(secondAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(resultNotification);
            Assert.AreEqual(3, resultNotification.Events.Count);
            Assert.AreEqual(0u, resultNotification.Header.Count);
            Assert.AreEqual(76u, resultNotification.Header.Head);
            Assert.AreEqual(508748u, resultNotification.Header.NextSequenceNumber);
            Assert.AreEqual(DataType.EventQueue, resultNotification.Header.Metadata.DataType);
            Assert.IsTrue(resultNotification.Header.Metadata.IsInitialized);
        }

        [TestMethod]
        public void SubscribeEventQueueWithLiquidations()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/Streaming/SubscribeEventQueueWithLiquidations.json");
            EventQueue resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, EventQueue, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeEventQueue(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(sub);
            Assert.IsNotNull(resultNotification);
            Assert.AreEqual(256, resultNotification.Events.Count);
            Assert.AreEqual(0u, resultNotification.Header.Count);
            Assert.AreEqual(86u, resultNotification.Header.Head);
            Assert.AreEqual(107350u, resultNotification.Header.NextSequenceNumber);
            Assert.AreEqual(DataType.EventQueue, resultNotification.Header.Metadata.DataType);
            Assert.IsTrue(resultNotification.Header.Metadata.IsInitialized);
        }

        [TestMethod]
        public void UnsubscribeEventQueue()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/Streaming/SubscribeEventQueueFirstAccountInfoNotification.json");
            string secondAccountInfoNotification =
                File.ReadAllText("Resources/Streaming/SubscribeEventQueueSecondAccountInfoNotification.json");
            EventQueue resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, EventQueue, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeEventQueue(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(sub);
            Assert.IsNotNull(resultNotification);
            Assert.AreEqual(256, resultNotification.Events.Count);
            Assert.AreEqual(0u, resultNotification.Header.Count);
            Assert.AreEqual(11u, resultNotification.Header.Head);
            Assert.AreEqual(508683u, resultNotification.Header.NextSequenceNumber);
            Assert.AreEqual(DataType.EventQueue, resultNotification.Header.Metadata.DataType);
            Assert.IsTrue(resultNotification.Header.Metadata.IsInitialized);

            notificationContent = JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(secondAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(resultNotification);
            Assert.AreEqual(0, resultNotification.Events.Count);
            Assert.AreEqual(0u, resultNotification.Header.Count);
            Assert.AreEqual(11u, resultNotification.Header.Head);
            Assert.AreEqual(508683u, resultNotification.Header.NextSequenceNumber);
            Assert.AreEqual(DataType.EventQueue, resultNotification.Header.Metadata.DataType);
            Assert.IsTrue(resultNotification.Header.Metadata.IsInitialized);

            streamingRpcMock
                .Setup(s => s.UnsubscribeAsync(It.IsAny<SubscriptionState>()))
                .Callback<SubscriptionState>(state =>
                {
                    Assert.AreEqual(sub.SubscriptionState, state);
                })
                .Returns(() => null)
                .Verifiable();

            sut.UnsubscribeEventQueue("31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            streamingRpcMock.Verify(
                s => s.UnsubscribeAsync(
                    It.Is<SubscriptionState>(ss => ss.Channel == sub.SubscriptionState.Channel)), Times.Once);
        }

        [TestMethod]
        public void UnsubscribeMangoAccount()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/MangoClient/GetMangoAccountAccountInfo.json");
            MangoAccount resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, MangoAccount, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeMangoAccount(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(resultNotification);
            Assert.IsTrue(resultNotification.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoAccount, resultNotification.Metadata.DataType);
            Assert.IsFalse(resultNotification.Bankrupt);
            Assert.IsFalse(resultNotification.BeingLiquidated);
            Assert.IsFalse(resultNotification.NotUpgradeable);
            Assert.AreEqual("Solnet Test v1", resultNotification.AccountInfo);
            Assert.AreEqual(SystemProgram.ProgramIdKey, resultNotification.Delegate);
            Assert.AreEqual(Constants.DevNetMangoGroup, resultNotification.MangoGroup);
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", resultNotification.Owner);
            Assert.AreEqual(16, resultNotification.Borrows.Count);
            Assert.AreEqual(16, resultNotification.Deposits.Count);
            Assert.AreEqual(15, resultNotification.SpotOpenOrders.Count);
            Assert.AreEqual(15, resultNotification.PerpetualAccounts.Count);
            Assert.AreEqual(64, resultNotification.ClientOrderIds.Count);
            Assert.AreEqual(64, resultNotification.OrderIds.Count);
            Assert.AreEqual(64, resultNotification.OrderMarket.Count);
            Assert.AreEqual(64, resultNotification.OrderSide.Count);
            Assert.AreEqual(0, resultNotification.OpenOrdersAccounts.Count);
            Assert.AreEqual(16, resultNotification.Deposits.Count);
            Assert.AreEqual(15, resultNotification.InMarginBasket.Count);
            Assert.AreEqual(0, resultNotification.NumInMarginBasket);
            Assert.AreEqual(0UL, resultNotification.MegaSerumAmount);

            streamingRpcMock
                .Setup(s => s.UnsubscribeAsync(It.IsAny<SubscriptionState>()))
                .Callback<SubscriptionState>(state =>
                {
                    Assert.AreEqual(sub.SubscriptionState, state);
                })
                .Returns(() => null)
                .Verifiable();

            sut.UnsubscribeMangoAccount("31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            streamingRpcMock.Verify(
                s => s.UnsubscribeAsync(
                    It.Is<SubscriptionState>(ss => ss.Channel == sub.SubscriptionState.Channel)), Times.Once);
        }

        [TestMethod]
        public void SubscribeMangoAccount()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/MangoClient/GetMangoAccountAccountInfo.json");
            MangoAccount resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, MangoAccount, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeMangoAccount(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(resultNotification);
            Assert.IsTrue(resultNotification.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoAccount, resultNotification.Metadata.DataType);
            Assert.IsFalse(resultNotification.Bankrupt);
            Assert.IsFalse(resultNotification.BeingLiquidated);
            Assert.IsFalse(resultNotification.NotUpgradeable);
            Assert.AreEqual("Solnet Test v1", resultNotification.AccountInfo);
            Assert.AreEqual(SystemProgram.ProgramIdKey, resultNotification.Delegate);
            Assert.AreEqual(Constants.DevNetMangoGroup, resultNotification.MangoGroup);
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", resultNotification.Owner);
            Assert.AreEqual(16, resultNotification.Borrows.Count);
            Assert.AreEqual(16, resultNotification.Deposits.Count);
            Assert.AreEqual(15, resultNotification.SpotOpenOrders.Count);
            Assert.AreEqual(15, resultNotification.PerpetualAccounts.Count);
            Assert.AreEqual(64, resultNotification.ClientOrderIds.Count);
            Assert.AreEqual(64, resultNotification.OrderIds.Count);
            Assert.AreEqual(64, resultNotification.OrderMarket.Count);
            Assert.AreEqual(64, resultNotification.OrderSide.Count);
            Assert.AreEqual(0, resultNotification.OpenOrdersAccounts.Count);
            Assert.AreEqual(16, resultNotification.Deposits.Count);
            Assert.AreEqual(15, resultNotification.InMarginBasket.Count);
            Assert.AreEqual(0, resultNotification.NumInMarginBasket);
            Assert.AreEqual(0UL, resultNotification.MegaSerumAmount);
        }

        [TestMethod]
        public void SubscribeMangoCache()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/MangoClient/GetMangoCacheAccountInfo.json");
            MangoCache resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, MangoCache, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeMangoCache(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(resultNotification);
            Assert.IsTrue(resultNotification.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoCache, resultNotification.Metadata.DataType);
            Assert.AreEqual(15, resultNotification.PerpetualMarketCaches.Count);
            Assert.AreEqual(15, resultNotification.PriceCaches.Count);
            Assert.AreEqual(16, resultNotification.RootBankCaches.Count);
        }

        [TestMethod]
        public void UnsubscribeMangoCache()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/MangoClient/GetMangoCacheAccountInfo.json");
            MangoCache resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, MangoCache, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeMangoCache(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);

            Assert.IsNotNull(resultNotification);
            Assert.IsTrue(resultNotification.Metadata.IsInitialized);
            Assert.AreEqual(DataType.MangoCache, resultNotification.Metadata.DataType);
            Assert.AreEqual(15, resultNotification.PerpetualMarketCaches.Count);
            Assert.AreEqual(15, resultNotification.PriceCaches.Count);
            Assert.AreEqual(16, resultNotification.RootBankCaches.Count);

            streamingRpcMock
                .Setup(s => s.UnsubscribeAsync(It.IsAny<SubscriptionState>()))
                .Callback<SubscriptionState>(state =>
                {
                    Assert.AreEqual(sub.SubscriptionState, state);
                })
                .Returns(() => null)
                .Verifiable();

            sut.UnsubscribeMangoCache("31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            streamingRpcMock.Verify(
                s => s.UnsubscribeAsync(
                    It.Is<SubscriptionState>(ss => ss.Channel == sub.SubscriptionState.Channel)), Times.Once);
        }

        [TestMethod]
        public void SubscribeOrderBookSide()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/MangoClient/GetOrderBookSideAccountInfo.json");
            OrderBookSide resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, OrderBookSide, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeOrderBookSide(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);
            Assert.IsNotNull(resultNotification);
            Assert.IsTrue(resultNotification.Metadata.IsInitialized);
            Assert.AreEqual(DataType.Asks, resultNotification.Metadata.DataType);
        }

        [TestMethod]
        public void UnsubscribeOrderBookSide()
        {
            string firstAccountInfoNotification =
                File.ReadAllText("Resources/MangoClient/GetOrderBookSideAccountInfo.json");
            OrderBookSide resultNotification = null;
            Mock<IStreamingRpcClient> streamingRpcMock = MultipleNotificationsStreamingClientTestSetup(
                out Action<Subscription, OrderBookSide, ulong> action,
                (x) =>
                {
                    resultNotification = x;
                },
                "https://api.mainnet-beta.solana.com");

            var rpcClient = Rpc.ClientFactory.GetClient(Cluster.MainNet);

            MangoClient sut = new(rpcClient: rpcClient, streamingRpcClient: streamingRpcMock.Object);
            Assert.IsNotNull(sut.StreamingRpcClient);
            Assert.AreEqual(MainNetUrl, sut.NodeAddress.ToString());
            sut.ConnectAsync();

            Assert.AreEqual(WebSocketState.Open, sut.State);

            Subscription sub = sut.SubscribeOrderBookSide(action, "31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            ResponseValue<AccountInfo> notificationContent =
                JsonSerializer.Deserialize<ResponseValue<AccountInfo>>(firstAccountInfoNotification,
                    JsonSerializerOptions);
            _action(_subscriptionStateMock.Object, notificationContent);

            Assert.IsNotNull(resultNotification);
            Assert.IsTrue(resultNotification.Metadata.IsInitialized);
            Assert.AreEqual(DataType.Asks, resultNotification.Metadata.DataType);

            streamingRpcMock
                .Setup(s => s.UnsubscribeAsync(It.IsAny<SubscriptionState>()))
                .Callback<SubscriptionState>(state =>
                {
                    Assert.AreEqual(sub.SubscriptionState, state);
                })
                .Returns(() => null)
                .Verifiable();

            sut.UnsubscribeOrderBookSide("31cKs646dt1YkA3zPyxZ7rUAkxTBz279w4XEobFXcAKP");

            streamingRpcMock.Verify(
                s => s.UnsubscribeAsync(
                    It.Is<SubscriptionState>(ss => ss.Channel == sub.SubscriptionState.Channel)), Times.Once);
        }
    }
}
