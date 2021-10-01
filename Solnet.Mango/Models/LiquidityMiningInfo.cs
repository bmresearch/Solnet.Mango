using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LiquidityMiningInfo
    {
        /// <summary>
        /// 
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            internal const int Length = 64;
            
            /// <summary>
            /// 
            /// </summary>
            internal const int RateOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int MaxDepthBasisOffset = 16;

            /// <summary>
            /// 
            /// </summary>
            internal const int PeriodStartOffset = 32;

            /// <summary>
            /// 
            /// </summary>
            internal const int TargetPeriodLengthOffset = 40;

            /// <summary>
            /// 
            /// </summary>
            internal const int MangoLeftOffset = 48;

            /// <summary>
            /// 
            /// </summary>
            internal const int MangoPerPeriodOffset = 56;
        }

        /// <summary>
        /// 
        /// </summary>
        public I80F48 Rate;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 MaxDepthBasis;

        /// <summary>
        /// 
        /// </summary>
        public ulong PeriodStart;

        /// <summary>
        /// 
        /// </summary>
        public ulong TargetPeriodLength;

        /// <summary>
        /// 
        /// </summary>
        public ulong MangoLeft;

        /// <summary>
        /// 
        /// </summary>
        public ulong MangoPerPeriod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static LiquidityMiningInfo Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new Exception("data length is invalid");

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