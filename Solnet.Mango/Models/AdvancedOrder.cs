using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// The base advanced order.
    /// </summary>
    public abstract class AdvancedOrder
    {
        /// <summary>
        /// The Layout of the <see cref="AdvancedOrder"/>
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the advanced orders.
            /// </summary>
            internal const int Length = 80;

            /// <summary>
            /// The offset at which the order type begins.
            /// </summary>
            internal const int OrderTypeOffset = 0;

            /// <summary>
            /// The offset at which the boolean which defines if the order is active begins.
            /// </summary>
            internal const int IsActiveOffset = 1;
        }

        /// <summary>
        /// The order type.
        /// </summary>
        public AdvancedOrderType AdvancedOrderType;

        /// <summary>
        /// Whether the order is active or not.
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="AdvancedOrder"/> instance.
        /// </summary>
        /// <param name="span">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="AdvancedOrder"/> structure.</returns>
        public static AdvancedOrder Deserialize(ReadOnlySpan<byte> span)
        {
            if (span.Length != Layout.Length)
                throw new ArgumentException($"invalid data length, expected {Layout.Length}, got {span.Length}");

            var type = (AdvancedOrderType)Enum.Parse(typeof(AdvancedOrderType), span.GetU8(Layout.OrderTypeOffset).ToString());

            switch (type)
            {
                case AdvancedOrderType.PerpTrigger:
                    return PerpTriggerOrder.Deserialize(span);
                default:
                    return null;
            }
        }
    }
}
