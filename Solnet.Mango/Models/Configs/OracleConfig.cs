using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models.Configs
{
    /// <summary>
    /// Represents the config of an oracle.
    /// </summary>
    public class OracleConfig
    {
        /// <summary>
        /// The symbol of the token.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The public key of the oracle account.
        /// </summary>
        public string PublicKey { get; set; }
    }
}
