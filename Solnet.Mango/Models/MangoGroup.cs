using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a group of lending pools that can be cross margined.
    /// </summary>
    public class MangoGroup
    {
        /// <summary>
        /// The layout of the <see cref="MangoGroup"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int NumOraclesOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int TokensOffset = 16;

            /// <summary>
            /// 
            /// </summary>
            internal const int SpotMarketsOffset = 1168;

            /// <summary>
            /// 
            /// </summary>
            internal const int PerpMarketsOffset = 2848;

            /// <summary>
            /// 
            /// </summary>
            internal const int OraclesOffset = 5248;

            /// <summary>
            /// 
            /// </summary>
            internal const int SignerNonceOffset = 5728;

            /// <summary>
            /// 
            /// </summary>
            internal const int SignerKeyOffset = 5736;

            /// <summary>
            /// 
            /// </summary>
            internal const int AdminKeyOffset = 5768;

            /// <summary>
            /// 
            /// </summary>
            internal const int DexProgramKeyOffset = 5800;

            /// <summary>
            /// 
            /// </summary>
            internal const int MangoCacheKeyOffset = 5832;

            /// <summary>
            /// 
            /// </summary>
            internal const int ValidInternalOffset = 5864;

            /// <summary>
            /// 
            /// </summary>
            internal const int InsuranceVaultKeyOffset = 5872;

            /// <summary>
            /// 
            /// </summary>
            internal const int SerumVaultOffset = 5904;

            /// <summary>
            /// 
            /// </summary>
            internal const int MegaSerumVaultOffset = 5936;

            /// <summary>
            /// 
            /// </summary>
            internal const int FeesVaultOffset = 5968;
        }

        /// <summary>
        /// The account's metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The number of oracles.
        /// </summary>
        public ulong NumOracles;

        /// <summary>
        /// The list of tokens.
        /// </summary>
        public List<TokenInfo> Tokens;

        /// <summary>
        /// The list of markets whose tokens are cross-collateralized.
        /// </summary>
        public List<SpotMarketInfo> SpotMarkets;

        /// <summary>
        /// The list of perpetual markets.
        /// </summary>
        public List<PerpMarketInfo> PerpetualMarkets;

        /// <summary>
        /// The oracle public keys.
        /// </summary>
        public List<PublicKey> Oracles;

        /// <summary>
        /// 
        /// </summary>
        public ulong SignerNonce;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey SignerKey;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey Admin;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey DexProgramId;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey MangoCache;

        /// <summary>
        /// 
        /// </summary>
        public ulong ValidInterval;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey InsuranceVault;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey SerumVault;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey MegaSerumVault;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey FeesVault;

        /// <summary>
        /// Gets the index for the given oracle <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="oracle"></param>
        /// <returns></returns>
        public int GetOracleIndex(PublicKey oracle)
        {
            for(int i = 0; i < (int) NumOracles; i++)
            {
                if (Oracles[i] == oracle) return i;
            }

            throw new Exception("This Oracle does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given spot market <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="spotMarket"></param>
        /// <returns></returns>
        public int GetSpotMarketIndex(PublicKey spotMarket)
        {
            for (int i = 0; i < (int)NumOracles; i++)
            {
                if (SpotMarkets[i].Market == spotMarket) return i;
            }

            throw new Exception("This Market does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given perp market <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="perpMarket"></param>
        /// <returns></returns>
        public int GetPerpMarketIndex(PublicKey perpMarket)
        {
            for (int i = 0; i < (int)NumOracles; i++)
            {
                if (PerpetualMarkets[i].Market == perpMarket) return i;
            }

            throw new Exception("This PerpMarket does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given token mint <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="tokenMint"></param>
        /// <returns></returns>
        public int GetTokenIndex(PublicKey tokenMint)
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].Mint == tokenMint) return i;
            }

            throw new Exception("This Token Mint does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given root bank <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="rootBank"></param>
        /// <returns></returns>
        public int GetRootBankIndex(PublicKey rootBank)
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].RootBank == rootBank) return i;
            }

            throw new Exception("This Root Bank does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the borrow rate for the given token index.
        /// </summary>
        /// <param name="tokenIndex"></param>
        /// <returns></returns>
        public int GetBorrowRate(int tokenIndex)
        {
            // TODO: load rootbanks

            throw new Exception("This Root Bank does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the borrow rate for the given token index.
        /// </summary>
        /// <param name="tokenIndex"></param>
        /// <returns></returns>
        public int GetDepositRate(int tokenIndex)
        {
            // TODO: load rootbanks

            throw new Exception("This Root Bank does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the price for the given token index.
        /// </summary>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The price.</returns>
        public double GetPrice(MangoCache mangoCache, int tokenIndex)
        {
            if (tokenIndex == Constants.QuoteIndex) return 1;

            var decimalAdj = Math.Pow(10, Tokens[tokenIndex].Decimals - Tokens[Constants.QuoteIndex].Decimals);

            return mangoCache.PriceCaches[tokenIndex].Price.Value * decimalAdj;
        }

        /// <summary>
        /// Gets the native price for the given token index.
        /// </summary>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The price.</returns>
        public double GetPriceNative(MangoCache mangoCache, int tokenIndex)
        {
            if (tokenIndex == Constants.QuoteIndex) return 1;
            return mangoCache.PriceCaches[tokenIndex].Price.Value;
        }

        /// <summary>
        /// Gets the <see cref="TokenInfo"/> of the quote token.
        /// </summary>
        /// <returns>The token info.</returns>
        public TokenInfo GetQuoteTokenInfo()
        {
            return Tokens[Constants.MaxTokens - 1];
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="MangoGroup"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="MangoGroup"/> structure.</returns>
        public static MangoGroup Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();
            List<TokenInfo> tokens = new(Constants.MaxTokens);
            ReadOnlySpan<byte> tokensBytes =
                span.Slice(Layout.TokensOffset, TokenInfo.Layout.Length * Constants.MaxTokens);

            for (int i = 0; i < Constants.MaxTokens; i++)
            {
                TokenInfo tokenInfo = TokenInfo.Deserialize(tokensBytes.GetSpan(i * TokenInfo.Layout.Length,
                    TokenInfo.Layout.Length));
                tokens.Add(tokenInfo);
            }

            List<SpotMarketInfo> spotMarkets = new(Constants.MaxPairs);
            ReadOnlySpan<byte> spotMarketsBytes =
                span.Slice(Layout.SpotMarketsOffset, SpotMarketInfo.Layout.Length * Constants.MaxPairs);

            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                SpotMarketInfo spotMarketInfo = SpotMarketInfo.Deserialize(spotMarketsBytes.GetSpan(
                    i * SpotMarketInfo.Layout.Length,
                    SpotMarketInfo.Layout.Length));
                spotMarkets.Add(spotMarketInfo);
            }

            List<PerpMarketInfo> perpMarkets = new(Constants.MaxPairs);
            ReadOnlySpan<byte> perpMarketsBytes = span.Slice(Layout.PerpMarketsOffset,
                PerpMarketInfo.ExtraLayout.Length * Constants.MaxPairs);

            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                PerpMarketInfo perpMarketInfo =
                    PerpMarketInfo.Deserialize(perpMarketsBytes.GetSpan(i * PerpMarketInfo.ExtraLayout.Length,
                        PerpMarketInfo.ExtraLayout.Length));
                perpMarkets.Add(perpMarketInfo);
            }

            List<PublicKey> oracles = new();
            ReadOnlySpan<byte> oraclesBytes =
                span.Slice(Layout.OraclesOffset, PublicKey.PublicKeyLength * Constants.MaxPairs);

            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                PublicKey oracle = oraclesBytes.GetPubKey(i * PublicKey.PublicKeyLength);
                oracles.Add(oracle);
            }

            return new MangoGroup
            {
                Metadata = MetaData.Deserialize(span.GetSpan(Layout.MetadataOffset, MetaData.Layout.Length)),
                NumOracles = span.GetU64(Layout.NumOraclesOffset),
                Tokens = tokens,
                SpotMarkets = spotMarkets,
                PerpetualMarkets = perpMarkets,
                Oracles = oracles,
                SignerNonce = span.GetU64(Layout.SignerNonceOffset),
                SignerKey = span.GetPubKey(Layout.SignerKeyOffset),
                Admin = span.GetPubKey(Layout.AdminKeyOffset),
                DexProgramId = span.GetPubKey(Layout.DexProgramKeyOffset),
                MangoCache = span.GetPubKey(Layout.MangoCacheKeyOffset),
                ValidInterval = span.GetU64(Layout.ValidInternalOffset),
                InsuranceVault = span.GetPubKey(Layout.InsuranceVaultKeyOffset),
                SerumVault = span.GetPubKey(Layout.SerumVaultOffset),
                MegaSerumVault = span.GetPubKey(Layout.MegaSerumVaultOffset),
                FeesVault = span.GetPubKey(Layout.FeesVaultOffset)
            };
        }
    }
}