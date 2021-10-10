using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents information about a cross-collateralized token in Mango.
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            internal const int Length = 72;

            /// <summary>
            /// 
            /// </summary>
            internal const int MintOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int RootBankOffset = 32;

            /// <summary>
            /// 
            /// </summary>
            internal const int DecimalsOffset = 64;
        }

        /// <summary>
        /// The token's mint.
        /// </summary>
        public PublicKey Mint;

        /// <summary>
        /// The root bank of this token.
        /// </summary>
        public PublicKey RootBank;

        /// <summary>
        /// The number of decimals of the token.
        /// </summary>1
        public byte Decimals;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TokenInfo Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");

            return new TokenInfo
            {
                Mint = data.GetPubKey(Layout.MintOffset),
                RootBank = data.GetPubKey(Layout.RootBankOffset),
                Decimals = data.GetU8(Layout.DecimalsOffset)
            };
        }
    }
}