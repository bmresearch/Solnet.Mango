using Solnet.Mango.Models;
using Solnet.Mango.Models.Events;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Serum;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public class EventQueueFillLatencyExample : IRunnableExample
    {
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);

        private static readonly IStreamingRpcClient StreamingRpcClient =
            Solnet.Rpc.ClientFactory.GetStreamingClient(Cluster.MainNet);

        private readonly IMangoClient _mangoClient;

        public EventQueueFillLatencyExample()
        {
            StreamingRpcClient.ConnectAsync().Wait();
            _mangoClient = ClientFactory.GetClient(RpcClient, StreamingRpcClient);
        }

        public void Run()
        {
            AccountResultWrapper<MangoGroup> mangoGroup = _mangoClient.GetMangoGroup(Constants.MangoGroup);

            TokenInfo wrappedSolTokenInfo =
                mangoGroup.ParsedResult.Tokens.FirstOrDefault(x => x.Mint.Key == MarketUtils.WrappedSolMint);
            if (wrappedSolTokenInfo == null) return;

            int tokenIndex = mangoGroup.ParsedResult.GetTokenIndex(wrappedSolTokenInfo.Mint);

            var market = _mangoClient.GetPerpMarket(mangoGroup.ParsedResult.PerpetualMarkets[tokenIndex].Market);

            Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()} Loading Event Queue for {mangoGroup.ParsedResult.PerpetualMarkets[tokenIndex].Market}");

            var queue = _mangoClient.GetEventQueue(market.ParsedResult.EventQueue, Rpc.Types.Commitment.Processed);

            var lastSeqNum = queue.ParsedResult.Header.NextSequenceNumber;
            var lastFetchTs = DateTime.UtcNow;
            Console.WriteLine($"{lastFetchTs.ToLongTimeString()} Last Sequence Number: {lastSeqNum}");

            while (true)
            {
                while(lastSeqNum == queue.ParsedResult.Header.NextSequenceNumber)
                {
                    Task.Delay(1000).Wait();
                    queue = _mangoClient.GetEventQueue(market.ParsedResult.EventQueue, Rpc.Types.Commitment.Processed);
                    lastFetchTs = DateTime.UtcNow;
                    //Console.WriteLine($"{lastFetchTs.ToLongTimeString()} Refreshed Event Queue");
                }

                var eventQ = EventQueue.DeserializeSince(Convert.FromBase64String(queue.OriginalRequest.Result.Value.Data[0]), lastSeqNum);
                Console.WriteLine($"{DateTime.UtcNow.ToShortTimeString()} Events: {eventQ.Events.Count}");

                var lastFill = eventQ.Events.Reverse().First(x => x.EventType == EventType.Fill);
                if(lastFill != null)
                {
                    Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()} Lag: {(lastFetchTs - DateTime.UnixEpoch).TotalSeconds - lastFill.Timestamp}");
                }

                lastSeqNum = eventQ.Header.NextSequenceNumber;
            }
        }
    }
}
