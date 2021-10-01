using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Solnet.Rpc;

namespace Solnet.Mango
{
    /// <summary>
    /// The client factory for the Mango Client.
    /// </summary>
    public static class ClientFactory
    {
        /// <summary>
        /// Instantiate the Mango client.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="rpcClient">The RPC Client instance.</param>
        /// <param name="streamingRpcClient">The Streaming RPC Client instance.</param>
        /// <returns>The Serum Client.</returns>
        public static IMangoClient GetClient(IRpcClient rpcClient = null, IStreamingRpcClient streamingRpcClient = null,
            ILogger logger = null)
        {
#if DEBUG
            logger ??= GetDebugLogger();
#endif
            return new MangoClient(logger, rpcClient, streamingRpcClient);
        }

#if DEBUG
        /// <summary>
        /// Get a logger instance for use in debug mode.
        /// </summary>
        /// <returns>The logger.</returns>
        private static ILogger GetDebugLogger()
        {
            return LoggerFactory.Create(x =>
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
        }
#endif
    }
}