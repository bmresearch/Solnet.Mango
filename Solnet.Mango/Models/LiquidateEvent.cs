using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Mango.Models
{    
    /// <summary>
    /// Represents a <see cref="EventType.Liquidate"/> in the Mango <see cref="EventQueue"/>.
    /// </summary>
    public class LiquidateEvent : Event
    {
        /// <summary>
        /// Represents the layout of the <see cref="LiquidateEvent"/>.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the liquidated account's public key value begins.
            /// </summary>
            internal const int LiquidatedOffset = 24;

            /// <summary>
            /// The offset at which the liquidator account's public key value begins.
            /// </summary>
            internal const int LiquidatorOffset = 56;
            
            /// <summary>
            /// The offset at which the price value begins.
            /// </summary>
            internal const int PriceOffset = 88;
            
            /// <summary>
            /// The offset at which the quantity value begins.
            /// </summary>
            internal const int QuantityOffset = 104;
            
            /// <summary>
            /// The offset at which the liquidation fee value begins.
            /// </summary>
            internal const int LiquidationFeeOffset = 112;
        }
        
        /// <summary>
        /// The liquidated account's public key.
        /// </summary>
        public PublicKey Liquidated;
        
        /// <summary>
        /// The liquidator account's public key.
        /// </summary>
        public PublicKey Liquidator;
        
        /// <summary>
        /// The price at which the liquidation occurred.
        /// </summary>
        public I80F48 Price;
        
        /// <summary>
        /// The quantity that was liquidated.
        /// </summary>
        public long Quantity;
        
        /// <summary>
        /// The liquidation fee.
        /// </summary>
        public I80F48 LiquidationFee;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="LiquidateEvent"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="LiquidateEvent"/> structure.</returns>
        public static LiquidateEvent Deserialize(ReadOnlySpan<byte> data)
        {
            return new LiquidateEvent
            {
                EventType = (EventType) Enum.Parse(typeof(EventType), data.GetU8(Layout.EventTypeOffset).ToString()),
                Liquidated = data.GetPubKey(ExtraLayout.LiquidatedOffset),
                Liquidator = data.GetPubKey(ExtraLayout.LiquidatorOffset),
                Price = I80F48.Deserialize(data.Slice(ExtraLayout.PriceOffset, I80F48.Length)),
                Quantity = data.GetS64(ExtraLayout.QuantityOffset),
                LiquidationFee = I80F48.Deserialize(data.Slice(ExtraLayout.LiquidationFeeOffset, I80F48.Length)),
            };
        }
    }
}