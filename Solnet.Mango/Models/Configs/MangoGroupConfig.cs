using System.Collections.Immutable;

namespace Solnet.Mango.Models.Configs
{
    /// <summary>
    /// Represents the config of a mango group.
    /// </summary>
    public class MangoGroupConfig
    {
        /// <summary>
        /// The name of the mango group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cluster of the mango group.
        /// </summary>
        public string Cluster { get; set; }

        /// <summary>
        /// The public key of the mango group account.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// The quote symbol of the mango group.
        /// </summary>
        public string QuoteSymbol { get; set; }

        /// <summary>
        /// The mango program id this mango group is associated with.
        /// </summary>
        public string MangoProgramId { get; set; }

        /// <summary>
        /// The serum dex program id.
        /// </summary>
        public string SerumProgramId { get; set; }

        /// <summary>
        /// The token configs.
        /// </summary>
        public IImmutableList<TokenConfig> Tokens { get; set; }

        /// <summary>
        /// The oracle configs.
        /// </summary>
        public IImmutableList<OracleConfig> Oracles { get; set; }

        /// <summary>
        /// The perp market configs.
        /// </summary>
        public IImmutableList<MarketConfig> PerpMarkets { get; set; }

        /// <summary>
        /// The spot market configs.
        /// </summary>
        public IImmutableList<MarketConfig> SpotMarkets { get; set; }
    }
}
