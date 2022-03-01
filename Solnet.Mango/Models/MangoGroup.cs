using Microsoft.Extensions.Logging;
using Solnet.Mango.Models.Banks;
using Solnet.Mango.Models.Caches;
using Solnet.Mango.Models.Configs;
using Solnet.Mango.Models.Perpetuals;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            /// The length of the structure.
            /// </summary>
            internal const int Length = 6032;

            /// <summary>
            /// The offset at which the metadata begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the number of oracles begins.
            /// </summary>
            internal const int NumOraclesOffset = 8;

            /// <summary>
            /// The offset at which the tokens infos begins.
            /// </summary>
            internal const int TokensOffset = 16;

            /// <summary>
            /// The offset at which the spot market infos begins.
            /// </summary>
            internal const int SpotMarketsOffset = 1168;

            /// <summary>
            /// The offset at which the perp markets infos begins.
            /// </summary>
            internal const int PerpMarketsOffset = 2848;

            /// <summary>
            /// The offset at which the oracles public keys begins.
            /// </summary>
            internal const int OraclesOffset = 5248;

            /// <summary>
            /// The offset at which the signer nonce value begins.
            /// </summary>
            internal const int SignerNonceOffset = 5728;

            /// <summary>
            /// The offset at which the signer key begins.
            /// </summary>
            internal const int SignerKeyOffset = 5736;

            /// <summary>
            /// The offset at which the admin key begins.
            /// </summary>
            internal const int AdminKeyOffset = 5768;

            /// <summary>
            /// The offset at which the dex program begins.
            /// </summary>
            internal const int DexProgramKeyOffset = 5800;

            /// <summary>
            /// The offset at which the mango cache begins.
            /// </summary>
            internal const int MangoCacheKeyOffset = 5832;

            /// <summary>
            /// The offset at which the valid interval value begins.
            /// </summary>
            internal const int ValidInternalOffset = 5864;

            /// <summary>
            /// The offset at which the insurance vault begins.
            /// </summary>
            internal const int InsuranceVaultKeyOffset = 5872;

            /// <summary>
            /// The offset at which the serum vault begins
            /// </summary>
            internal const int SerumVaultOffset = 5904;

            /// <summary>
            /// The offset at which the megaserum vault begins.
            /// </summary>
            internal const int MegaSerumVaultOffset = 5936;

            /// <summary>
            /// The offset at which the fee vault begins.
            /// </summary>
            internal const int FeesVaultOffset = 5968;

            /// <summary>
            /// The offset at which the max mango accounts value begins.
            /// </summary>
            internal const int MaxMangoAccountsOffset = 6000;

            /// <summary>
            /// The offset at which the number of mango accounts begins.
            /// </summary>
            internal const int NumMangoAccountsOffset = 6004;
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
        /// The signer nonce.
        /// </summary>
        public ulong SignerNonce;

        /// <summary>
        /// The signer key.
        /// </summary>
        public PublicKey SignerKey;

        /// <summary>
        /// The admin of the mango group.
        /// </summary>
        public PublicKey Admin;

        /// <summary>
        /// The dex program id.
        /// </summary>
        public PublicKey DexProgramId;

        /// <summary>
        /// The mango cache.
        /// </summary>
        public PublicKey MangoCache;

        /// <summary>
        /// The valid interval.
        /// </summary>
        public ulong ValidInterval;

        /// <summary>
        /// The insurance vault.
        /// </summary>
        public PublicKey InsuranceVault;

        /// <summary>
        /// The serum vault.
        /// </summary>
        public PublicKey SerumVault;

        /// <summary>
        /// The mega serum vault.
        /// </summary>
        public PublicKey MegaSerumVault;

        /// <summary>
        /// The fee vault.
        /// </summary>
        public PublicKey FeesVault;

        /// <summary>
        /// The loaded root banks.
        /// </summary>
        public List<RootBank> RootBankAccounts;

        /// <summary>
        /// The loaded root banks.
        /// </summary>
        public List<PerpMarket> PerpMarketAccounts;

        /// <summary>
        /// Maximum number of <see cref="MangoAccount"/>s with <see cref="MetaData.Version"/> 1.
        /// </summary>
        public uint MaxMangoAccounts;

        /// <summary>
        /// The number of <see cref="MangoAccount"/>s with <see cref="MetaData.Version"/> 1.
        /// </summary>
        public uint NumMangoAccounts;

        /// <summary>
        /// The mango group's config.
        /// </summary>
        public MangoGroupConfig Config;

        /// <summary>
        /// Loads the perp markets for this mango group. This is an asynchronous operation.
        /// </summary>
        /// <param name="mangoClient">A mango client instance.</param>
        /// <param name="logger">A logger instance.</param>
        public async Task<MultipleAccountsResultWrapper<List<PerpMarket>>> LoadPerpMarketsAsync(IMangoClient mangoClient, ILogger logger = null)
        {
            List<PublicKey> filteredPerpMarkets = PerpetualMarkets
                .Where(x => !x.Market.Equals(SystemProgram.ProgramIdKey))
                .Select(x => x.Market).ToList();
            MultipleAccountsResultWrapper<List<PerpMarket>> perpMarketAccounts =
                await mangoClient.GetPerpMarketsAsync(filteredPerpMarkets);
            if (!perpMarketAccounts.OriginalRequest.WasSuccessful)
            {
                logger?.LogInformation($"Could not fetch perp market accounts.");
                return perpMarketAccounts;
            }
            logger?.LogInformation(
                $"Successfully fetched {perpMarketAccounts.ParsedResult.Count} perp market accounts.");

            int i = 0;
            PerpetualMarkets.ForEach(key =>
            {
                int keyIndex = filteredPerpMarkets.IndexOf(key.Market);
                if (keyIndex == -1)
                {
                    PerpMarketAccounts.Add(null);
                    return;
                }
                var perpMarket = perpMarketAccounts.ParsedResult[keyIndex];
                if (Config != null)
                {
                    perpMarket.SetConfig(Config.PerpMarkets[i++]);
                }
                PerpMarketAccounts.Add(perpMarket);
            });

            return perpMarketAccounts;
        }

        /// <summary>
        /// Loads the perp markets for this mango group.
        /// </summary>
        /// <param name="mangoClient">A mango client instance.</param>
        /// <param name="logger">A logger instance.</param>
        public MultipleAccountsResultWrapper<List<PerpMarket>> LoadPerpMarkets(IMangoClient mangoClient, ILogger logger = null) =>
            LoadPerpMarketsAsync(mangoClient, logger).Result;

        /// <summary>
        /// Loads the root banks for this root bank. This is an asynchronous operation.
        /// </summary>
        /// <param name="mangoClient">A mango client instance.</param>
        /// <param name="logger">A logger instance.</param>
        public async Task<MultipleAccountsResultWrapper<List<RootBank>>> LoadRootBanksAsync(IMangoClient mangoClient, ILogger logger = null)
        {
            List<PublicKey> filteredRootBanks = Tokens
                .Where(x => !x.RootBank.Equals(SystemProgram.ProgramIdKey))
                .Select(x => x.RootBank).ToList();
            MultipleAccountsResultWrapper<List<RootBank>> rootBankAccounts =
                await mangoClient.GetRootBanksAsync(filteredRootBanks);
            if (!rootBankAccounts.WasSuccessful)
            {
                logger?.LogInformation($"Could not fetch root bank accounts.");
                return rootBankAccounts;
            }
            logger?.LogInformation(
                $"Successfully fetched {rootBankAccounts.ParsedResult.Count} root bank accounts.");

            foreach(var key in Tokens)
            {
                int keyIndex = filteredRootBanks.IndexOf(key.RootBank);
                if (keyIndex == -1)
                {
                    RootBankAccounts.Add(null);
                    continue;
                }
                await rootBankAccounts.ParsedResult[keyIndex].LoadNodeBanksAsync(mangoClient, logger);
                RootBankAccounts.Add(rootBankAccounts.ParsedResult[keyIndex]);
            }

            return rootBankAccounts;
        }

        /// <summary>
        /// Loads the root banks for this root bank.
        /// </summary>
        /// <param name="mangoClient">A mango client instance.</param>
        /// <param name="logger">A logger instance.</param>
        public MultipleAccountsResultWrapper<List<RootBank>> LoadRootBanks(IMangoClient mangoClient, ILogger logger = null) =>
            LoadRootBanksAsync(mangoClient, logger).Result;

        /// <summary>
        /// Gets the index for the given oracle <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="oracle">The oracle public key.</param>
        /// <returns>The index.</returns>
        public int GetOracleIndex(PublicKey oracle)
        {
            for (int i = 0; i < (int)NumOracles; i++)
            {
                if (Oracles[i].Equals(oracle)) return i;
            }

            throw new Exception("This Oracle does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given spot market <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="spotMarket">The spot market public key.</param>
        /// <returns>The index.</returns>
        public int GetSpotMarketIndex(PublicKey spotMarket)
        {
            for (int i = 0; i < (int)NumOracles; i++)
            {
                if (SpotMarkets[i].Market.Equals(spotMarket)) return i;
            }

            throw new Exception("This Market does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given perp market <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="perpMarket">The perp market public key.</param>
        /// <returns>The index.</returns>
        public int GetPerpMarketIndex(PublicKey perpMarket)
        {
            for (int i = 0; i < (int)NumOracles; i++)
            {
                if (PerpetualMarkets[i].Market.Equals(perpMarket)) return i;
            }

            throw new Exception("This PerpMarket does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given token mint <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="tokenMint">The token mint public key.</param>
        /// <returns>The index.</returns>
        public int GetTokenIndex(PublicKey tokenMint)
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].Mint.Equals(tokenMint)) return i;
            }

            throw new Exception("This Token Mint does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the index for the given root bank <see cref="PublicKey"/>.
        /// </summary>
        /// <param name="rootBank">The root bank public key.</param>
        /// <returns>The index.</returns>
        public int GetRootBankIndex(PublicKey rootBank)
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].RootBank.Equals(rootBank)) return i;
            }

            throw new Exception("This Root Bank does not belong to this MangoGroup");
        }

        /// <summary>
        /// Gets the borrow rate for the given token index.
        /// </summary>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The borrow rate.</returns>
        public I80F48 GetBorrowRate(int tokenIndex)
        {
            RootBank rootBank = RootBankAccounts[tokenIndex];
            if (rootBank == null)
                throw new Exception($"Root Bank at index {tokenIndex} has not been loaded");

            return rootBank.GetBorrowRate(Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the borrow rate for the given token index.
        /// </summary>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The deposit rate.</returns>
        public I80F48 GetDepositRate(int tokenIndex)
        {
            RootBank rootBank = RootBankAccounts[tokenIndex];
            if (rootBank == null)
                throw new Exception($"Root Bank at index {tokenIndex} has not been loaded");

            return rootBank.GetDepositRate(Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Humanizes a cache price.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The humanized value.</returns>
        public I80F48 HumanizeCachePrice(I80F48 price, int tokenIndex)
        {
            var decimalAdj = (decimal) Math.Pow(10, Tokens[tokenIndex].Decimals - Tokens[Constants.QuoteIndex].Decimals);
            return price * new I80F48(decimalAdj);
        }

        /// <summary>
        /// Gets the price for the given token index.
        /// </summary>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The price.</returns>
        public I80F48 GetPrice(MangoCache mangoCache, int tokenIndex)
        {
            if (tokenIndex == Constants.QuoteIndex) return new(1m);
            var decimalAdj = (decimal) Math.Pow(10, Tokens[tokenIndex].Decimals - Tokens[Constants.QuoteIndex].Decimals);

            return mangoCache.PriceCaches[tokenIndex].Price * new I80F48(decimalAdj);
        }

        /// <summary>
        /// Gets the native price for the given token index.
        /// </summary>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The price.</returns>
        public I80F48 GetPriceNative(MangoCache mangoCache, int tokenIndex)
        {
            if (tokenIndex == Constants.QuoteIndex) return new(1m);
            return mangoCache.PriceCaches[tokenIndex].Price;
        }

        /// <summary>
        /// Gets the total deposit value humanized.
        /// </summary>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The deposit amount.</returns>
        public I80F48 GetUiTotalDeposit(int tokenIndex)
        {
            RootBank rootBank = RootBankAccounts[tokenIndex];
            if (rootBank == null)
                throw new Exception($"Root Bank at index {tokenIndex} has not been loaded");

            return rootBank.GetUiTotalDeposit(Tokens[tokenIndex].Decimals);
        }

        /// <summary>
        /// Gets the total borrow value humanized.
        /// </summary>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The borrow amount.</returns>
        public I80F48 GetUiTotalBorrow(int tokenIndex)
        {
            RootBank rootBank = RootBankAccounts[tokenIndex];
            if (rootBank == null)
                throw new Exception($"Root Bank at index {tokenIndex} has not been loaded");

            return rootBank.GetUiTotalBorrow(Tokens[tokenIndex].Decimals);
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
        /// Adds the config to a mango group.
        /// </summary>
        /// <param name="config">The mango group's config.</param>
        public void SetConfig(MangoGroupConfig config)
        {
            if (config == null) return;
            if (Config == null)
                Config = config;
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="MangoGroup"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="MangoGroup"/> structure.</returns>
        public static MangoGroup Deserialize(byte[] data)
        {
            if(data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

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
                FeesVault = span.GetPubKey(Layout.FeesVaultOffset),
                RootBankAccounts = new List<RootBank>(Constants.MaxTokens),
                PerpMarketAccounts = new List<PerpMarket>(Constants.MaxTokens),
                MaxMangoAccounts = span.GetU32(Layout.MaxMangoAccountsOffset),
                NumMangoAccounts = span.GetU32(Layout.NumMangoAccountsOffset)
            };
        }
    }
}