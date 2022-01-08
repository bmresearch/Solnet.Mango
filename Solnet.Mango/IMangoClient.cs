using Solnet.Mango.Models;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventQueue = Solnet.Mango.Models.EventQueue;
using OrderBook = Solnet.Mango.Models.OrderBook;
using OrderBookSide = Solnet.Mango.Models.OrderBookSide;

namespace Solnet.Mango
{
    /// <summary>
    /// Specifies functionality for the Mango client.
    /// </summary>
    public interface IMangoClient
    {
        /// <summary>
        /// The <see cref="IRpcClient"/> instance.
        /// </summary>
        IRpcClient RpcClient { get; }

        /// <summary>
        /// The <see cref="IStreamingRpcClient"/> instance.
        /// </summary>
        IStreamingRpcClient StreamingRpcClient { get; }

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
        /// Gets the <see cref="RootBank"/>s for the given <see cref="MangoGroup"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="mangoGroup">The <see cref="MangoGroup"/>.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="RootBank"/>s or null in case an error occurred.</returns>
        Task<MultipleAccountsResultWrapper<List<RootBank>>> GetRootBanksAsync(MangoGroup mangoGroup,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the <see cref="RootBank"/>s for the given <see cref="MangoGroup"/>.
        /// </summary>
        /// <param name="mangoGroup">The <see cref="MangoGroup"/>.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="RootBank"/>s.</returns>
        MultipleAccountsResultWrapper<List<RootBank>> GetRootBanks(MangoGroup mangoGroup, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the given <see cref="NodeBank"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="NodeBank"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="NodeBank"/>s or null in case an error occurred.</returns>
        Task<AccountResultWrapper<NodeBank>> GetNodeBankAsync(string account,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the <see cref="NodeBank"/>s for the given <see cref="RootBank"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="rootBank">The <see cref="RootBank"/>.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="NodeBank"/>s or null in case an error occurred.</returns>
        Task<MultipleAccountsResultWrapper<List<NodeBank>>> GetNodeBanksAsync(RootBank rootBank,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the <see cref="NodeBank"/>s for the given <see cref="RootBank"/>.
        /// </summary>
        /// <param name="rootBank">The <see cref="RootBank"/>.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="NodeBank"/>s.</returns>
        MultipleAccountsResultWrapper<List<NodeBank>> GetNodeBanks(RootBank rootBank, Commitment commitment = Commitment.Finalized);

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
        /// Gets the given <see cref="Models.OrderBookSide"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="account">The <see cref="Models.OrderBookSide"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="Models.OrderBookSide"/>s or null in case an error occurred.</returns>
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
        /// Gets the given <see cref="Models.OrderBook"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="perpMarket">The <see cref="PerpMarket"/>.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="Models.OrderBook"/>s or null in case an error occurred.</returns>
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
        /// Gets the given <see cref="Models.EventQueue"/>. This is an asynchronous operation.
        /// </summary>
        /// <param name="eventQueueAddress">The <see cref="Models.EventQueue"/> public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="Models.EventQueue"/>s or null in case an error occurred.</returns>
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
        /// <param name="mangoGroup">The mango group public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="PerpMarket"/>s or null in case an error occurred.</returns>
        Task<ProgramAccountsResultWrapper<List<PerpMarket>>> GetPerpMarketsAsync(string mangoGroup,
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Gets the perpetual markets for the given Mango group.
        /// </summary>
        /// <param name="mangoGroup">The mango group public key.</param>
        /// <param name="commitment">The confirmation commitment parameter for the RPC call.</param>
        /// <returns>The list of <see cref="PerpMarket"/>s or null in case an error occurred.</returns>
        ProgramAccountsResultWrapper<List<PerpMarket>> GetPerpMarkets(string mangoGroup,
            Commitment commitment = Commitment.Finalized);

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
        /// <param name="mangoAccountAddress">The public key of the <see cref="MangoCache"/> account.</param>
        /// <param name="commitment">The commitment parameter for the Rpc Client.</param>
        Task<Subscription> SubscribeMangoCacheAsync(Action<Subscription, MangoCache, ulong> action, string mangoAccountAddress, Commitment commitment = Commitment.Finalized);

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