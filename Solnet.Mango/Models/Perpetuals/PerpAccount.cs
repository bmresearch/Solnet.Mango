using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
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
        /// <param name="perpMarketInfo">The perpetual market's info.</param>
        /// <param name="perpMarketCache">The perpetual market's cache.</param>
        /// <param name="price">The current price.</param>
        /// <returns>The PNL value.</returns>
        public double GetProfitAndLoss(PerpMarketInfo perpMarketInfo, PerpMarketCache perpMarketCache, double price)
        {
            return BasePosition * perpMarketInfo.BaseLotSize * price + GetQuotePosition(perpMarketCache);

        }

        /// <summary>
        /// Gets the amount of unsettled funding.
        /// </summary>
        /// <param name="perpMarketCache">The perpetual market's cache.</param>
        /// <returns>The unsettled funding.</returns>
        public double GetUnsettledFunding(PerpMarketCache perpMarketCache)
        {
            if (BasePosition < 0)
            {
                return BasePosition * (perpMarketCache.ShortFunding.Value - ShortSettledFunding.Value);
            }
            return BasePosition * (perpMarketCache.LongFunding.Value - LongSettledFunding.Value);
        }

        /// <summary>
        /// Gets the quote position after adjusting for unsettled funding.
        /// </summary>
        /// <param name="perpMarketCache">The perpetual market's cache.</param>
        /// <returns>The quote position.</returns>
        public double GetQuotePosition(PerpMarketCache perpMarketCache)
        {
            return QuotePosition.Value - GetUnsettledFunding(perpMarketCache);
        }

        /// <summary>
        /// Simulates the position health against the given base position change.
        /// </summary>
        /// <param name="perpMarketInfo">The perpetual market's info.</param>
        /// <param name="price">The current price.</param>
        /// <param name="assetWeight">The asset weight.</param>
        /// <param name="liabilityWeight">The liability weight.</param>
        /// <param name="baseChange">The change on the base position.</param>
        /// <returns>The position health.</returns>
        public double SimulatePositionHealth(PerpMarketInfo perpMarketInfo, double price,
            double assetWeight, double liabilityWeight, double baseChange)
        {
            double newBase = BasePosition + baseChange;
            double health = QuotePosition.Value - ((baseChange * perpMarketInfo.BaseLotSize) * price);

            if (newBase > 0)
            {
                health += newBase * perpMarketInfo.BaseLotSize * price * assetWeight;
            }
            else if (newBase < 0)
            {
                health += newBase * perpMarketInfo.BaseLotSize * price * liabilityWeight;
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
        public double GetHealth(PerpMarketInfo perpMarketInfo, double price, double assetWeight,
            double liabilityWeight, double shortFunding, double longFunding)
        {
            double bidsHealth = SimulatePositionHealth(perpMarketInfo, price, assetWeight, liabilityWeight, BidsQuantity);
            double asksHealth = SimulatePositionHealth(perpMarketInfo, price, assetWeight, liabilityWeight, -AsksQuantity);
            double health = bidsHealth < asksHealth ? bidsHealth : asksHealth;

            double x = 0;

            if(BasePosition > 0)
            {
                x = health - ((longFunding - LongSettledFunding.Value) * BasePosition);
            }
            else if(BasePosition < 0)
            {
                x = health + ((shortFunding - ShortSettledFunding.Value) * BasePosition);
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
        public double GetLiabilitiesValue(PerpMarketInfo perpMarketInfo, double price,
            double shortFunding, double longFunding)
        {
            double liabsValue = 0;

            if (BasePosition < 0)
            {
                liabsValue += ((BasePosition * perpMarketInfo.BaseLotSize) * price);
            }

            double realQuotePosition = QuotePosition.Value;

            if (BasePosition > 0)
            {
                realQuotePosition = QuotePosition.Value - ((longFunding - LongSettledFunding.Value) * BasePosition);
            }
            else if (BasePosition < 0)
            {
                realQuotePosition = QuotePosition.Value - ((shortFunding - ShortSettledFunding.Value) * BasePosition);
            }

            if (realQuotePosition < 0)
            {
                liabsValue += realQuotePosition;
            }
            return liabsValue;
        }

        /// <summary>
        /// Gets the asset values for this <see cref="PerpAccount"/> and given <see cref="PerpMarketInfo"/>.
        /// </summary>
        /// <param name="perpMarketInfo">The perpetual market's info.</param>
        /// <param name="price">The current price.</param>
        /// <param name="shortFunding">The short funding.</param>
        /// <param name="longFunding">The long funding.</param>
        /// <returns>The assets value.</returns>
        public double GetAssetValue(PerpMarketInfo perpMarketInfo, double price, double shortFunding, double longFunding)
        {
            double assetsValue = 0;

            if (BasePosition > 0)
            {
                assetsValue += ((BasePosition * perpMarketInfo.BaseLotSize) * price);
            }

            double realQuotePosition = QuotePosition.Value;

            if (BasePosition > 0)
            {
                realQuotePosition = QuotePosition.Value - ((longFunding - LongSettledFunding.Value) * BasePosition);
            }
            else if (BasePosition < 0)
            {
                realQuotePosition = QuotePosition.Value - ((shortFunding - ShortSettledFunding.Value) * BasePosition);
            }

            if (realQuotePosition > 0)
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