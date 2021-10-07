using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a <see cref="EventType.Out"/> in the Mango <see cref="EventQueue"/>.
    /// </summary>
    public class OutEvent : Event
    {
        /// <summary>
        /// Represents the layout of the <see cref="OutEvent"/>.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the side value begins.
            /// </summary>
            internal const int SideOffset = 1;

            /// <summary>
            /// The offset at which the slot value begins.
            /// </summary>
            internal const int SlotOffset = 2;

            /// <summary>
            /// The offset at which the owner public key value begins.
            /// </summary>
            internal const int OwnerOffset = 24;

            /// <summary>
            /// The offset at which the quantity value begins.
            /// </summary>
            internal const int QuantityOffset = 56;
        }
        
        /// <summary>
        /// The side from the taker's point of view.
        /// </summary>
        public Side Side;

        /// <summary>
        /// The slot of the order in the maker's orders.
        /// </summary>
        public byte Slot;

        /// <summary>
        /// The public key of the order owner.
        /// </summary>
        public PublicKey Owner;

        /// <summary>
        /// The quantity that was filled.
        /// </summary>
        public long Quantity;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="OutEvent"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="OutEvent"/> structure.</returns>
        public static OutEvent Deserialize(ReadOnlySpan<byte> data)
        {
            return new OutEvent
            {
                EventType = (EventType) Enum.Parse(typeof(EventType), data.GetU8(Layout.EventTypeOffset).ToString()),
                Side = (Side) Enum.Parse(typeof(Side), data.GetU8(ExtraLayout.SideOffset).ToString()),
                Slot = data.GetU8(ExtraLayout.SlotOffset),
                Owner = data.GetPubKey(ExtraLayout.OwnerOffset),
                Quantity = data.GetS64(ExtraLayout.QuantityOffset)
            };
        }
    }
}