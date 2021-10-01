using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents information about a cross-collateralized spot market in Mango.
    /// </summary>
    public class SpotMarketInfo
    {
        /// <summary>
        /// The layout of the <see cref="SpotMarketInfo"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="SpotMarketInfo"/> structure.
            /// </summary>
            internal const int Length = 112;
            
            /// <summary>
            /// The offset at which the market public key starts.
            /// </summary>
            internal const int MarketOffset = 0;
            
            /// <summary>
            /// The offset at which the asset's maintenance weight starts.
            /// </summary>
            internal const int MaintenanceAssetWeightOffset = 32;

            /// <summary>
            /// The offset at which the asset's initialization weight value starts.
            /// </summary>
            internal const int InitializationAssetWeightOffset = 48;

            /// <summary>
            /// The offset at which the maintenance liability weight value starts.
            /// </summary>
            internal const int MaintenanceLiabilityWeightOffset = 64;

            /// <summary>
            /// The offset at which the initialization liability weight value starts.
            /// </summary>
            internal const int InitializationLiabilityWeightOffset = 80;
    
            /// <summary>
            /// The offset at which the liquidation fee value starts.
            /// </summary>
            internal const int LiquidationFeeOffset = 96;
        }
        
        /// <summary>
        /// The public key of the market.
        /// </summary>
        public PublicKey Market;

        /// <summary>
        /// The asset maintenance weight.
        /// </summary>
        public I80F48 MaintenanceAssetWeight;
        
        /// <summary>
        /// The asset initialization weight.
        /// </summary>
        public I80F48 InitializationAssetWeight;
        
        /// <summary>
        /// The maintenance liability weight.
        /// </summary>
        public I80F48 MaintenanceLiabilityWeight;
        
        /// <summary>
        /// The initialization liability weight.
        /// </summary>
        public I80F48 InitializationLiabilityWeight;
        
        /// <summary>
        /// The liquidation fee.
        /// </summary>
        public I80F48 LiquidationFee;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="SpotMarketInfo"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="SpotMarketInfo"/> structure.</returns>
        public static SpotMarketInfo Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
            
            return new SpotMarketInfo
            {
                Market = data.GetPubKey(Layout.MarketOffset),
                MaintenanceAssetWeight = I80F48.Deserialize(data.GetSpan(Layout.MaintenanceAssetWeightOffset, I80F48.Length)),
                InitializationAssetWeight = I80F48.Deserialize(data.GetSpan(Layout.InitializationAssetWeightOffset, I80F48.Length)),
                MaintenanceLiabilityWeight = I80F48.Deserialize(data.GetSpan(Layout.MaintenanceLiabilityWeightOffset, I80F48.Length)),
                InitializationLiabilityWeight = I80F48.Deserialize(data.GetSpan(Layout.InitializationLiabilityWeightOffset, I80F48.Length)),
                LiquidationFee = I80F48.Deserialize(data.GetSpan(Layout.LiquidationFeeOffset, I80F48.Length)),
            };
        }
    }
}