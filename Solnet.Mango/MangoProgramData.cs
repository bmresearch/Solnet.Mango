using Solnet.Mango.Models;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Solnet.Mango
{
    /// <summary>
    /// Implements the Mango Program instruction data encoding and decoding.
    /// </summary>
    internal static class MangoProgramData
    {
        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.InitMangoAccount"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeInitMangoAccountData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.InitMangoAccount, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.InitMangoAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitMangoAccountData(DecodedInstruction decodedInstruction, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Sysvar Rent", keys[keyIndices[3]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.Deposit"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeDepositData(ulong quantity)
        {
            byte[] data = new byte[12];
            data.WriteU32((uint)MangoProgramInstructions.Values.Deposit, MangoProgramLayouts.MethodOffset);
            data.WriteU64(quantity, MangoProgramLayouts.DepositQuantityOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.Deposit"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeDepositData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Root Bank", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Node Bank", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Vault", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Token Program", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Owner Token Account", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Quantity", data.GetU64(MangoProgramLayouts.DepositQuantityOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.Withdraw"/> method.
        /// </summary>
        /// <param name="quantity">The quantity to withdraw.</param>
        /// <param name="allowBorrow">Whether this withdraw should allow borrowing or not.</param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeWithdrawData(ulong quantity, bool allowBorrow)
        {
            byte[] data = new byte[13];
            data.WriteU32((uint)MangoProgramInstructions.Values.Withdraw, MangoProgramLayouts.MethodOffset);
            data.WriteU64(quantity, MangoProgramLayouts.WithdrawQuantityOffset);
            data.WriteU8(allowBorrow ? (byte)1 : (byte)0, MangoProgramLayouts.WithdrawAllowBorrowOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.Withdraw"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeWithdrawData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Root Bank", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Node Bank", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Vault", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Owner Token Account", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Token Program", keys[keyIndices[9]]);
            for (int i = 10; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Open Orders Account {i - 9}", keys[keyIndices[i]]);
            }
            decodedInstruction.Values.Add("Quantity", data.GetU64(MangoProgramLayouts.WithdrawQuantityOffset));
            decodedInstruction.Values.Add("Allow Borrow", data.GetBool(MangoProgramLayouts.WithdrawAllowBorrowOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        public static byte[] EncodePlaceSpotOrderData(Order order)
            => EncodePlaceSpotOrderData(order.Side, order.RawPrice, order.RawQuantity, order.Type, order.ClientOrderId,
                order.SelfTradeBehavior, order.MaxQuoteQuantity, ushort.MaxValue);

        /// <summary>
        /// Encode the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder"/> method.
        /// </summary>
        /// <param name="side">The side of the order.</param>
        /// <param name="limitPrice">The price at which the order is to be placed.</param>
        /// <param name="maxCoinQty">The maximum amount of coins to receive.</param>
        /// <param name="orderType">The type of the order.</param>
        /// <param name="clientOrderId">The client's order id.</param>
        /// <param name="selfTradeBehaviorType">The behavior when trading against oneself.</param>
        /// <param name="maxNativePcQtyIncludingFees">The maximum amount of price coins to pay.</param>
        /// <param name="limit">The maximum number of iterations of the Serum order matching loop.</param>
        /// <returns>The encoded data.</returns>
        public static byte[] EncodePlaceSpotOrderData(Side side, ulong limitPrice, ulong maxCoinQty,
            OrderType orderType, ulong clientOrderId, SelfTradeBehavior selfTradeBehaviorType,
            ulong maxNativePcQtyIncludingFees, ushort limit)
        {
            byte[] data = new byte[50];
            data.WriteU32((uint)MangoProgramInstructions.Values.PlaceSpotOrder, MangoProgramLayouts.MethodOffset);
            data.WriteU32((uint)side, MangoProgramLayouts.PlaceSpotOrder.SideOffset);
            data.WriteU64(limitPrice, MangoProgramLayouts.PlaceSpotOrder.PriceOffset);
            data.WriteU64(maxCoinQty, MangoProgramLayouts.PlaceSpotOrder.MaxBaseQuantityOffset);
            data.WriteU64(maxNativePcQtyIncludingFees, MangoProgramLayouts.PlaceSpotOrder.MaxQuoteQuantity);
            data.WriteU32((uint)selfTradeBehaviorType, MangoProgramLayouts.PlaceSpotOrder.SelfTradeBehaviorOffset);
            data.WriteU32((uint)orderType, MangoProgramLayouts.PlaceSpotOrder.OrderTypeOffset);
            data.WriteU64(clientOrderId, MangoProgramLayouts.PlaceSpotOrder.ClientIdOffset);
            data.WriteU16(limit, MangoProgramLayouts.PlaceSpotOrder.LimitOffset);
            return data;
        }
        
        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        public static byte[] EncodePlaceSpotOrder2Data(Order order)
            => EncodePlaceSpotOrder2Data(order.Side, order.RawPrice, order.RawQuantity, order.Type, order.ClientOrderId,
                order.SelfTradeBehavior, order.MaxQuoteQuantity, ushort.MaxValue);

        /// <summary>
        /// Encode the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder"/> method.
        /// </summary>
        /// <param name="side">The side of the order.</param>
        /// <param name="limitPrice">The price at which the order is to be placed.</param>
        /// <param name="maxCoinQty">The maximum amount of coins to receive.</param>
        /// <param name="orderType">The type of the order.</param>
        /// <param name="clientOrderId">The client's order id.</param>
        /// <param name="selfTradeBehaviorType">The behavior when trading against oneself.</param>
        /// <param name="maxNativePcQtyIncludingFees">The maximum amount of price coins to pay.</param>
        /// <param name="limit">The maximum number of iterations of the Serum order matching loop.</param>
        /// <returns>The encoded data.</returns>
        public static byte[] EncodePlaceSpotOrder2Data(Side side, ulong limitPrice, ulong maxCoinQty,
            OrderType orderType, ulong clientOrderId, SelfTradeBehavior selfTradeBehaviorType,
            ulong maxNativePcQtyIncludingFees, ushort limit)
        {
            byte[] data = new byte[50];
            data.WriteU32((uint)MangoProgramInstructions.Values.PlaceSpotOrder2, MangoProgramLayouts.MethodOffset);
            data.WriteU32((uint)side, MangoProgramLayouts.PlaceSpotOrder.SideOffset);
            data.WriteU64(limitPrice, MangoProgramLayouts.PlaceSpotOrder.PriceOffset);
            data.WriteU64(maxCoinQty, MangoProgramLayouts.PlaceSpotOrder.MaxBaseQuantityOffset);
            data.WriteU64(maxNativePcQtyIncludingFees, MangoProgramLayouts.PlaceSpotOrder.MaxQuoteQuantity);
            data.WriteU32((uint)selfTradeBehaviorType, MangoProgramLayouts.PlaceSpotOrder.SelfTradeBehaviorOffset);
            data.WriteU32((uint)orderType, MangoProgramLayouts.PlaceSpotOrder.OrderTypeOffset);
            data.WriteU64(clientOrderId, MangoProgramLayouts.PlaceSpotOrder.ClientIdOffset);
            data.WriteU16(limit, MangoProgramLayouts.PlaceSpotOrder.LimitOffset);
            return data;
        }
        
        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodePlaceSpotOrderData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Dex Program", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Spot Market", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Dex Request Queue", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Dex Event Queue", keys[keyIndices[9]]);
            decodedInstruction.Values.Add("Dex Base Vault", keys[keyIndices[10]]);
            decodedInstruction.Values.Add("Dex Quote Vault", keys[keyIndices[11]]);
            decodedInstruction.Values.Add("Base Root Bank", keys[keyIndices[12]]);
            decodedInstruction.Values.Add("Base Node Bank", keys[keyIndices[13]]);
            decodedInstruction.Values.Add("Base Vault", keys[keyIndices[14]]);
            decodedInstruction.Values.Add("Quote Root Bank", keys[keyIndices[15]]);
            decodedInstruction.Values.Add("Quote Node Bank", keys[keyIndices[16]]);
            decodedInstruction.Values.Add("Quote Vault", keys[keyIndices[17]]);
            decodedInstruction.Values.Add("Token Program", keys[keyIndices[18]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[19]]);
            decodedInstruction.Values.Add("Sysvar Rent", keys[keyIndices[20]]);
            decodedInstruction.Values.Add("Dex Vault Signer", keys[keyIndices[21]]);
            decodedInstruction.Values.Add("Serum Vault", keys[keyIndices[22]]);
            for (int i = 23; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Open Orders Account {i - 22}", keys[keyIndices[i]]);
            }
            decodedInstruction.Values.Add("Side",
                (Side)Enum.Parse(typeof(Side), data.GetU8(MangoProgramLayouts.PlaceSpotOrder.SideOffset).ToString()));
            decodedInstruction.Values.Add("Price", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.PriceOffset));
            decodedInstruction.Values.Add("Max Coin Quantity", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.MaxBaseQuantityOffset));
            decodedInstruction.Values.Add("Max Price Coin Quantity", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.MaxQuoteQuantity));
            decodedInstruction.Values.Add("Self Trade Behavior",
                (SelfTradeBehavior)Enum.Parse(typeof(SelfTradeBehavior), data.GetU8(MangoProgramLayouts.PlaceSpotOrder.SelfTradeBehaviorOffset).ToString()));
            decodedInstruction.Values.Add("Order Type",
                (OrderType)Enum.Parse(typeof(OrderType), data.GetU8(MangoProgramLayouts.PlaceSpotOrder.OrderTypeOffset).ToString()));
            decodedInstruction.Values.Add("Client Order Id", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.ClientIdOffset));
            decodedInstruction.Values.Add("Limit", data.GetU16(MangoProgramLayouts.PlaceSpotOrder.LimitOffset));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder2"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodePlaceSpotOrder2Data(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Dex Program", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Spot Market", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Dex Request Queue", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Dex Event Queue", keys[keyIndices[9]]);
            decodedInstruction.Values.Add("Dex Base Vault", keys[keyIndices[10]]);
            decodedInstruction.Values.Add("Dex Quote Vault", keys[keyIndices[11]]);
            decodedInstruction.Values.Add("Base Root Bank", keys[keyIndices[12]]);
            decodedInstruction.Values.Add("Base Node Bank", keys[keyIndices[13]]);
            decodedInstruction.Values.Add("Base Vault", keys[keyIndices[14]]);
            decodedInstruction.Values.Add("Quote Root Bank", keys[keyIndices[15]]);
            decodedInstruction.Values.Add("Quote Node Bank", keys[keyIndices[16]]);
            decodedInstruction.Values.Add("Quote Vault", keys[keyIndices[17]]);
            decodedInstruction.Values.Add("Token Program", keys[keyIndices[18]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[19]]);
            decodedInstruction.Values.Add("Dex Vault Signer", keys[keyIndices[20]]);
            decodedInstruction.Values.Add("Serum Vault", keys[keyIndices[21]]);
            for (int i = 22; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Open Orders Account {i - 21}", keys[keyIndices[i]]);
            }
            decodedInstruction.Values.Add("Side",
                (Side)Enum.Parse(typeof(Side), data.GetU8(MangoProgramLayouts.PlaceSpotOrder.SideOffset).ToString()));
            decodedInstruction.Values.Add("Price", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.PriceOffset));
            decodedInstruction.Values.Add("Max Coin Quantity", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.MaxBaseQuantityOffset));
            decodedInstruction.Values.Add("Max Price Coin Quantity", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.MaxQuoteQuantity));
            decodedInstruction.Values.Add("Self Trade Behavior",
                (SelfTradeBehavior)Enum.Parse(typeof(SelfTradeBehavior), data.GetU8(MangoProgramLayouts.PlaceSpotOrder.SelfTradeBehaviorOffset).ToString()));
            decodedInstruction.Values.Add("Order Type",
                (OrderType)Enum.Parse(typeof(OrderType), data.GetU8(MangoProgramLayouts.PlaceSpotOrder.OrderTypeOffset).ToString()));
            decodedInstruction.Values.Add("Client Order Id", data.GetU64(MangoProgramLayouts.PlaceSpotOrder.ClientIdOffset));
            decodedInstruction.Values.Add("Limit", data.GetU16(MangoProgramLayouts.PlaceSpotOrder.LimitOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.PlacePerpOrder"/> method.
        /// </summary>
        /// <param name="side">The side order type.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="reduceOnly">Whether the order is to reduce only or not.</param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodePlacePerpOrderData(Side side, PerpOrderType orderType, long price, long quantity, ulong clientOrderId, bool reduceOnly = false)
        {
            byte[] data = new byte[31];
            data.WriteU32((uint)MangoProgramInstructions.Values.PlacePerpOrder, MangoProgramLayouts.MethodOffset);
            data.WriteS64(price, MangoProgramLayouts.PlacePerpOrder.PriceOffset);
            data.WriteS64(quantity, MangoProgramLayouts.PlacePerpOrder.QuantityOffset);
            data.WriteU64(clientOrderId, MangoProgramLayouts.PlacePerpOrder.ClientOrderIdOffset);
            data.WriteU8((byte)side, MangoProgramLayouts.PlacePerpOrder.SideOffset);
            data.WriteU8((byte)orderType, MangoProgramLayouts.PlacePerpOrder.OrderTypeOffset);
            data.WriteU8(reduceOnly ? (byte)1 : (byte)0, MangoProgramLayouts.PlacePerpOrder.ReduceOnlyOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.PlacePerpOrder"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodePlacePerpOrderData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Event Queue", keys[keyIndices[7]]);
            for (int i = 8; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Open Orders Account {i - 7}", keys[keyIndices[i]]);
            }
            decodedInstruction.Values.Add("Price", data.GetS64(MangoProgramLayouts.PlacePerpOrder.PriceOffset));
            decodedInstruction.Values.Add("Quantity", data.GetS64(MangoProgramLayouts.PlacePerpOrder.QuantityOffset));
            decodedInstruction.Values.Add("Client Order Id", data.GetU64(MangoProgramLayouts.PlacePerpOrder.ClientOrderIdOffset));
            decodedInstruction.Values.Add("Side",
                (Side)Enum.Parse(typeof(Side), data.GetU8(MangoProgramLayouts.PlacePerpOrder.SideOffset).ToString()));
            decodedInstruction.Values.Add("Order Type",
                (OrderType)Enum.Parse(typeof(OrderType), data.GetU8(MangoProgramLayouts.PlacePerpOrder.OrderTypeOffset).ToString()));
            decodedInstruction.Values.Add("Reduce Only", data.GetBool(MangoProgramLayouts.PlacePerpOrder.ReduceOnlyOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CancelPerpOrderByClientId"/> method.
        /// </summary>
        /// <param name="clientOrderId">The client's order id.</param>
        /// <param name="invalidIdOk"></param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCancelPerpOrderByClientIdData(ulong clientOrderId, bool invalidIdOk)
        {
            byte[] data = new byte[13];
            data.WriteU32((uint)MangoProgramInstructions.Values.CancelPerpOrderByClientId, MangoProgramLayouts.MethodOffset);
            data.WriteU64(clientOrderId, MangoProgramLayouts.CancelPerpOrderByClientId.ClientOrderIdOffset);
            data.WriteU8(invalidIdOk ? (byte)1 : (byte)0, MangoProgramLayouts.CancelPerpOrderByClientId.InvalidIdOkOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CancelPerpOrderByClientId"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCancelPerpOrderByClientIdData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Client Order Id", data.GetU64(MangoProgramLayouts.CancelPerpOrderByClientId.ClientOrderIdOffset));
            decodedInstruction.Values.Add("Invalid Ok", data.GetU8(MangoProgramLayouts.CancelPerpOrderByClientId.InvalidIdOkOffset) == 1);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CancelPerpOrder"/> method.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="invalidIdOk"></param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCancelPerpOrderData(BigInteger orderId, bool invalidIdOk)
        {
            byte[] data = new byte[21];
            data.WriteU32((uint)MangoProgramInstructions.Values.CancelPerpOrder, MangoProgramLayouts.MethodOffset);
            data.WriteBigInt(orderId, MangoProgramLayouts.CancelPerpOrder.OrderIdOffset);
            data.WriteU8(invalidIdOk ? (byte)1 : (byte)0, MangoProgramLayouts.CancelPerpOrder.InvalidIdOkOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CancelPerpOrder"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCancelPerpOrderData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Order Id", data.GetBigInt(MangoProgramLayouts.CancelPerpOrder.OrderIdOffset, 16));
            decodedInstruction.Values.Add("Invalid Ok", data.GetU8(MangoProgramLayouts.CancelPerpOrder.InvalidIdOkOffset) == 1);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.SettleFunds"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeSettleFundsData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.SettleFunds, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.SettleFunds"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSettleFundsData(DecodedInstruction decodedInstruction, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Dex Program", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Spot Market", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Open Orders Account", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Dex Base Vault", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Dex Quote Vault", keys[keyIndices[9]]);
            decodedInstruction.Values.Add("Base Root Bank", keys[keyIndices[10]]);
            decodedInstruction.Values.Add("Base Node Bank", keys[keyIndices[11]]);
            decodedInstruction.Values.Add("Quote Root Bank", keys[keyIndices[12]]);
            decodedInstruction.Values.Add("Quote Node Bank", keys[keyIndices[13]]);
            decodedInstruction.Values.Add("Base Vault", keys[keyIndices[14]]);
            decodedInstruction.Values.Add("Quote Vault", keys[keyIndices[15]]);
            decodedInstruction.Values.Add("Dex Signer", keys[keyIndices[16]]);
            decodedInstruction.Values.Add("Token Program", keys[keyIndices[17]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.SettleFees"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeSettleFeesData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.SettleFees, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.SettleFees"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSettleFeesData(DecodedInstruction decodedInstruction, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Root Bank", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Node Bank", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Bank Vault", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Fees Vault", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Signer", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Token Program", keys[keyIndices[9]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CancelSpotOrder"/> method.
        /// </summary>
        /// <param name="side">The order's side.</param>
        /// <param name="orderId">The client's order id.</param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCancelSpotOrderData(Side side, BigInteger orderId)
        {
            byte[] data = new byte[24];
            data.WriteU32((uint)MangoProgramInstructions.Values.CancelSpotOrder, MangoProgramLayouts.MethodOffset);
            data.WriteU32((uint)side, MangoProgramLayouts.CancelSpotOrder.SideOffset);
            data.WriteBigInt(orderId, MangoProgramLayouts.CancelSpotOrder.OrderIdOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CancelSpotOrder"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCancelSpotOrderData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Dex Program", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Spot Market", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Open Orders Account", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Dex Event Queue", keys[keyIndices[9]]);
            decodedInstruction.Values.Add("Side",
                (Side)Enum.Parse(typeof(Side), data.GetU32(MangoProgramLayouts.CancelSpotOrder.SideOffset).ToString()));
            decodedInstruction.Values.Add("Order Id", data.GetBigInt(MangoProgramLayouts.CancelSpotOrder.OrderIdOffset, 16));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.SettleProfitAndLoss"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeSettleProfitAndLossData(ulong quantity)
        {
            byte[] data = new byte[12];
            data.WriteU32((uint)MangoProgramInstructions.Values.SettleProfitAndLoss, MangoProgramLayouts.MethodOffset);
            data.WriteU64(quantity, MangoProgramLayouts.DepositQuantityOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.SettleProfitAndLoss"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSettleProfitAndLossData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account A", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Mango Account B", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Root Bank", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Node Bank", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Market Index", data.GetU64(MangoProgramLayouts.DepositQuantityOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.InitSpotOpenOrders"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeInitSpotOpenOrdersData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.InitSpotOpenOrders, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.InitSpotOpenOrders"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitSpotOpenOrdersData(DecodedInstruction decodedInstruction, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Dex Program", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Open Orders Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Spot Market", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Sysvar Rent", keys[keyIndices[7]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.RedeemMango"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeRedeemMangoData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.RedeemMango, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.RedeemMango"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeRedeemMangoData(DecodedInstruction decodedInstruction, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Mango Perp Vault", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Mango Root Bank", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Mango Node Bank", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Mango Bank Vault", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[9]]);
            decodedInstruction.Values.Add("Token Program", keys[keyIndices[10]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.AddMangoAccountInfo"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeAddMangoAccountInfoData(string info)
        {
            byte[] data = new byte[36];
            data.WriteU32((uint)MangoProgramInstructions.Values.AddMangoAccountInfo, MangoProgramLayouts.MethodOffset);
            byte[] encodedInfo = Serialization.EncodeRustString(info);
            data.WriteSpan(encodedInfo, MangoProgramLayouts.MangoAccountInfoOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.AddMangoAccountInfo"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAddMangoAccountInfoData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            (string accountInfo, _) = data.DecodeRustString(MangoProgramLayouts.MangoAccountInfoOffset);
            decodedInstruction.Values.Add("Account Info", accountInfo);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CancelAllPerpOrders"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCancelAllPerpOrdersData(byte limit)
        {
            byte[] data = new byte[5];
            data.WriteU32((uint)MangoProgramInstructions.Values.CancelAllPerpOrders, MangoProgramLayouts.MethodOffset);
            data.WriteU8(limit, MangoProgramLayouts.DepositQuantityOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CancelAllPerpOrders"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCancelAllPerpOrdersData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Limit", data.GetU8(MangoProgramLayouts.DepositQuantityOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.InitAdvancedOrders"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeInitAdvancedOrdersData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.InitAdvancedOrders, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.InitAdvancedOrders"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitAdvancedOrdersData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Advanced Orders", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("System Program", keys[keyIndices[4]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.AddPerpTriggerOrder"/> method.
        /// </summary>
        /// <param name="orderType">The order type.</param>
        /// <param name="side">The order side.</param>
        /// <param name="triggerCondition">The trigger condition.</param>
        /// <param name="reduceOnly">Whether the trigger order is reduce only or not.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="triggerPrice">The trigger price</param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeAddPerpTriggerOrderData(PerpOrderType orderType, Side side, TriggerCondition triggerCondition,
            bool reduceOnly, ulong clientOrderId, long price, long quantity, I80F48 triggerPrice)
        {
            byte[] data = new byte[48];
            data.WriteU32((uint)MangoProgramInstructions.Values.AddPerpTriggerOrder, MangoProgramLayouts.MethodOffset);
            data.WriteU8((byte)orderType, MangoProgramLayouts.AddPerpTriggerOrder.OrderTypeOffset);
            data.WriteU8((byte)side, MangoProgramLayouts.AddPerpTriggerOrder.SideOffset);
            data.WriteU8((byte)triggerCondition, MangoProgramLayouts.AddPerpTriggerOrder.TriggerConditionOffset);
            data.WriteU8(reduceOnly ? (byte) 1 : (byte) 0, MangoProgramLayouts.AddPerpTriggerOrder.ReduceOnlyOffset);
            data.WriteU64(clientOrderId, MangoProgramLayouts.AddPerpTriggerOrder.ClientOrderIdOffset);
            data.WriteS64(price, MangoProgramLayouts.AddPerpTriggerOrder.PriceOffset);
            data.WriteS64(quantity, MangoProgramLayouts.AddPerpTriggerOrder.QuantityOffset);
            I80F48.Serialize(data, triggerPrice, MangoProgramLayouts.AddPerpTriggerOrder.TriggerPriceOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.AddPerpTriggerOrder"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAddPerpTriggerOrderData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Advanced Orders", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Mango Cache", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("System Program", keys[keyIndices[6]]);
            for (int i = 7; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Open Orders Account {i - 6}", keys[keyIndices[i]]);
            }
            var triggerPrice = I80F48.Deserialize(data.Slice(MangoProgramLayouts.AddPerpTriggerOrder.TriggerPriceOffset, I80F48.Length));
            decodedInstruction.Values.Add("Trigger Price", triggerPrice);
            decodedInstruction.Values.Add("Price", data.GetS64(MangoProgramLayouts.AddPerpTriggerOrder.PriceOffset));
            decodedInstruction.Values.Add("Quantity", data.GetS64(MangoProgramLayouts.AddPerpTriggerOrder.QuantityOffset));
            decodedInstruction.Values.Add("Client Order Id", data.GetU64(MangoProgramLayouts.AddPerpTriggerOrder.ClientOrderIdOffset));
            decodedInstruction.Values.Add("Trigger Condition",
                (TriggerCondition)Enum.Parse(typeof(TriggerCondition), data.GetU8(MangoProgramLayouts.AddPerpTriggerOrder.TriggerConditionOffset).ToString()));
            decodedInstruction.Values.Add("Side",
                (Side)Enum.Parse(typeof(Side), data.GetU8(MangoProgramLayouts.AddPerpTriggerOrder.SideOffset).ToString()));
            decodedInstruction.Values.Add("Order Type",
                (OrderType)Enum.Parse(typeof(OrderType), data.GetU8(MangoProgramLayouts.AddPerpTriggerOrder.OrderTypeOffset).ToString()));
            decodedInstruction.Values.Add("Reduce Only", data.GetBool(MangoProgramLayouts.AddPerpTriggerOrder.ReduceOnlyOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.RemoveAdvancedOrder"/> method.
        /// </summary>
        /// <param name="orderIndex">The order index.</param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeRemoveAdvancedOrderData(byte orderIndex)
        {
            byte[] data = new byte[5];
            data.WriteU32((uint)MangoProgramInstructions.Values.RemoveAdvancedOrder, MangoProgramLayouts.MethodOffset);
            data.WriteU8(orderIndex, MangoProgramLayouts.OrderIndexOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.RemoveAdvancedOrder"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeRemoveAdvancedOrderData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Advanced Orders", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("System Program", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Order Index", data.GetU8(MangoProgramLayouts.OrderIndexOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CloseAdvancedOrders"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCloseAdvancedOrdersData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.CloseAdvancedOrders, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CloseAdvancedOrders"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCloseAdvancedOrdersData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Advanced Orders", keys[keyIndices[3]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CloseSpotOpenOrders"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCloseSpotOpenOrdersData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.CloseSpotOpenOrders, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CloseSpotOpenOrders"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCloseSpotOpenOrdersData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Dex Program", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Open Orders Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Spot Market", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Mango Signer", keys[keyIndices[6]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CloseMangoAccount"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCloseMangoAccountData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.CloseMangoAccount, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CloseMangoAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCloseMangoAccountData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CreateMangoAccount"/> method.
        /// </summary>
        /// <param name="accountNum">The account number.</param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCreateMangoAccountData(ulong accountNum)
        {
            byte[] data = new byte[12];
            data.WriteU32((uint)MangoProgramInstructions.Values.CreateMangoAccount, MangoProgramLayouts.MethodOffset);
            data.WriteU64(accountNum, MangoProgramLayouts.AccountNumOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CreateMangoAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCreateMangoAccountData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("System Program", keys[keyIndices[3]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.UpgradeMangoAccountV0V1"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeUpgradeMangoAccountV0V1Data()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.UpgradeMangoAccountV0V1, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.UpgradeMangoAccountV0V1"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeUpgradeMangoAccountV0V1Data(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.CancelPerpOrdersSide"/> method.
        /// </summary>
        /// <param name="side">The side of the orders to cancel.</param>
        /// <param name="limit">The maximum number of orders to cancel.</param>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeCancelPerpOrdersSideData(Side side, byte limit)
        {
            byte[] data = new byte[6];
            data.WriteU32((uint)MangoProgramInstructions.Values.CancelPerpOrdersSide, MangoProgramLayouts.MethodOffset);
            data.WriteU8((byte)side, MangoProgramLayouts.CancelPerpOrdersSide.SideOffset);
            data.WriteU8((byte)limit, MangoProgramLayouts.CancelPerpOrdersSide.LimitOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.CancelAllPerpOrders"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCancelPerpOrdersSideData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Perp Market", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Bids", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Asks", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Side",
                (Side)Enum.Parse(typeof(Side), data.GetU32(MangoProgramLayouts.CancelPerpOrdersSide.SideOffset).ToString()));
            decodedInstruction.Values.Add("Limit", data.GetU8(MangoProgramLayouts.CancelPerpOrdersSide.LimitOffset));
        }

        /// <summary>
        /// Encodes the <see cref="TransactionInstruction"/> data for the <see cref="MangoProgramInstructions.Values.SetDelegate"/> method.
        /// </summary>
        /// <returns>The encoded data.</returns>
        internal static byte[] EncodeSetDelegateData()
        {
            byte[] data = new byte[4];
            data.WriteU32((uint)MangoProgramInstructions.Values.SetDelegate, MangoProgramLayouts.MethodOffset);
            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="MangoProgramInstructions.Values.SetDelegate"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSetDelegateData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mango Group", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mango Account", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Delegate", keys[keyIndices[3]]);
        }
    }
}