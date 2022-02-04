using Solnet.Mango.Models.Perpetuals;
using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models.Perpetuals
{
    /// <summary>
    /// Represents the liquidity mining info for a <see cref="PerpMarketInfo"/>.
    /// </summary>
    public class LiquidityMiningInfo
    {
        /// <summary>
        /// The layout of the <see cref="LiquidityMiningInfo"/> structure.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="LiquidityMiningInfo"/> structure.
            /// </summary>
            internal const int Length = 64;

            /// <summary>
            /// The offset at which the earning rate value begins.
            /// </summary>
            internal const int RateOffset = 0;

            /// <summary>
            /// The offset at which the maximum depth in bps value begins.
            /// </summary>
            internal const int MaxDepthBasisOffset = 16;

            /// <summary>
            /// The offset at which the period start value begins.
            /// </summary>
            internal const int PeriodStartOffset = 32;

            /// <summary>
            /// The offset at which the target period length begins.
            /// </summary>
            internal const int TargetPeriodLengthOffset = 40;

            /// <summary>
            /// The offset at which the MNGO left begins.
            /// </summary>
            internal const int MangoLeftOffset = 48;

            /// <summary>
            /// The offset at which the MNGO per period begins.
            /// </summary>
            internal const int MangoPerPeriodOffset = 56;
        }

        /// <summary>
        /// The earning rate.
        /// </summary>
        public I80F48 Rate;

        /// <summary>
        /// The maximum depth in basis points.
        /// </summary>
        public I80F48 MaxDepthBasis;

        /// <summary>
        /// The period start.
        /// </summary>
        public ulong PeriodStart;

        /// <summary>
        /// The target period length.
        /// </summary>
        public ulong TargetPeriodLength;

        /// <summary>
        /// The amount of MNGO left.
        /// </summary>
        public ulong MangoLeft;

        /// <summary>
        /// The amount of MNGO per period.
        /// </summary>
        public ulong MangoPerPeriod;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="LiquidityMiningInfo"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="LiquidityMiningInfo"/> structure.</returns>
        public static LiquidityMiningInfo Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");

            return new LiquidityMiningInfo
            {
                Rate = I80F48.Deserialize(data.Slice(Layout.RateOffset, I80F48.Length)),
                MaxDepthBasis = I80F48.Deserialize(data.Slice(Layout.MaxDepthBasisOffset, I80F48.Length)),
                PeriodStart = data.GetU64(Layout.PeriodStartOffset),
                TargetPeriodLength = data.GetU64(Layout.TargetPeriodLengthOffset),
                MangoLeft = data.GetU64(Layout.MangoLeftOffset),
                MangoPerPeriod = data.GetU64(Layout.MangoPerPeriodOffset)
            };
        }
    }
}