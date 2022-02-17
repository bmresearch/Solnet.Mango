using Solnet.Mango.Models;
using Solnet.Mango.Models.Banks;
using Solnet.Mango.Models.Events;
using Solnet.Mango.Models.Matching;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Solnet.Mango
{
    /// <summary>
    /// Specifies functionality for the Mango client.
    /// </summary>
    public interface IMangoClient
    {
        /// <summary>
        /// The websocket connection state.
        /// </summary>
        WebSocketState State { get; }

        /// <summary>
        /// The <see cref="IRpcClient"/> instance.
        /// </summary>
        IRpcClient RpcClient { get; }

        /// <summary>
        /// The <see cref="IStreamingRpcClient"/> instance.
        /// </summary>
        IStreamingRpcClient StreamingRpcClient { get; }

        /// <summary>
        /// The program id.
        /// </summary>
        PublicKey ProgramId { get; }

        /// <summary>
        /// The statistics of the current websocket connection.
        /// </summary>
        IConnectionStatistics ConnectionStatistics { get; }

        /// <summary>
        /// The cluster the client is connected to.
        /// </summary>
        Uri NodeAddress { get; }

        /// <summary>
        /// Connect to the <see cref="StreamingRpcClient"/> for data streaming. This is an asynchronous operation.
        /// </summary>
        /// <returns>A task which may connect to the Rpc.</returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the <see cref="StreamingRpcClient"/> for data streaming. This is an asynchronous operation.
        /// </summary>
        /// <returns>A task which may connect to the Rpc.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Gets the given <see cref="MangoGroup"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="MangoGroup"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="MangoGroup"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<MangoGroup>> GetMangoGroupAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="MangoGroup"/>.
        /// </summary>
        /// <param name="account">The <see cref="MangoGroup"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="MangoGroup"/>s.</returns>
        AccountResultWrapper<MangoGroup> GetMangoGroup(string account, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="MangoCache"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="MangoCache"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="MangoCache"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<MangoCache>> GetMangoCacheAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="MangoCache"/>.
        /// </summary>
        /// <param name="account">The <see cref="MangoCache"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="MangoCache"/>s.</returns>
        AccountResultWrapper<MangoCache> GetMangoCache(string account, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="RootBank"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="RootBank"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="RootBank"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<RootBank>> GetRootBankAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="RootBank"/>.
        /// </summary>
        /// <param name="account">The <see cref="RootBank"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="RootBank"/>.</returns>
        AccountResultWrapper<RootBank> GetRootBank(string account, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the <see cref="RootBank"/>s with the given keys. This is an asynchronous operation.
        /// </summary>
        /// <param name="rootBanks">The root bank public keys.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="RootBank"/>s or null in case an error occurred.</returns>
        Task<MultipleAccountsResultWrapper<List<RootBank>>> GetRootBanksAsync(List<PublicKey> rootBanks,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the <see cref="RootBank"/>s with the given keys.
        /// </summary>
        /// <param name="rootBanks">The root bank public keys.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="RootBank"/>s.</returns>
        MultipleAccountsResultWrapper<List<RootBank>> GetRootBanks(List<PublicKey> rootBanks, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="NodeBank"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="NodeBank"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="NodeBank"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<NodeBank>> GetNodeBankAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the <see cref="NodeBank"/>s with the given keys. This is an asynchronous operation.
        /// </summary>
        /// <param name="nodeBanks">The node bank public keys.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="NodeBank"/>s or null in case an error occurred.</returns>
        Task<MultipleAccountsResultWrapper<List<NodeBank>>> GetNodeBanksAsync(List<PublicKey> nodeBanks,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the <see cref="NodeBank"/>s with the given keys.
        /// </summary>
        /// <param name="nodeBanks">The node bank public keys.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="NodeBank"/>s.</returns>
        MultipleAccountsResultWrapper<List<NodeBank>> GetNodeBanks(List<PublicKey> nodeBanks, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="NodeBank"/>.
        /// </summary>
        /// <param name="account">The <see cref="NodeBank"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="NodeBank"/>s.</returns>
        AccountResultWrapper<NodeBank> GetNodeBank(string account, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="PerpMarket"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="PerpMarket"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="PerpMarket"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<PerpMarket>> GetPerpMarketAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="PerpMarket"/>.
        /// </summary>
        /// <param name="account">The <see cref="PerpMarket"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="PerpMarket"/>s.</returns>
        AccountResultWrapper<PerpMarket> GetPerpMarket(string account, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="OrderBookSide"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="OrderBookSide"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="OrderBookSide"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<OrderBookSide>> GetOrderBookSideAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="OrderBookSide"/>.
        /// </summary>
        /// <param name="account">The <see cref="OrderBookSide"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="OrderBookSide"/>s.</returns>
        AccountResultWrapper<OrderBookSide> GetOrderBookSide(string account, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="OrderBook"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="perpMarket">The <see cref="PerpMarket"/>.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="OrderBook"/>s or null in case an error occurred.</returns>
        Task<MultipleAccountsResultWrapper<OrderBook>> GetOrderBookAsync(PerpMarket perpMarket,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="OrderBook"/>.
        /// </summary>
        /// <param name="perpMarket">The <see cref="PerpMarket"/>.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="OrderBook"/>s.</returns>
        MultipleAccountsResultWrapper<OrderBook> GetOrderBook(PerpMarket perpMarket, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="EventQueue"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="eventQueueAddress">The <see cref="EventQueue"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="EventQueue"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<EventQueue>> GetEventQueueAsync(string eventQueueAddress,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="EventQueue"/>.
        /// </summary>
        /// <param name="eventQueueAddress">The <see cref="EventQueue"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="EventQueue"/>s.</returns>
        AccountResultWrapper<EventQueue> GetEventQueue(string eventQueueAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given Mango Account. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The account's public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="MangoAccount"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<MangoAccount>> GetMangoAccountAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given Mango Account.
        /// </summary>
        /// <param name="account">The account's public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="MangoAccount"/>s or null in case an error occurred.</returns>
        AccountResultWrapper<MangoAccount> GetMangoAccount(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given Advanced Orders Account. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The account's public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="AdvancedOrdersAccount"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<AdvancedOrdersAccount>> GetAdvancedOrdersAccountAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given Advanced Orders Account.
        /// </summary>
        /// <param name="account">The account's public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="AdvancedOrdersAccount"/>s or null in case an error occurred.</returns>
        AccountResultWrapper<AdvancedOrdersAccount> GetAdvancedOrdersAccount(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the Mango accounts for the given owner. This is an asynchronous operation.
        /// </summary>
        /// <param name="ownerAccount">The owner account's public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="MangoAccount"/>s or null in case an error occurred.</returns>
        Task<ProgramAccountsResultWrapper<List<MangoAccount>>> GetMangoAccountsAsync(string ownerAccount,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the Mango accounts for the given owner.
        /// </summary>
        /// <param name="ownerAccount">The owner account's public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="MangoAccount"/>s or null in case an error occurred.</returns>
        ProgramAccountsResultWrapper<List<MangoAccount>> GetMangoAccounts(string ownerAccount,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the perpetual markets for the given Mango group. This is an asynchronous operation.
        /// </summary>
        /// <param name="perpMarkets">The perp markets.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="PerpMarket"/>s or null in case an error occurred.</returns>
        Task<MultipleAccountsResultWrapper<List<PerpMarket>>> GetPerpMarketsAsync(List<PublicKey> perpMarkets,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the perpetual markets for the given Mango group.
        /// </summary>
        /// <param name="perpMarkets">The perp markets.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="PerpMarket"/>s or null in case an error occurred.</returns>
        MultipleAccountsResultWrapper<List<PerpMarket>> GetPerpMarkets(List<PublicKey> perpMarkets,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given referrer memory. This is an asynchronous operation.
        /// </summary>
        /// <param name="referrerMemory">The referrer memory.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="ReferrerMemoryAccount"/> or null in case an error occurred.</returns>
        Task<AccountResultWrapper<ReferrerMemoryAccount>> GetReferrerMemoryAccountAsync(PublicKey referrerMemory, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given referrer memory.
        /// </summary>
        /// <param name="referrerMemory">The referrer memory.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="ReferrerMemoryAccount"/> or null in case an error occurred.</returns>
        AccountResultWrapper<ReferrerMemoryAccount> GetReferrerMemoryAccount(PublicKey referrerMemory, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given referrer id record. This is an asynchronous operation.
        /// </summary>
        /// <param name="referrerIdRecord">The referrer id record.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="ReferrerIdRecordAccount"/> or null in case an error occurred.</returns>
        Task<AccountResultWrapper<ReferrerIdRecordAccount>> GetReferrerIdRecordAccountAsync(PublicKey referrerIdRecord, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given referrer id record.
        /// </summary>
        /// <param name="referrerIdRecord">The referrer id record.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The <see cref="ReferrerIdRecordAccount"/> or null in case an error occurred.</returns>
        AccountResultWrapper<ReferrerIdRecordAccount> GetReferrerIdRecordAccount(PublicKey referrerIdRecord, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribe to a live feed of an <see cref="EventQueue"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="EventQueue"/> and the corresponding slot.</param>
        /// <param name="eventQueueAccountAddress">The public key of the <see cref="EventQueue"/> account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Task<Subscription> SubscribeEventQueueAsync(Action<Subscription, EventQueue, ulong> action, string eventQueueAccountAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribe to a live feed of an <see cref="EventQueue"/>.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="EventQueue"/> and the corresponding slot.</param>
        /// <param name="eventQueueAccountAddress">The public key of the <see cref="EventQueue"/> account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Subscription SubscribeEventQueue(Action<Subscription, EventQueue, ulong> action, string eventQueueAccountAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Unsubscribe to a live feed of an <see cref="EventQueue"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="eventQueueAccountAddress">The public key of the <see cref="EventQueue"/> account.</param>
        Task UnsubscribeEventQueueAsync(string eventQueueAccountAddress);

        /// <summary>
        /// Unsubscribe to a live feed of an <see cref="EventQueue"/>.
        /// </summary>
        /// <param name="eventQueueAccountAddress">The public key of the <see cref="EventQueue"/> account.</param>
        void UnsubscribeEventQueue(string eventQueueAccountAddress);

        /// <summary>
        /// Subscribe to a live feed of a Mango Perpetual Market's Order Book. This will either be a Bid or Ask account data feed. This is an asynchronous operation.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="OrderBookSide"/> and the corresponding slot.</param>
        /// <param name="orderBookAccountAddress">The public key of the Order Book account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Task<Subscription> SubscribeOrderBookSideAsync(Action<Subscription, OrderBookSide, ulong> action, string orderBookAccountAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribe to a live feed of a Mango Perpetual Market's Order Book. This will either be a Bid or Ask account data feed.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="OrderBookSide"/> and the corresponding slot.</param>
        /// <param name="orderBookAccountAddress">The public key of the Order Book account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Subscription SubscribeOrderBookSide(Action<Subscription, OrderBookSide, ulong> action, string orderBookAccountAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Unsubscribe to a live feed of a Mango Perpetual Market's Order Book Side. This will either be a Bid or Ask account data feed. This is an asynchronous operation.
        /// </summary>
        /// <param name="orderBookAccountAddress">The public key of the Order Book account.</param>
        Task UnsubscribeOrderBookSideAsync(string orderBookAccountAddress);

        /// <summary>
        /// Unsubscribe to a live feed of a Mango Perpetual Market's Order Book Side. This will either be a Bid or Ask account data feed.
        /// </summary>
        /// <param name="orderBookAccountAddress">The public key of the Order Book account.</param>
        void UnsubscribeOrderBookSide(string orderBookAccountAddress);

        /// <summary>
        /// Subscribe to a live feed of a <see cref="MangoAccount"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="MangoAccount"/> and the corresponding slot.</param>
        /// <param name="mangoAccountAddress">The public key of the <see cref="MangoAccount"/> account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Task<Subscription> SubscribeMangoAccountAsync(Action<Subscription, MangoAccount, ulong> action, string mangoAccountAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribe to a live feed of a <see cref="MangoAccount"/>.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="MangoAccount"/> and the corresponding slot.</param>
        /// <param name="mangoAccountAddress">The public key of the <see cref="MangoAccount"/> account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Subscription SubscribeMangoAccount(Action<Subscription, MangoAccount, ulong> action, string mangoAccountAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Unsubscribe to a live feed of a <see cref="MangoAccount"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="mangoAccountAddress">The public key of the <see cref="MangoAccount"/> account.</param>
        Task UnsubscribeMangoAccountAsync(string mangoAccountAddress);

        /// <summary>
        /// Unsubscribe to a live feed of a <see cref="MangoAccount"/>.
        /// </summary>
        /// <param name="mangoAccountAddress">The public key of the <see cref="MangoAccount"/> account.</param>
        void UnsubscribeMangoAccount(string mangoAccountAddress);

        /// <summary>
        /// Subscribe to a live feed of a <see cref="MangoCache"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="MangoCache"/> and the corresponding slot.</param>
        /// <param name="mangoCacheAddress">The public key of the <see cref="MangoCache"/> account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Task<Subscription> SubscribeMangoCacheAsync(Action<Subscription, MangoCache, ulong> action, string mangoCacheAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribe to a live feed of a <see cref="MangoCache"/>.
        /// </summary>
        /// <param name="action">An action which receives the <see cref="Subscription"/>, an <see cref="MangoCache"/> and the corresponding slot.</param>
        /// <param name="mangoCacheAddress">The public key of the <see cref="MangoCache"/> account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Subscription SubscribeMangoCache(Action<Subscription, MangoCache, ulong> action, string mangoCacheAddress, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Unsubscribe to a live feed of a <see cref="MangoCache"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="mangoCacheAddress">The public key of the <see cref="MangoCache"/> account.</param>
        Task UnsubscribeMangoCacheAsync(string mangoCacheAddress);

        /// <summary>
        /// Unsubscribe to a live feed of a <see cref="MangoCache"/>.
        /// </summary>
        /// <param name="mangoCacheAddress">The public key of the <see cref="MangoCache"/> account.</param>
        void UnsubscribeMangoCache(string mangoCacheAddress);
    }
}