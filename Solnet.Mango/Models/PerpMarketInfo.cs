using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents information about a perpetual market in Mango.
    /// </summary>
    public class PerpMarketInfo : SpotMarketInfo
    {
        /// <summary>
        /// The layout of the <see cref="PerpMarketInfo"/>.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The length of the <see cref="PerpMarketInfo"/> structure.
            /// </summary>
            internal const int Length = 160;

            /// <summary>
            /// The offset at which the value for the maker fee starts.
            /// </summary>
            internal const int MakerFeeOffset = 112;

            /// <summary>
            /// The offset at which the value for the taker fee starts.
            /// </summary>
            internal const int TakerFeeOffset = 128;

            /// <summary>
            /// The offset at which the value for the base lot size starts.
            /// </summary>
            internal const int BaseLotSizeOffset = 144;

            /// <summary>
            /// The offset at which the value for the quote lot size starts.
            /// </summary>
            internal const int QuoteLotSizeOffset = 152;
        }

        /// <summary>
        /// The taker fee.
        /// </summary>
        public I80F48 TakerFee;
        
        /// <summary>
        /// The maker fee.
        /// </summary>
        public I80F48 MakerFee;
        
        /// <summary>
        /// The base lot size.
        /// </summary>
        public long BaseLotSize;

        /// <summary>
        /// The quote lot size.
        /// </summary>
        public long QuoteLotSize;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PerpMarketInfo"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PerpMarketInfo"/> structure.</returns>
        public static new PerpMarketInfo Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != ExtraLayout.Length) throw new ArgumentException("data length is invalid");
            
            return new PerpMarketInfo
            {
                Market = data.GetPubKey(Layout.MarketOffset),
                MaintenanceAssetWeight = I80F48.Deserialize(data.GetSpan(Layout.MaintenanceAssetWeightOffset, I80F48.Length)),
                InitializationAssetWeight = I80F48.Deserialize(data.GetSpan(Layout.InitializationAssetWeightOffset, I80F48.Length)),
                MaintenanceLiabilityWeight = I80F48.Deserialize(data.GetSpan(Layout.MaintenanceLiabilityWeightOffset, I80F48.Length)),
                InitializationLiabilityWeight = I80F48.Deserialize(data.GetSpan(Layout.InitializationLiabilityWeightOffset, I80F48.Length)),
                LiquidationFee = I80F48.Deserialize(data.GetSpan(Layout.LiquidationFeeOffset, I80F48.Length)),
                MakerFee = I80F48.Deserialize(data.GetSpan(ExtraLayout.MakerFeeOffset, I80F48.Length)),
                TakerFee = I80F48.Deserialize(data.GetSpan(ExtraLayout.TakerFeeOffset, I80F48.Length)),
                BaseLotSize = data.GetS64(ExtraLayout.BaseLotSizeOffset),
                QuoteLotSize = data.GetS64(ExtraLayout.QuoteLotSizeOffset)
            };
        }
    }
}