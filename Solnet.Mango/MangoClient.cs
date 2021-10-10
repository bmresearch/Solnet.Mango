using Microsoft.Extensions.Logging;
using Solnet.Mango.Models;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Serum;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventQueue = Solnet.Mango.Models.EventQueue;
using OrderBook = Solnet.Mango.Models.OrderBook;
using OrderBookSide = Solnet.Mango.Models.OrderBookSide;

namespace Solnet.Mango
{
    /// <summary>
    /// Implements functionality for the Mango client.
    /// </summary>
    public class MangoClient : BaseClient, IMangoClient
    {
        /// <summary>
        /// The logger instance.
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// The list of <see cref="EventQueue"/> subscriptions.
        /// </summary>
        private readonly IList<SubscriptionWrapper<EventQueue>> _eventQueueSubscriptions;

        /// <summary>
        /// The list of <see cref="OrderBookSide"/> subscriptions.
        /// </summary>
        private readonly IList<SubscriptionWrapper<OrderBookSide>> _orderBookSideSubscriptions;

        /// <summary>
        /// The list of <see cref="MangoAccount"/> subscriptions.
        /// </summary>
        private readonly IList<SubscriptionWrapper<MangoAccount>> _mangoAccountSubscriptions;

        /// <summary>
        /// The list of <see cref="MangoCache"/> subscriptions.
        /// </summary>
        private readonly IList<SubscriptionWrapper<MangoCache>> _mangoCacheSubscriptions;

        /// <summary>
        /// Initialize the Mango Client.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="rpcClient">The RPC client instance.</param>
        /// <param name="streamingRpcClient">The streaming RPC client.</param>
        /// <returns>The Mango Client.</returns>
        internal MangoClient(ILogger logger = null, IRpcClient rpcClient = default,
            IStreamingRpcClient streamingRpcClient = default) : base(rpcClient, streamingRpcClient)
        {
            _logger = logger;
            _eventQueueSubscriptions = new List<SubscriptionWrapper<EventQueue>>();
            _mangoAccountSubscriptions = new List<SubscriptionWrapper<MangoAccount>>();
            _mangoCacheSubscriptions = new List<SubscriptionWrapper<MangoCache>>();
            _orderBookSideSubscriptions = new List<SubscriptionWrapper<OrderBookSide>>();
        }

        /// <inheritdoc cref="IMangoClient.GetMangoGroupAsync(string, Commitment)"/>
        public async Task<AccountResultWrapper<MangoGroup>> GetMangoGroupAsync(string account,
            Commitment commitment = Commitment.Finalized)
            => await GetAccount<MangoGroup>(account, commitment);

