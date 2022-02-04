using Solnet.Mango.Models.Caches;
using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models.Perpetuals
{
    /// <summary>
    /// Represents a perp account for a <see cref="MangoAccount"/>,
    /// </summary>
    public class PerpAccount
    {
        /// <summary>
        /// Represents the layout of the <see cref="PerpAccount"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="PerpAccount"/> structure.
            /// </summary>
            internal const int Length = 96;

            /// <summary>
            /// The offset at which the base position value begins.
            /// </summary>
            internal const int BasePositionOffset = 0;

            /// <summary>
            /// The offset at which the quote position value begins.
            /// </summary>
            internal const int QuotePositionOffset = 8;

            /// <summary>
            /// The offset at which the long settled funding value begins.
            /// </summary>
            internal const int LongSettledFundingOffset = 24;

            /// <summary>
            /// The offset at which the short settled funding value begins.
            /// </summary>
            internal const int ShortSettledFundingOffset = 40;

            /// <summary>
            /// The offset at which the bids quantity value begins.
            /// </summary>
            internal const int BidsQuantityOffset = 56;

            /// <summary>
            /// The offset at which the asks quantity value begins.
            /// </summary>
            internal const int AsksQuantityOffset = 64;

            /// <summary>
            /// The offset at which the taker base value begins.
            /// </summary>
            internal const int TakerBaseOffset = 72;

            /// <summary>
            /// The offset at which the taker quote value begins.
            /// </summary>
            internal const int TakerQuoteOffset = 80;

            /// <summary>
            /// The offset at which the mango accrued value begins.
            /// </summary>
            internal const int MngoAccruedOffset = 88;
        }

        /// <summary>
        /// The base position.
        /// </summary>
        public long BasePosition;

        /// <summary>
        /// The quote position.
        /// </summary>
        public I80F48 QuotePosition;

        /// <summary>
        /// The funding settled for long positions.
        /// </summary>
        public I80F48 LongSettledFunding;

        /// <summary>
        /// The funding settled for short positions.
        /// </summary>
        public I80F48 ShortSettledFunding;

        /// <summary>
        /// The amount of bids.
        /// </summary>
        public long BidsQuantity;

        /// <summary>
        /// The amount of asks.
        /// </summary>
        public long AsksQuantity;

        /// <summary>
        /// The taker base.
        /// </summary>
        public long TakerBase;

        /// <summary>
        /// The taker quote.
        /// </summary>
        public long TakerQuote;

        /// <summary>
        /// The native amount of mangno accrued.
        /// </summary>
        public ulong MngoAccrued;

        /// <summary>
        /// Gets profit and loss adjusted for unsettled funding.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="market">The market.</param>
        /// <param name="breakEvenPrice">The break-even price.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The adjusted PnL.</returns>
        public decimal GetProfitAndLoss(MangoGroup mangoGroup, MangoCache mangoCache, PerpMarket market, I80F48 breakEvenPrice, int tokenIndex)
        {
            return market.BaseLotsToNumber(BasePosition, mangoGroup.Tokens[tokenIndex].Decimals) * 
                (mangoGroup.GetPrice(mangoCache, tokenIndex) - breakEvenPrice).ToDecimal();
        }

        /// <summary>
        /// Gets the amount of unsettled funding.
        /// </summary>
        /// <param name="perpMarketCache">The perpetual market's cache.</param>
        /// <returns>The unsettled funding.</returns>
        public I80F48 GetUnsettledFunding(PerpMarketCache perpMarketCache)
        {
            if (BasePosition < 0)
            {
                return new I80F48((decimal) BasePosition) * (perpMarketCache.ShortFunding - ShortSettledFunding);
            }
            return new I80F48((decimal) BasePosition) * (perpMarketCache.LongFunding - LongSettledFunding);
        }

        /// <summary>
        /// Gets the quote position after adjusting for unsettled funding.
        /// </summary>
        /// <param name="perpMarketCache">The perpetual market's cache.</param>
        /// <returns>The quote position.</returns>
        public I80F48 GetQuotePosition(PerpMarketCache perpMarketCache)
        {
            return QuotePosition - GetUnsettledFunding(perpMarketCache);
        }

        /// <summary>
        /// Gets the notional size of the position.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="market">The market.</param>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The notional size of the position.</returns>
        public decimal GetNotionalSize(MangoGroup mangoGroup, MangoCache mangoCache, PerpMarket market, int tokenIndex)
            => GetUiBasePosition(market, mangoGroup.Tokens[tokenIndex].Decimals) * mangoGroup.GetPrice(mangoCache, tokenIndex).ToDecimal();

        /// <summary>
        /// Gets the base position value converted for ui display.
        /// </summary>
        /// <param name="market">The market.</param>
        /// <param name="baseDecimals">The decimals of the base token.</param>
        /// <returns>The base position size for ui display.</returns>
        public decimal GetUiBasePosition(PerpMarket market, byte baseDecimals) =>
            market.BaseLotsToNumber(BasePosition, baseDecimals);

        /// <summary>
        /// Simulates the position health against the given base position change.
        /// </summary>
        /// <param name="perpMarketInfo">The perpetual market's info.</param>
        /// <param name="price">The current price.</param>
        /// <param name="assetWeight">The asset weight.</param>
        /// <param name="liabilityWeight">The liability weight.</param>
        /// <param name="baseChange">The change on the base position.</param>
        /// <returns>The position health.</returns>
        public I80F48 SimulatePositionHealth(PerpMarketInfo perpMarketInfo, I80F48 price,
            I80F48 assetWeight, I80F48 liabilityWeight, long baseChange)
        {
            long newBase = BasePosition + baseChange;
            I80F48 health = QuotePosition - (new I80F48((decimal) (baseChange * perpMarketInfo.BaseLotSize)) * price);

            if (newBase > 0)
            {
                health += new I80F48((decimal)newBase * perpMarketInfo.BaseLotSize) * price * assetWeight;
            }
            else if (newBase < 0)
            {
                health += new I80F48((decimal)newBase * perpMarketInfo.BaseLotSize) * price * liabilityWeight;
            }
            return health;
        }

        /// <summary>
        /// Gets the health of the perpetual account.
        /// </summary>
        /// <param name="perpMarketInfo">The perpetual market's info.</param>
        /// <param name="price">The current price.</param>
        /// <param name="assetWeight">The asset weight.</param>
        /// <param name="liabilityWeight">The liability weight.</param>
        /// <param name="shortFunding">The short funding.</param>
        /// <param name="longFunding">The long funding.</param>
        /// <returns>The health value.</returns>
        public I80F48 GetHealth(PerpMarketInfo perpMarketInfo, I80F48 price, I80F48 assetWeight,
            I80F48 liabilityWeight, I80F48 shortFunding, I80F48 longFunding)
        {
            I80F48 bidsHealth = SimulatePositionHealth(perpMarketInfo, price, assetWeight, liabilityWeight, BidsQuantity);
            I80F48 asksHealth = SimulatePositionHealth(perpMarketInfo, price, assetWeight, liabilityWeight, AsksQuantity * -1);
            I80F48 health = bidsHealth < asksHealth ? bidsHealth : asksHealth;

            I80F48 x = I80F48.Zero;

            if (BasePosition > 0)
            {
                x = health - ((longFunding - LongSettledFunding) * new I80F48((decimal)BasePosition));
            }
            else if (BasePosition < 0)
            {
                x = health + ((shortFunding - ShortSettledFunding) * new I80F48((decimal)BasePosition));
            }

            return x;
        }

        /// <summary>
        /// Gets the liabilities values for this <see cref="PerpAccount"/> and given <see cref="PerpMarketInfo"/>.
        /// </summary>
        /// <param name="perpMarketInfo">The perpetual market's info.</param>
        /// <param name="price">The current price.</param>
        /// <param name="shortFunding">The short funding.</param>
        /// <param name="longFunding">The long funding.</param>
        /// <returns>The liabilities value.</returns>
        public I80F48 GetLiabilitiesValue(PerpMarketInfo perpMarketInfo, I80F48 price,
            I80F48 shortFunding, I80F48 longFunding)
        {
            I80F48 liabsValue = I80F48.Zero;

            if (BasePosition < 0)
            {
                liabsValue += ((new I80F48((decimal)BasePosition * perpMarketInfo.BaseLotSize)) * price);
            }

            I80F48 realQuotePosition = QuotePosition;

            if (BasePosition > 0)
            {
                realQuotePosition = QuotePosition - ((longFunding - LongSettledFunding) * new I80F48((decimal)BasePosition));
            }
            else if (BasePosition < 0)
            {
                realQuotePosition = QuotePosition - ((shortFunding - ShortSettledFunding) * new I80F48((decimal)BasePosition));
            }

            if (realQuotePosition < I80F48.Zero)
            {
                liabsValue += realQuotePosition;
            }
            return liabsValue * I80F48.NegativeOne;
        }

        /// <summary>
        /// Gets the asset values for this <see cref="PerpAccount"/> and given <see cref="PerpMarketInfo"/>.
        /// </summary>
        /// <param name="perpMarketInfo">The perpetual market's info.</param>
        /// <param name="price">The current price.</param>
        /// <param name="shortFunding">The short funding.</param>
        /// <param name="longFunding">The long funding.</param>
        /// <returns>The assets value.</returns>
        public I80F48 GetAssetValue(PerpMarketInfo perpMarketInfo, I80F48 price, I80F48 shortFunding, I80F48 longFunding)
        {
            I80F48 assetsValue = I80F48.Zero;

            if (BasePosition > 0)
            {
                assetsValue += ((new I80F48((decimal)BasePosition * perpMarketInfo.BaseLotSize)) * price);
            }

            I80F48 realQuotePosition = QuotePosition;

            if (BasePosition > 0)
            {
                realQuotePosition = QuotePosition - ((longFunding - LongSettledFunding) * new I80F48((decimal)BasePosition));
            }
            else if (BasePosition < 0)
            {
                realQuotePosition = QuotePosition - ((shortFunding - ShortSettledFunding) * new I80F48((decimal)BasePosition));
            }

            if (realQuotePosition > I80F48.Zero)
            {
                assetsValue += realQuotePosition;
            }
            return assetsValue;
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PerpAccount"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PerpAccount"/> structure.</returns>
        public static PerpAccount Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");
            return new PerpAccount
            {
                BasePosition = data.GetS64(Layout.BasePositionOffset),
                QuotePosition = I80F48.Deserialize(data.GetSpan(Layout.QuotePositionOffset, I80F48.Length)),
                LongSettledFunding = I80F48.Deserialize(data.GetSpan(Layout.LongSettledFundingOffset, I80F48.Length)),
                ShortSettledFunding = I80F48.Deserialize(data.GetSpan(Layout.ShortSettledFundingOffset, I80F48.Length)),
                BidsQuantity = data.GetS64(Layout.BidsQuantityOffset),
                AsksQuantity = data.GetS64(Layout.AsksQuantityOffset),
                TakerBase = data.GetS64(Layout.TakerBaseOffset),
                TakerQuote = data.GetS64(Layout.TakerQuoteOffset),
                MngoAccrued = data.GetU64(Layout.MngoAccruedOffset)
            };
        }
    }
}