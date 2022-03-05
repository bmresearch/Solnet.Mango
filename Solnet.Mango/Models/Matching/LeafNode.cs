using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Numerics;

namespace Solnet.Mango.Models.Matching
{
    /// <summary>
    /// Represents a leaf node in the Mango Perpetual Market order book side.
    /// <remarks>
    /// This is similar to Serum's slabs.
    /// </remarks>
    /// </summary>
    public class LeafNode : Node
    {
        /// <summary>
        /// Represents the layout of the <see cref="Node"/> structure.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the owner slot value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int OwnerSlotOffset = 0;

            /// <summary>
            /// The offset at which the order type value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int OrderTypeOffset = 1;

            /// <summary>
            /// The offset at which the version begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int VersionOffset = 2;

            /// <summary>
            /// The offset at which the time in force value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int TimeInForceOffset = 3;

            /// <summary>
            /// The offset at which the order id value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int OrderIdOffset = 4;

            /// <summary>
            /// The offset at which the order id value begins.
            /// <remarks>
            /// This value is only valid after reading the Key property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int PriceOffset = 8;

            /// <summary>
            /// The offset at which the owner public key value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int OwnerOffset = 20;

            /// <summary>
            /// The offset at which the quantity value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int QuantityOffset = 52;

            /// <summary>
            /// The offset at which the client order id value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int ClientOrderIdOffset = 60;

            /// <summary>
            /// The offset at which the best initial value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int BestInitialOffset = 68;

            /// <summary>
            /// The offset at which the timestamp value begins.
            /// <remarks>
            /// This value is only valid after reading the Tag property of the <see cref="Node"/>.
            /// </remarks>
            /// </summary>
            internal const int TimestampOffset = 76;
        }

        /// <summary>
        /// The slot of the order in the owner's account..
        /// </summary>
        public byte OwnerSlot;

        /// <summary>
        /// The order type.
        /// </summary>
        public PerpOrderType OrderType;

        /// <summary>
        /// The version.
        /// </summary>
        public byte Version;

        /// <summary>
        /// The time in force.
        /// <remarks>If value is 0 then order never expires.</remarks>
        /// </summary>
        public byte TimeInForce;

        /// <summary>
        /// The order id.
        /// </summary>
        public BigInteger OrderId;

        /// <summary>
        /// The public key of the owner's mango account.
        /// </summary>
        public PublicKey Owner;

        /// <summary>
        /// The quantity of the order.
        /// </summary>
        public long Quantity;

        /// <summary>
        /// The price of the order.
        /// </summary>
        public long Price;

        /// <summary>
        /// The client's order id.
        /// </summary>
        public ulong ClientOrderId;

        /// <summary>
        /// Liquidity incentive related parameters
        /// Either the best bid or best ask at the time the order was placed
        /// </summary>
        public long BestInitial;

        /// <summary>
        /// The time the order was placed.
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// Whether the order is valid or not.
        /// <remarks>
        /// This is checked by the order's <see cref="TimeInForce"/> value.
        /// If it is equal to<see cref="byte.MinValue"/> the order never expires,
        /// otherwise the order expires at <see cref="Timestamp"/> plus <see cref="byte.MaxValue"/> seconds.</remarks>
        /// </summary>
        /// <returns>true if it is valid, otherwise false.</returns>
        public bool IsValid(ulong timestamp)
        {
            var expiry = Timestamp + TimeInForce;
            return TimeInForce == 0 || timestamp < expiry;
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="LeafNode"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="LeafNode"/> structure.</returns>
        public static new LeafNode Deserialize(ReadOnlySpan<byte> data)
        {
            ReadOnlySpan<byte> key = data.GetSpan(ExtraLayout.OrderIdOffset, Layout.KeyLength);
            long price = key.GetS64(ExtraLayout.PriceOffset);
            return new LeafNode
            {
                Tag = NodeType.LeafNode,
                OwnerSlot = data.GetU8(ExtraLayout.OwnerSlotOffset),
                OrderType = (PerpOrderType) Enum.Parse(typeof(PerpOrderType), data.GetU8(ExtraLayout.OrderTypeOffset).ToString()),
                Version = data.GetU8(ExtraLayout.VersionOffset),
                TimeInForce = data.GetU8(ExtraLayout.TimeInForceOffset),
                OrderId = data.GetBigInt(ExtraLayout.OrderIdOffset, Layout.KeyLength),
                Owner = data.GetPubKey(ExtraLayout.OwnerOffset),
                Quantity = data.GetS64(ExtraLayout.QuantityOffset),
                ClientOrderId = data.GetU64(ExtraLayout.ClientOrderIdOffset),
                BestInitial = data.GetS64(ExtraLayout.BestInitialOffset),
                Key = key.ToArray(),
                Timestamp = data.GetU64(ExtraLayout.TimestampOffset),
                Price = price
            };
        }
    }
}