using Solnet.Mango.Types;
using Solnet.Programs.Utilities;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Numerics;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a <see cref="EventType.Fill"/> in the Mango <see cref="EventQueue"/>.
    /// </summary>
    public class FillEvent : Event
    {
        /// <summary>
        /// Represents the layout of the <see cref="FillEvent"/>.
        /// </summary>
        internal static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the taker side value begins.
            /// </summary>
            internal const int TakerSideOffset = 1;

            /// <summary>
            /// The offset at which the maker slot value begins.
            /// </summary>
            internal const int MakerSlotOffset = 2;

            /// <summary>
            /// The offset at which the maker out value begins.
            /// </summary>
            internal const int MakerOutOffset = 3;

            /// <summary>
            /// The offset at which the maker public key value begins.
            /// </summary>
            internal const int MakerOffset = 24;

            /// <summary>
            /// The offset at which the maker order id value begins.
            /// </summary>
            internal const int MakerOrderIdOffset = 56;

            /// <summary>
            /// The offset at which the maker client order id value begins.
            /// </summary>
            internal const int MakerClientOrderIdOffset = 72;
            
            /// <summary>
            /// The offset at which the maker fee value begins.
            /// </summary>
            internal const int MakerFeeOffset = 80;

            /// <summary>
            /// The offset at which the best initial value begins.
            /// </summary>
            internal const int BestInitialOffset = 96;

            /// <summary>
            /// The offset at which the maker timestamp value begins.
            /// </summary>
            internal const int MakerTimestampOffset = 104;
            
            /// <summary>
            /// The offset at which the taker public key value begins.
            /// </summary>
            internal const int TakerOffset = 112;

            /// <summary>
            /// The offset at which the taker order id value begins.
            /// </summary>
            internal const int TakerOrderIdOffset = 144;

            /// <summary>
            /// The offset at which the taker client order id value begins.
            /// </summary>
            internal const int TakerClientOrderIdOffset = 160;

            /// <summary>
            /// The offset at which the taker fee value begins.
            /// </summary>
            internal const int TakerFeeOffset = 168;

            /// <summary>
            /// The offset at which the price value begins.
            /// </summary>
            internal const int PriceOffset = 184;

            /// <summary>
            /// The offset at which the quantity value begins.
            /// </summary>
            internal const int QuantityOffset = 192;
        }

        /// <summary>
        /// The side from the taker's point of view.
        /// </summary>
        public Side TakerSide;

        /// <summary>
        /// The slot of the order in the maker's orders.
        /// </summary>
        public byte MakerSlot;

        /// <summary>
        /// Whether the maker's order quantity has nothing left to fill.
        /// </summary>
        public bool MakerOut;

        /// <summary>
        /// The maker's public key.
        /// </summary>
        public PublicKey Maker;

        /// <summary>
        /// The maker's order id.
        /// </summary>
        public BigInteger MakerOrderId;

        /// <summary>
        /// The maker's client order id.
        /// </summary>
        public ulong MakerClientOrderId;

        /// <summary>
        /// The maker fee.
        /// </summary>
        public I80F48 MakerFee;

        /// <summary>
        /// The best bid/ask at the time the maker order was placed.
        /// </summary>
        public long BestInitial;

        /// <summary>
        /// Timestamp of when the maker order was placed.
        /// </summary>
        public ulong MakerTimestamp;
        
        /// <summary>
        /// The taker's public key.
        /// </summary>
        public PublicKey Taker;

        /// <summary>
        /// The taker's order id.
        /// </summary>
        public BigInteger TakerOrderId;

        /// <summary>
        /// The taker's client order id.
        /// </summary>
        public ulong TakerClientOrderId;

        /// <summary>
        /// The taker fee.
        /// </summary>
        public I80F48 TakerFee;

        /// <summary>
        /// The price of the fill.
        /// </summary>
        public long Price;

        /// <summary>
        /// The quantity that was filled.
        /// </summary>
        public long Quantity;
        
        /// <summary>
        /// Deserialize a span of bytes into a <see cref="FillEvent"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="FillEvent"/> structure.</returns>
        public static FillEvent Deserialize(ReadOnlySpan<byte> data)
        {
            return new FillEvent
            {
                EventType = (EventType) Enum.Parse(typeof(EventType), data.GetU8(Layout.EventTypeOffset).ToString()),
                TakerSide = (Side) Enum.Parse(typeof(Side), data.GetU8(ExtraLayout.TakerSideOffset).ToString()),
                MakerSlot = data.GetU8(ExtraLayout.MakerSlotOffset),
                MakerOut = data.GetU8(ExtraLayout.MakerOutOffset) == 1,
                Timestamp = data.GetU64(Layout.TimestampOffset),
                SequenceNumber = data.GetU64(Layout.SequenceNumberOffset),
                Maker = data.GetPubKey(ExtraLayout.MakerOffset),
                MakerOrderId = data.GetBigInt(ExtraLayout.MakerOrderIdOffset, I80F48.Length, true),
                MakerClientOrderId = data.GetU64(ExtraLayout.MakerClientOrderIdOffset),
                MakerFee = I80F48.Deserialize(data.Slice(ExtraLayout.MakerFeeOffset, I80F48.Length)),
                BestInitial = data.GetS64(ExtraLayout.BestInitialOffset),
                MakerTimestamp = data.GetU64(ExtraLayout.MakerTimestampOffset),
                Taker = data.GetPubKey(ExtraLayout.TakerOffset),
                TakerOrderId = data.GetBigInt(ExtraLayout.TakerOrderIdOffset, I80F48.Length, true),
                TakerClientOrderId = data.GetU64(ExtraLayout.TakerClientOrderIdOffset),
                TakerFee = I80F48.Deserialize(data.Slice(ExtraLayout.TakerFeeOffset, I80F48.Length)),
                Price = data.GetS64(ExtraLayout.PriceOffset),
                Quantity = data.GetS64(ExtraLayout.QuantityOffset)
            };
        }
    }
}