using System.Collections.Immutable;

namespace Solnet.Mango.Models.Configs
{
    /// <summary>
    /// Represents the config of a token.
    /// </summary>
    public class TokenConfig
    {
        /// <summary>
        /// The symbol of the token.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The public key of the token's mint.
        /// </summary>

        public string MintKey { get; set; }

        /// <summary>
        /// The decimals of the token.
        /// </summary>
        public byte Decimals { get; set; }

        /// <summary>
        /// The public key of the root bank of the token.
        /// </summary>
        public string RootKey { get; set; }

        /// <summary>
        /// The public keys of the node banks.
        /// </summary>
        public IImmutableList<string> NodeKeys { get; set; }
    }
}
