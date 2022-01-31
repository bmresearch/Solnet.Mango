using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents the perp trigger order.
    /// </summary>
    public class PerpTriggerOrder : AdvancedOrder
    {
        /// <summary>
        /// The Layout of the <see cref="PerpTriggerOrder"/>
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The length of the structure.
            /// </summary>
            internal const int Length = 80;

            /// <summary>
            /// 
            /// </summary>
            internal const int MarketIndexOffset = 2;

            /// <summary>
            /// 
            /// </summary>
            internal const int OrderTypeOffset = 3;

            /// <summary>
            /// 
            /// </summary>
            internal const int SideOffset = 4;

            /// <summary>
            /// 
            /// </summary>
            internal const int TriggerConditionOffset = 5;

            /// <summary>
            /// 
            /// </summary>
            internal const int ReduceOnlyOffset = 6;

            /// <summary>
            /// 
            /// </summary>
            internal const int ClientOrderIdOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int PriceOffset = 16;

            /// <summary>
            /// 
            /// </summary>
            internal const int QuantityOffset = 24;

            /// <summary>
            /// 
            /// </summary>
            internal const int TriggerPriceOffset = 32;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte MarketIndex;

        /// <summary>
        /// 
        /// </summary>
        public PerpOrderType OrderType;

        /// <summary>
        /// 
        /// </summary>
        public Side Side;

        /// <summary>
        /// 
        /// </summary>
        public TriggerCondition TriggerCondition;

        /// <summary>
        /// 
        /// </summary>
        public bool ReduceOnly;

        /// <summary>
        /// 
        /// </summary>
        public ulong ClientOrderId;

        /// <summary>
        /// 
        /// </summary>
        public long Price;

        /// <summary>
        /// 
        /// </summary>
        public long Quantity;

        /// <summary>
        /// 
        /// </summary>
        public I80F48 TriggerPrice;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PerpTriggerOrder"/> instance.
        /// </summary>
        /// <param name="span">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PerpTriggerOrder"/> structure.</returns>
        public static new PerpTriggerOrder Deserialize(ReadOnlySpan<byte> span)
        {
            return new PerpTriggerOrder
            {
                AdvancedOrderType = AdvancedOrderType.PerpTrigger,
                IsActive = span.GetBool(Layout.IsActiveOffset),
                MarketIndex = span.GetU8(ExtraLayout.MarketIndexOffset),
                OrderType = (PerpOrderType)Enum.Parse(typeof(PerpOrderType), span.GetU8(ExtraLayout.OrderTypeOffset).ToString()),
                Side = (Side)Enum.Parse(typeof(Side), span.GetU8(ExtraLayout.SideOffset).ToString()),
                TriggerCondition = (TriggerCondition)Enum.Parse(typeof(TriggerCondition), span.GetU8(ExtraLayout.TriggerConditionOffset).ToString()),
                ReduceOnly = span.GetBool(ExtraLayout.ReduceOnlyOffset),
                ClientOrderId = span.GetU64(ExtraLayout.ClientOrderIdOffset),
                Price = span.GetS64(ExtraLayout.PriceOffset),
                Quantity = span.GetS64(ExtraLayout.QuantityOffset),
                TriggerPrice = I80F48.Deserialize(span.Slice(ExtraLayout.TriggerConditionOffset, I80F48.Length))
            };
        }
    }
}
