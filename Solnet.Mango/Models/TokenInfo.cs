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
        /// The layout of the <see cref="TokenInfo"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="TokenInfo"/> structure.
            /// </summary>
            internal const int Length = 72;

            /// <summary>
            /// The offset at which the token mint begins.
            /// </summary>
            internal const int MintOffset = 0;

            /// <summary>
            /// The offset at which the root bank begins.
            /// </summary>
            internal const int RootBankOffset = 32;

            /// <summary>
            /// The offset at which the decimals begin.
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
        /// Deserialize a span of bytes into a <see cref="TokenInfo"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="TokenInfo"/> structure.</returns>
        public static TokenInfo Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            return new TokenInfo
            {
                Mint = data.GetPubKey(Layout.MintOffset),
                RootBank = data.GetPubKey(Layout.RootBankOffset),
                Decimals = data.GetU8(Layout.DecimalsOffset)
            };
        }
    }
}