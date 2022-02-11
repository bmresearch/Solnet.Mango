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
        /// The layout of the <see cref="PerpTriggerOrder"/> structure.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The length of the <see cref="PerpTriggerOrder"/> structure.
            /// </summary>
            internal const int Length = 80;

            /// <summary>
            /// The offset at which the market index begins.
            /// </summary>
            internal const int MarketIndexOffset = 2;

            /// <summary>
            /// The offset at which the order type begins.
            /// </summary>
            internal const int OrderTypeOffset = 3;

            /// <summary>
            /// The offset at which the side begins.
            /// </summary>
            internal const int SideOffset = 4;

            /// <summary>
            /// The offset at which the trigger condition begins.
            /// </summary>
            internal const int TriggerConditionOffset = 5;

            /// <summary>
            /// The offset at which the reduce only flag begins.
            /// </summary>
            internal const int ReduceOnlyOffset = 6;

            /// <summary>
            /// The offset at which the client order id begins.
            /// </summary>
            internal const int ClientOrderIdOffset = 8;

            /// <summary>
            /// The offset at which the price begins.
            /// </summary>
            internal const int PriceOffset = 16;

            /// <summary>
            /// The offset at which the quantity begins.
            /// </summary>
            internal const int QuantityOffset = 24;

            /// <summary>
            /// The offset at which the trigger price begins.
            /// </summary>
            internal const int TriggerPriceOffset = 32;
        }

        /// <summary>
        /// The order's market index.
        /// </summary>
        public byte MarketIndex;

        /// <summary>
        /// The order type.
        /// </summary>
        public PerpOrderType OrderType;

        /// <summary>
        /// The side.
        /// </summary>
        public Side Side;

        /// <summary>
        /// The trigger condition.
        /// </summary>
        public TriggerCondition TriggerCondition;

        /// <summary>
        /// Whether it is a reduce only order or not.
        /// </summary>
        public bool ReduceOnly;

        /// <summary>
        /// The client order id.
        /// </summary>
        public ulong ClientOrderId;

        /// <summary>
        /// The price.
        /// </summary>
        public long Price;

        /// <summary>
        /// The quantity.
        /// </summary>
        public long Quantity;

        /// <summary>
        /// The trigger price.
        /// </summary>
        public I80F48 TriggerPrice;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="PerpTriggerOrder"/> instance.
        /// </summary>
        /// <param name="span">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="PerpTriggerOrder"/> structure.</returns>
        public static new PerpTriggerOrder Deserialize(ReadOnlySpan<byte> span)
        {
            if(span.Length != Layout.Length)
                throw new ArgumentException($"invalid data length, expected {Layout.Length}, got {span.Length}");
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
                TriggerPrice = I80F48.Deserialize(span.Slice(ExtraLayout.TriggerPriceOffset, I80F48.Length))
            };
        }
    }
}