        /// <inheritdoc cref="IMangoClient.GetMangoGroup(string, Commitment)"/>
        public AccountResultWrapper<MangoGroup> GetMangoGroup(string account,
            Commitment commitment = Commitment.Finalized) => GetMangoGroupAsync(account, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetMangoCacheAsync(string, Commitment)"/>
        public async Task<AccountResultWrapper<MangoCache>> GetMangoCacheAsync(string account,
            Commitment commitment = Commitment.Finalized)
            => await GetAccount<MangoCache>(account, commitment);

        /// <inheritdoc cref="IMangoClient.GetMangoCache(string, Commitment)"/>
        public AccountResultWrapper<MangoCache> GetMangoCache(string account,
            Commitment commitment = Commitment.Finalized) => GetMangoCacheAsync(account, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetRootBankAsync(string, Commitment)"/>
        public async Task<AccountResultWrapper<RootBank>> GetRootBankAsync(string account,
            Commitment commitment = Commitment.Finalized)
            => await GetAccount<RootBank>(account, commitment);

        /// <inheritdoc cref="IMangoClient.GetRootBank(string, Commitment)"/>
        public AccountResultWrapper<RootBank> GetRootBank(string account,
            Commitment commitment = Commitment.Finalized) => GetRootBankAsync(account, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetRootBanksAsync(MangoGroup, Commitment)"/>
        public async Task<MultipleAccountsResultWrapper<List<RootBank>>> GetRootBanksAsync(MangoGroup mangoGroup, Commitment commitment = Commitment.Finalized)
            => await GetMultipleAccounts<RootBank>(mangoGroup.Tokens.Select(x => x.RootBank.Key).ToList(), commitment);

        /// <inheritdoc cref="IMangoClient.GetRootBanks(MangoGroup, Commitment)"/>
        public MultipleAccountsResultWrapper<List<RootBank>> GetRootBanks(MangoGroup mangoGroup,
            Commitment commitment = Commitment.Finalized)
            => GetRootBanksAsync(mangoGroup, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetNodeBankAsync(string, Commitment)"/>
        public async Task<AccountResultWrapper<NodeBank>> GetNodeBankAsync(string account,
            Commitment commitment = Commitment.Finalized)
            => await GetAccount<NodeBank>(account, commitment);

        /// <inheritdoc cref="IMangoClient.GetNodeBank(string, Commitment)"/>
        public AccountResultWrapper<NodeBank> GetNodeBank(string account,
            Commitment commitment = Commitment.Finalized) => GetNodeBankAsync(account, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetNodeBanksAsync(RootBank, Commitment)"/>
        public async Task<MultipleAccountsResultWrapper<List<NodeBank>>> GetNodeBanksAsync(RootBank rootBank, Commitment commitment = Commitment.Finalized)
            => await GetMultipleAccounts<NodeBank>(rootBank.NodeBanks.Select(x => x.Key).ToList(), commitment);

        /// <inheritdoc cref="IMangoClient.GetNodeBanks(RootBank, Commitment)"/>
        public MultipleAccountsResultWrapper<List<NodeBank>> GetNodeBanks(RootBank rootBank,
            Commitment commitment = Commitment.Finalized)
            => GetNodeBanksAsync(rootBank, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetPerpMarketAsync(string, Commitment)"/>
        public async Task<AccountResultWrapper<PerpMarket>> GetPerpMarketAsync(string account,
            Commitment commitment = Commitment.Finalized)
            => await GetAccount<PerpMarket>(account, commitment);

        /// <inheritdoc cref="IMangoClient.GetPerpMarket(string, Commitment)"/>
        public AccountResultWrapper<PerpMarket> GetPerpMarket(string account,
            Commitment commitment = Commitment.Finalized) => GetPerpMarketAsync(account, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetOrderBookSideAsync(string, Commitment)"/>
        public async Task<AccountResultWrapper<OrderBookSide>> GetOrderBookSideAsync(string account,
            Commitment commitment = Commitment.Finalized)
            => await GetAccount<OrderBookSide>(account, commitment);

        /// <inheritdoc cref="IMangoClient.GetOrderBookSide(string, Commitment)"/>
        public AccountResultWrapper<OrderBookSide> GetOrderBookSide(string account,
            Commitment commitment = Commitment.Finalized)
            => GetOrderBookSideAsync(account, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetOrderBookAsync(PerpMarket, Commitment)"/>
        public async Task<MultipleAccountsResultWrapper<OrderBook>> GetOrderBookAsync(PerpMarket perpMarket,
            Commitment commitment = Commitment.Finalized)
        {
            MultipleAccountsResultWrapper<List<OrderBookSide>> accounts =
                await GetMultipleAccounts<OrderBookSide>(new List<string> { perpMarket.Asks, perpMarket.Bids }, commitment);

            return new MultipleAccountsResultWrapper<OrderBook>(accounts.OriginalRequest,
                new OrderBook { Asks = accounts.ParsedResult[0], Bids = accounts.ParsedResult[1], });
        }

        /// <inheritdoc cref="IMangoClient.GetOrderBook(PerpMarket, Commitment)"/>
        public MultipleAccountsResultWrapper<OrderBook> GetOrderBook(PerpMarket perpMarket,
            Commitment commitment = Commitment.Finalized)
            => GetOrderBookAsync(perpMarket, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetEventQueueAsync(string,Commitment)"/>
        public async Task<AccountResultWrapper<EventQueue>> GetEventQueueAsync(string eventQueueAddress,
            Commitment commitment = Commitment.Finalized)
            => await GetAccount<EventQueue>(eventQueueAddress, commitment);

        /// <inheritdoc cref="IMangoClient.GetEventQueue(string,Commitment)"/>
        public AccountResultWrapper<EventQueue> GetEventQueue(string eventQueueAddress, Commitment commitment = Commitment.Finalized)
            => GetEventQueueAsync(eventQueueAddress, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetMangoAccountsAsync(string, Commitment)"/>
        public async Task<ProgramAccountsResultWrapper<List<MangoAccount>>> GetMangoAccountsAsync(string ownerAccount,
            Commitment commitment = Commitment.Finalized)
        {
            List<MemCmp> filters = new()
            {
                new MemCmp { Offset = MangoAccount.Layout.OwnerOffset, Bytes = ownerAccount }
            };

            return await GetProgramAccounts<MangoAccount>(MangoProgram.ProgramIdKeyV3, filters,
                MangoAccount.Layout.Length,
                commitment);
        }

        /// <inheritdoc cref="IMangoClient.GetMangoAccounts(string, Commitment)"/>
        public ProgramAccountsResultWrapper<List<MangoAccount>> GetMangoAccounts(string ownerAccount,
            Commitment commitment = Commitment.Finalized) => GetMangoAccountsAsync(ownerAccount, commitment).Result;

        /// <inheritdoc cref="IMangoClient.GetPerpMarketsAsync(string, Commitment)"/>
        public async Task<ProgramAccountsResultWrapper<List<PerpMarket>>> GetPerpMarketsAsync(string mangoGroup,
            Commitment commitment = Commitment.Finalized)
        {
            List<MemCmp> filters = new()
            {
                new MemCmp { Offset = PerpMarket.Layout.MangoGroupOffset, Bytes = mangoGroup }
            };

            return await GetProgramAccounts<PerpMarket>(MangoProgram.ProgramIdKeyV3, filters, PerpMarket.Layout.Length,
                commitment);
        }

        /// <inheritdoc cref="IMangoClient.GetPerpMarkets(string, Commitment)"/>
        public ProgramAccountsResultWrapper<List<PerpMarket>> GetPerpMarkets(string mangoGroup,
            Commitment commitment = Commitment.Finalized) => GetPerpMarketsAsync(mangoGroup, commitment).Result;

        /// <inheritdoc cref="IMangoClient.SubscribeEventQueueAsync(Action{Subscription, EventQueue, ulong}, string, Commitment)"/>
        public async Task<Subscription> SubscribeEventQueueAsync(
            Action<Subscription, EventQueue, ulong> action, string eventQueueAccountAddress, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState sub = await StreamingRpcClient.SubscribeAccountInfoAsync(eventQueueAccountAddress,
                (_, value) =>
                {
                    SubscriptionWrapper<EventQueue> evtQueueSub =
                        _eventQueueSubscriptions.FirstOrDefault(s => s.Address.Key == eventQueueAccountAddress);
                    EventQueue evtQueue;

                    if (evtQueueSub?.Data != null)
                    {
                        evtQueue = EventQueue.DeserializeSince(
                            Convert.FromBase64String(value.Value.Data[0]), evtQueueSub.Data.Header.NextSequenceNumber);
                        evtQueueSub.Data = evtQueue;
                    }
                    else
                    {
                        evtQueue = EventQueue.Deserialize(Convert.FromBase64String(value.Value.Data[0]));
                        if (evtQueueSub != null) evtQueueSub.Data = evtQueue;
                    }

                    action(evtQueueSub, evtQueue, value.Context.Slot);
                }, commitment);

            SubscriptionWrapper<EventQueue> subEvtQueue = new()
            {
                SubscriptionState = sub,
                Address = new PublicKey(eventQueueAccountAddress)
            };
            _eventQueueSubscriptions.Add(subEvtQueue);
            return subEvtQueue;
        }

        /// <inheritdoc cref="IMangoClient.SubscribeEventQueue(Action{Subscription, EventQueue, ulong}, string, Commitment)"/>
        public Subscription SubscribeEventQueue(
            Action<Subscription, EventQueue, ulong> action, string eventQueueAccountAddress, Commitment commitment = Commitment.Finalized)
            => SubscribeEventQueueAsync(action, eventQueueAccountAddress, commitment).Result;

        /// <inheritdoc cref="IMangoClient.UnsubscribeEventQueueAsync(string)"/>
        public Task UnsubscribeEventQueueAsync(string eventQueueAccountAddress)
        {
            SubscriptionWrapper<EventQueue> subscriptionWrapper = null;

            foreach (SubscriptionWrapper<EventQueue> sub in _eventQueueSubscriptions)
            {
                if (sub.Address.Key != eventQueueAccountAddress) continue;

                subscriptionWrapper = sub;
                _eventQueueSubscriptions.Remove(sub);
                break;
            }

            return subscriptionWrapper == null ? null : StreamingRpcClient.UnsubscribeAsync(subscriptionWrapper.SubscriptionState);
        }

        /// <inheritdoc cref="IMangoClient.UnsubscribeEventQueue(string)"/>
        public void UnsubscribeEventQueue(string eventQueueAccountAddress) =>
            UnsubscribeEventQueueAsync(eventQueueAccountAddress);

        /// <inheritdoc cref="IMangoClient.SubscribeOrderBookSideAsync(Action{Subscription, OrderBookSide, ulong}, string, Commitment)"/>
        public async Task<Subscription> SubscribeOrderBookSideAsync(Action<Subscription, OrderBookSide, ulong> action, string orderBookAccountAddress,
            Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState sub = await SubscribeAccount<OrderBookSide>(orderBookAccountAddress,
                (state, accountInfo, orderBook) =>
                {
                    SubscriptionWrapper<OrderBookSide> orderBookSub =
                        _orderBookSideSubscriptions.FirstOrDefault(
                            s => s.Address.Key == orderBookAccountAddress);
                    if (orderBookSub == null) return;
                    orderBookSub.SubscriptionState = state;
                    action(orderBookSub, orderBook, accountInfo.Context.Slot);
                }, commitment);

            SubscriptionWrapper<OrderBookSide> subOrderBook = new()
            {
                SubscriptionState = sub,
                Address = new PublicKey(orderBookAccountAddress)
            };
            _orderBookSideSubscriptions.Add(subOrderBook);
            return subOrderBook;
        }

        /// <inheritdoc cref="IMangoClient.SubscribeOrderBookSide(Action{Subscription, OrderBookSide, ulong}, string, Commitment)"/>
        public Subscription SubscribeOrderBookSide(Action<Subscription, OrderBookSide, ulong> action,
            string orderBookAccountAddress, Commitment commitment = Commitment.Finalized)
            => SubscribeOrderBookSideAsync(action, orderBookAccountAddress, commitment).Result;

        /// <inheritdoc cref="IMangoClient.UnsubscribeOrderBookSideAsync(string)"/>
        public Task UnsubscribeOrderBookSideAsync(string orderBookAccountAddress)
        {
            SubscriptionWrapper<OrderBookSide> subscriptionWrapper = null;

            foreach (SubscriptionWrapper<OrderBookSide> sub in _orderBookSideSubscriptions)
            {
                if (sub.Address.Key != orderBookAccountAddress) continue;

                subscriptionWrapper = sub;
                _orderBookSideSubscriptions.Remove(sub);
                break;
            }

            return subscriptionWrapper == null ? null : StreamingRpcClient.UnsubscribeAsync(subscriptionWrapper.SubscriptionState);
        }

        /// <inheritdoc cref="IMangoClient.UnsubscribeOrderBookSide(string)"/>
        public void UnsubscribeOrderBookSide(string orderBookAccountAddress) =>
            UnsubscribeOrderBookSideAsync(orderBookAccountAddress).Wait();

        /// <inheritdoc cref="IMangoClient.SubscribeMangoAccountAsync(Action{Subscription, MangoAccount, ulong}, string, Commitment)"/>
        public async Task<Subscription> SubscribeMangoAccountAsync(Action<Subscription, MangoAccount, ulong> action, string mangoAccountAddress,
            Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState sub = await SubscribeAccount<MangoAccount>(mangoAccountAddress,
                (state, accountInfo, mangoAccount) =>
                {
                    SubscriptionWrapper<MangoAccount> mangoAccountSub =
                        _mangoAccountSubscriptions.FirstOrDefault(
                            s => s.Address.Key == mangoAccountAddress);
                    if (mangoAccountSub == null) return;
                    mangoAccountSub.SubscriptionState = state;
                    action(mangoAccountSub, mangoAccount, accountInfo.Context.Slot);
                }, commitment);

            SubscriptionWrapper<MangoAccount> subOrderBook = new()
            {
                SubscriptionState = sub,
                Address = new PublicKey(mangoAccountAddress)
            };
            _mangoAccountSubscriptions.Add(subOrderBook);
            return subOrderBook;
        }

        /// <inheritdoc cref="IMangoClient.SubscribeMangoAccount(Action{Subscription, MangoAccount, ulong}, string, Commitment)"/>
        public Subscription SubscribeMangoAccount(Action<Subscription, MangoAccount, ulong> action,
            string mangoAccountAddress, Commitment commitment = Commitment.Finalized)
            => SubscribeMangoAccountAsync(action, mangoAccountAddress, commitment).Result;

        /// <inheritdoc cref="IMangoClient.UnsubscribeMangoAccountAsync(string)"/>
        public Task UnsubscribeMangoAccountAsync(string mangoAccountAddress)
        {
            SubscriptionWrapper<MangoAccount> subscriptionWrapper = null;

            foreach (SubscriptionWrapper<MangoAccount> sub in _mangoAccountSubscriptions)
            {
                if (sub.Address.Key != mangoAccountAddress) continue;

                subscriptionWrapper = sub;
                _mangoAccountSubscriptions.Remove(sub);
                break;
            }

            return subscriptionWrapper == null ? null : StreamingRpcClient.UnsubscribeAsync(subscriptionWrapper.SubscriptionState);
        }

        /// <inheritdoc cref="IMangoClient.UnsubscribeMangoAccount(string)"/>
        public void UnsubscribeMangoAccount(string mangoAccountAddress) =>
            UnsubscribeMangoAccountAsync(mangoAccountAddress).Wait();

        /// <inheritdoc cref="IMangoClient.SubscribeMangoCacheAsync(Action{Subscription, MangoCache, ulong}, string, Commitment)"/>
        public async Task<Subscription> SubscribeMangoCacheAsync(Action<Subscription, MangoCache, ulong> action, string mangoAccountAddress,
            Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState sub = await SubscribeAccount<MangoCache>(mangoAccountAddress,
                (state, accountInfo, mangoAccount) =>
                {
                    SubscriptionWrapper<MangoCache> mangoCacheSub =
                        _mangoCacheSubscriptions.FirstOrDefault(
                            s => s.Address.Key == mangoAccountAddress);
                    if (mangoCacheSub == null) return;
                    mangoCacheSub.SubscriptionState = state;
                    action(mangoCacheSub, mangoAccount, accountInfo.Context.Slot);
                }, commitment);

            SubscriptionWrapper<MangoCache> subMangoCache = new()
            {
                SubscriptionState = sub,
                Address = new PublicKey(mangoAccountAddress)
            };
            _mangoCacheSubscriptions.Add(subMangoCache);
            return subMangoCache;
        }

        /// <inheritdoc cref="IMangoClient.SubscribeMangoCache(Action{Subscription, MangoCache, ulong}, string, Commitment)"/>
        public Subscription SubscribeMangoCache(Action<Subscription, MangoCache, ulong> action,
            string mangoCacheAddress, Commitment commitment = Commitment.Finalized)
            => SubscribeMangoCacheAsync(action, mangoCacheAddress, commitment).Result;

        /// <inheritdoc cref="IMangoClient.UnsubscribeMangoCacheAsync(string)"/>
        public Task UnsubscribeMangoCacheAsync(string mangoCacheAddress)
        {
            SubscriptionWrapper<MangoCache> subscriptionWrapper = null;

            foreach (SubscriptionWrapper<MangoCache> sub in _mangoCacheSubscriptions)
            {
                if (sub.Address.Key != mangoCacheAddress) continue;

                subscriptionWrapper = sub;
                _mangoCacheSubscriptions.Remove(sub);
                break;
            }

            return subscriptionWrapper == null ? null : StreamingRpcClient.UnsubscribeAsync(subscriptionWrapper.SubscriptionState);
        }

        /// <inheritdoc cref="IMangoClient.UnsubscribeMangoCache(string)"/>
        public void UnsubscribeMangoCache(string mangoCacheAddress) =>
            UnsubscribeMangoAccountAsync(mangoCacheAddress).Wait();
    }
}