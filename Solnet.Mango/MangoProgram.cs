using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Serum;
using Solnet.Serum.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Solnet.Mango
{
    /// <summary>
    /// Implements the Mango Program methods.
    /// <remarks>
    /// For more information see:
    /// https://github.com/blockworks-foundation/mango/
    /// </remarks>
    /// </summary>
    public class MangoProgram
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly PublicKey SysVarClock = new("SysvarC1ock11111111111111111111111111111111");

        /// <summary>
        /// The public key of the MNGO token mint.
        /// </summary>
        public static readonly PublicKey MangoToken = new("MangoCzJ36AjZyKwVj3VnYU4GTonjfVEnJmvvWaxLac");

        /// <summary>
        /// The public key of the Mango V3 program.
        /// </summary>
        public static readonly PublicKey ProgramIdKeyV3 = new("mv3ekLzLbnVPNxjSKvqBpU3ZeZXPQdEC3bp5MDEBG68");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Mango Program V3";

        /// <summary>
        /// Initializes an instruction to initialize a given account as a <see cref="MangoAccount"/>.
        /// </summary>
        /// <param name="mangoGroup">The public key of the <see cref="MangoGroup"/> account.</param>
        /// <param name="mangoAccount">The public key of the account to initialize as <see cref="MangoAccount"/>.</param>
        /// <param name="owner">The public key of the owner of the <see cref="MangoAccount"/>.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeMangoAccount(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner) => InitializeMangoAccount(ProgramIdKeyV3, mangoGroup, mangoAccount, owner);

        /// <summary>
        /// Initializes an instruction to initialize a given account as a <see cref="MangoAccount"/>.
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="mangoGroup">The public key of the <see cref="MangoGroup"/> account.</param>
        /// <param name="mangoAccount">The public key of the account to initialize as <see cref="MangoAccount"/>.</param>
        /// <param name="owner">The public key of the owner of the <see cref="MangoAccount"/>.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeMangoAccount(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(SysVars.RentKey, false)
            };
            return new TransactionInstruction
            {
                Data = MangoProgramData.EncodeInitMangoAccountData(),
                Keys = keys,
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="vault"></param>
        /// <param name="ownerTokenAccount"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static TransactionInstruction Deposit(PublicKey mangoGroup, PublicKey mangoAccount, PublicKey owner,
            PublicKey mangoCache, PublicKey rootBank, PublicKey nodeBank, PublicKey vault, PublicKey ownerTokenAccount,
            ulong quantity)
            => Deposit(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, mangoCache, rootBank, nodeBank, vault,
                ownerTokenAccount, quantity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="vault"></param>
        /// <param name="ownerTokenAccount"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static TransactionInstruction Deposit(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey rootBank,
            PublicKey nodeBank, PublicKey vault, PublicKey ownerTokenAccount, ulong quantity)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(rootBank, false),
                AccountMeta.Writable(nodeBank, false),
                AccountMeta.Writable(vault, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.Writable(ownerTokenAccount, false)
            };
            return new TransactionInstruction
            {
                Data = MangoProgramData.EncodeDepositData(quantity),
                Keys = keys,
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="vault"></param>
        /// <param name="ownerTokenAccount"></param>
        /// <param name="signer"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="quantity"></param>
        /// <param name="allowBorrow"></param>
        /// <returns></returns>
        public static TransactionInstruction Withdraw(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey rootBank,
            PublicKey nodeBank, PublicKey vault, PublicKey ownerTokenAccount, PublicKey signer,
            IList<PublicKey> openOrdersAccounts, ulong quantity, bool allowBorrow)
            => Withdraw(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, mangoCache, rootBank, nodeBank, vault,
                ownerTokenAccount, signer, openOrdersAccounts, quantity, allowBorrow);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="vault"></param>
        /// <param name="ownerTokenAccount"></param>
        /// <param name="signer"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="quantity"></param>
        /// <param name="allowBorrow"></param>
        /// <returns></returns>
        public static TransactionInstruction Withdraw(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey rootBank,
            PublicKey nodeBank, PublicKey vault, PublicKey ownerTokenAccount, PublicKey signer,
            IList<PublicKey> openOrdersAccounts, ulong quantity, bool allowBorrow)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(rootBank, false),
                AccountMeta.Writable(nodeBank, false),
                AccountMeta.Writable(vault, false),
                AccountMeta.Writable(ownerTokenAccount, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            if (openOrdersAccounts != null)
                keys.AddRange(openOrdersAccounts.Select(key => AccountMeta.ReadOnly(key, false)));

            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = programIdKey,
                Data = MangoProgramData.EncodeWithdrawData(quantity, allowBorrow)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="spotMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="dexRequestQueue"></param>
        /// <param name="dexEventQueue"></param>
        /// <param name="dexBase"></param>
        /// <param name="dexQuote"></param>
        /// <param name="baseRootBank"></param>
        /// <param name="baseNodeBank"></param>
        /// <param name="baseVault"></param>
        /// <param name="quoteRootBank"></param>
        /// <param name="quoteNodeBank"></param>
        /// <param name="quoteVault"></param>
        /// <param name="signer"></param>
        /// <param name="dexSigner"></param>
        /// <param name="serumVault"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="marketIndex"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static TransactionInstruction PlaceSpotOrder(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey mangoCache, PublicKey spotMarket, PublicKey bids, PublicKey asks,
            PublicKey dexRequestQueue, PublicKey dexEventQueue, PublicKey dexBase, PublicKey dexQuote,
            PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey baseVault, PublicKey quoteRootBank,
            PublicKey quoteNodeBank, PublicKey quoteVault, PublicKey signer, PublicKey dexSigner, PublicKey serumVault,
            IList<PublicKey> openOrdersAccounts, ulong marketIndex, Order order)
            => PlaceSpotOrder(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, mangoCache, SerumProgram.ProgramIdKey,
                spotMarket, bids, asks, dexRequestQueue, dexEventQueue, dexBase, dexQuote, baseRootBank, baseNodeBank,
                baseVault, quoteRootBank, quoteNodeBank, quoteVault, signer, dexSigner, serumVault, openOrdersAccounts,
                marketIndex, order);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="dexProgramIdKey"></param>
        /// <param name="spotMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="dexRequestQueue"></param>
        /// <param name="dexEventQueue"></param>
        /// <param name="dexBase"></param>
        /// <param name="dexQuote"></param>
        /// <param name="baseRootBank"></param>
        /// <param name="baseNodeBank"></param>
        /// <param name="baseVault"></param>
        /// <param name="quoteRootBank"></param>
        /// <param name="quoteNodeBank"></param>
        /// <param name="quoteVault"></param>
        /// <param name="signer"></param>
        /// <param name="dexSigner"></param>
        /// <param name="serumVault"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="marketIndex"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static TransactionInstruction PlaceSpotOrder(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey dexProgramIdKey, PublicKey spotMarket,
            PublicKey bids, PublicKey asks, PublicKey dexRequestQueue, PublicKey dexEventQueue, PublicKey dexBase,
            PublicKey dexQuote, PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey baseVault, PublicKey quoteRootBank,
            PublicKey quoteNodeBank, PublicKey quoteVault, PublicKey signer, PublicKey dexSigner, PublicKey serumVault,
            IList<PublicKey> openOrdersAccounts, ulong marketIndex, Order order)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(dexProgramIdKey, false),
                AccountMeta.Writable(spotMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false),
                AccountMeta.Writable(dexRequestQueue, false),
                AccountMeta.Writable(dexEventQueue, false),
                AccountMeta.Writable(dexBase, false),
                AccountMeta.Writable(dexQuote, false),
                AccountMeta.ReadOnly(baseRootBank, false),
                AccountMeta.Writable(baseNodeBank, false),
                AccountMeta.Writable(baseVault, false),
                AccountMeta.ReadOnly(quoteRootBank, false),
                AccountMeta.Writable(quoteNodeBank, false),
                AccountMeta.Writable(quoteVault, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(dexSigner, false),
                AccountMeta.ReadOnly(serumVault, false)
            };

            keys.AddRange(openOrdersAccounts.Select((t, i) => (ulong)i == marketIndex
                ? AccountMeta.Writable(t, false)
                : AccountMeta.ReadOnly(t, false)));

            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodePlaceSpotOrderData(order),
                ProgramId = programIdKey,
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="spotMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="dexRequestQueue"></param>
        /// <param name="dexEventQueue"></param>
        /// <param name="dexBase"></param>
        /// <param name="dexQuote"></param>
        /// <param name="baseRootBank"></param>
        /// <param name="baseNodeBank"></param>
        /// <param name="baseVault"></param>
        /// <param name="quoteRootBank"></param>
        /// <param name="quoteNodeBank"></param>
        /// <param name="quoteVault"></param>
        /// <param name="signer"></param>
        /// <param name="dexSigner"></param>
        /// <param name="serumVault"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="marketIndex"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static TransactionInstruction PlaceSpotOrder2(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey mangoCache, PublicKey spotMarket, PublicKey bids, PublicKey asks,
            PublicKey dexRequestQueue, PublicKey dexEventQueue, PublicKey dexBase, PublicKey dexQuote,
            PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey baseVault, PublicKey quoteRootBank,
            PublicKey quoteNodeBank, PublicKey quoteVault, PublicKey signer, PublicKey dexSigner, PublicKey serumVault,
            IList<PublicKey> openOrdersAccounts, int marketIndex, Order order)
            => PlaceSpotOrder2(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, mangoCache,
                SerumProgram.ProgramIdKey,
                spotMarket, bids, asks, dexRequestQueue, dexEventQueue, dexBase, dexQuote, baseRootBank, baseNodeBank,
                baseVault, quoteRootBank, quoteNodeBank, quoteVault, signer, dexSigner, serumVault, openOrdersAccounts,
                marketIndex, order);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="dexProgramIdKey"></param>
        /// <param name="spotMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="dexRequestQueue"></param>
        /// <param name="dexEventQueue"></param>
        /// <param name="dexBase"></param>
        /// <param name="dexQuote"></param>
        /// <param name="baseRootBank"></param>
        /// <param name="baseNodeBank"></param>
        /// <param name="baseVault"></param>
        /// <param name="quoteRootBank"></param>
        /// <param name="quoteNodeBank"></param>
        /// <param name="quoteVault"></param>
        /// <param name="signer"></param>
        /// <param name="dexSigner"></param>
        /// <param name="serumVault"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="marketIndex"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static TransactionInstruction PlaceSpotOrder2(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey dexProgramIdKey,
            PublicKey spotMarket,
            PublicKey bids, PublicKey asks, PublicKey dexRequestQueue, PublicKey dexEventQueue, PublicKey dexBase,
            PublicKey dexQuote, PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey baseVault,
            PublicKey quoteRootBank,
            PublicKey quoteNodeBank, PublicKey quoteVault, PublicKey signer, PublicKey dexSigner, PublicKey serumVault,
            IList<PublicKey> openOrdersAccounts, int marketIndex, Order order)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(dexProgramIdKey, false),
                AccountMeta.Writable(spotMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false),
                AccountMeta.Writable(dexRequestQueue, false),
                AccountMeta.Writable(dexEventQueue, false),
                AccountMeta.Writable(dexBase, false),
                AccountMeta.Writable(dexQuote, false),
                AccountMeta.ReadOnly(baseRootBank, false),
                AccountMeta.Writable(baseNodeBank, false),
                AccountMeta.Writable(baseVault, false),
                AccountMeta.ReadOnly(quoteRootBank, false),
                AccountMeta.Writable(quoteNodeBank, false),
                AccountMeta.Writable(quoteVault, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.ReadOnly(dexSigner, false),
                AccountMeta.ReadOnly(serumVault, false)
            };

            if (openOrdersAccounts.Count == 1)
            {
                keys.Add(AccountMeta.Writable(openOrdersAccounts[0], false));
            }
            else
            {
                keys.AddRange(openOrdersAccounts.Select((t, i) => i == marketIndex
                    ? AccountMeta.Writable(t, false)
                    : AccountMeta.ReadOnly(t, false)));
            }

            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodePlaceSpotOrder2Data(order),
                ProgramId = programIdKey,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="eventQueue"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="side"></param>
        /// <param name="orderType"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="clientOrderId"></param>
        /// <param name="reduceOnly"></param>
        /// <returns></returns>
        public static TransactionInstruction PlacePerpOrder(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey perpetualMarket,
            PublicKey bids, PublicKey asks, PublicKey eventQueue, IList<PublicKey> openOrdersAccounts,
            Side side, OrderType orderType, long price, long quantity, ulong clientOrderId, bool reduceOnly = false)
            => PlacePerpOrder(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, mangoCache, perpetualMarket,
                bids, asks, eventQueue, openOrdersAccounts, side, orderType, price, quantity, clientOrderId, reduceOnly);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="mangoCache"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="eventQueue"></param>
        /// <param name="openOrdersAccounts"></param>
        /// <param name="side"></param>
        /// <param name="orderType"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="clientOrderId"></param>
        /// <param name="reduceOnly"></param>
        /// <returns></returns>
        public static TransactionInstruction PlacePerpOrder(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey perpetualMarket,
            PublicKey bids, PublicKey asks, PublicKey eventQueue, IList<PublicKey> openOrdersAccounts,
            Side side, OrderType orderType, long price, long quantity, ulong clientOrderId, bool reduceOnly = false)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.Writable(perpetualMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false),
                AccountMeta.Writable(eventQueue, false)
            };
            keys.AddRange(openOrdersAccounts.Select(key => AccountMeta.ReadOnly(key, false)));

            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = programIdKey,
                Data = MangoProgramData.EncodePlacePerpOrderData(side, orderType, price, quantity, clientOrderId, reduceOnly)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="clientOrderId"></param>
        /// <param name="invalidIdOk"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelPerpOrderByClientId(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids,
            PublicKey asks, ulong clientOrderId, bool invalidIdOk)
            => CancelPerpOrderByClientId(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, perpetualMarket,
                bids, asks, clientOrderId, invalidIdOk);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="clientOrderId"></param>
        /// <param name="invalidIdOk"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelPerpOrderByClientId(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids,
            PublicKey asks, ulong clientOrderId, bool invalidIdOk)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.Writable(perpetualMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false)
            };
            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = programIdKey,
                Data = MangoProgramData.EncodeCancelPerpOrderByClientIdData(clientOrderId, invalidIdOk)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="orderId"></param>
        /// <param name="invalidIdOk"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelPerpOrder(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids,
            PublicKey asks, BigInteger orderId, bool invalidIdOk)
            => CancelPerpOrder(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, perpetualMarket, bids, asks, orderId,
                invalidIdOk);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="orderId"></param>
        /// <param name="invalidIdOk"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelPerpOrder(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids,
            PublicKey asks, BigInteger orderId, bool invalidIdOk)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.Writable(perpetualMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false)
            };
            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = programIdKey,
                Data = MangoProgramData.EncodeCancelPerpOrderData(orderId, invalidIdOk)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="dexProgramIdKey"></param>
        /// <param name="spotMarket"></param>
        /// <param name="openOrders"></param>
        /// <param name="signer"></param>
        /// <param name="dexBase"></param>
        /// <param name="dexQuote"></param>
        /// <param name="baseRootBank"></param>
        /// <param name="baseNodeBank"></param>
        /// <param name="quoteRootBank"></param>
        /// <param name="quoteNodeBank"></param>
        /// <param name="baseVault"></param>
        /// <param name="quoteVault"></param>
        /// <param name="dexSigner"></param>
        /// <returns></returns>
        public static TransactionInstruction SettleFunds(PublicKey mangoGroup, PublicKey mangoCache,
            PublicKey mangoAccount, PublicKey owner, PublicKey dexProgramIdKey, PublicKey spotMarket,
            PublicKey openOrders, PublicKey signer, PublicKey dexBase, PublicKey dexQuote, PublicKey baseRootBank,
            PublicKey baseNodeBank, PublicKey quoteRootBank, PublicKey quoteNodeBank, PublicKey baseVault,
            PublicKey quoteVault, PublicKey dexSigner)
            => SettleFunds(ProgramIdKeyV3, mangoGroup, mangoCache, mangoAccount, owner, dexProgramIdKey, spotMarket,
                openOrders, signer, dexBase, dexQuote, baseRootBank, baseNodeBank, quoteRootBank, quoteNodeBank,
                baseVault, quoteVault, dexSigner);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="dexProgramIdKey"></param>
        /// <param name="spotMarket"></param>
        /// <param name="openOrders"></param>
        /// <param name="signer"></param>
        /// <param name="dexBase"></param>
        /// <param name="dexQuote"></param>
        /// <param name="baseRootBank"></param>
        /// <param name="baseNodeBank"></param>
        /// <param name="quoteRootBank"></param>
        /// <param name="quoteNodeBank"></param>
        /// <param name="baseVault"></param>
        /// <param name="quoteVault"></param>
        /// <param name="dexSigner"></param>
        /// <returns></returns>
        public static TransactionInstruction SettleFunds(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoCache, PublicKey mangoAccount, PublicKey owner, PublicKey dexProgramIdKey,
            PublicKey spotMarket, PublicKey openOrders, PublicKey signer, PublicKey dexBase, PublicKey dexQuote,
            PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey quoteRootBank, PublicKey quoteNodeBank,
            PublicKey baseVault, PublicKey quoteVault, PublicKey dexSigner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(dexProgramIdKey, false),
                AccountMeta.Writable(spotMarket, false),
                AccountMeta.Writable(openOrders, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.Writable(dexBase, false),
                AccountMeta.Writable(dexQuote, false),
                AccountMeta.ReadOnly(baseRootBank, false),
                AccountMeta.Writable(baseNodeBank, false),
                AccountMeta.ReadOnly(quoteRootBank, false),
                AccountMeta.Writable(quoteNodeBank, false),
                AccountMeta.Writable(baseVault, false),
                AccountMeta.Writable(quoteVault, false),
                AccountMeta.ReadOnly(dexSigner, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
            };
            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeSettleFundsData(),
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="owner"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="spotMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="openOrders"></param>
        /// <param name="signer"></param>
        /// <param name="eventQueue"></param>
        /// <param name="orderId"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelSpotOrder(PublicKey mangoGroup,
            PublicKey owner, PublicKey mangoAccount, PublicKey spotMarket,
            PublicKey bids, PublicKey asks, PublicKey openOrders, PublicKey signer, PublicKey eventQueue,
            BigInteger orderId, Side side) => CancelSpotOrder(ProgramIdKeyV3, mangoGroup, owner, mangoAccount,
            SerumProgram.ProgramIdKey, spotMarket, bids, asks, openOrders, signer, eventQueue, orderId, side);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="owner"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="dexProgramIdKey"></param>
        /// <param name="spotMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="openOrders"></param>
        /// <param name="signer"></param>
        /// <param name="eventQueue"></param>
        /// <param name="orderId"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelSpotOrder(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey owner, PublicKey mangoAccount, PublicKey dexProgramIdKey, PublicKey spotMarket,
            PublicKey bids, PublicKey asks, PublicKey openOrders, PublicKey signer, PublicKey eventQueue,
            BigInteger orderId, Side side)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(mangoAccount, false),
                AccountMeta.ReadOnly(dexProgramIdKey, false),
                AccountMeta.Writable(spotMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false),
                AccountMeta.Writable(openOrders, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.Writable(eventQueue, false)
            };
            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeCancelSpotOrderData(side, orderId),
                ProgramId = programIdKey
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccountA"></param>
        /// <param name="mangoAccountB"></param>
        /// <param name="mangoCache"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="marketIndex"></param>
        /// <returns></returns>
        public static TransactionInstruction SettleProfitAndLoss(PublicKey mangoGroup, PublicKey mangoAccountA,
            PublicKey mangoAccountB, PublicKey mangoCache, PublicKey rootBank, PublicKey nodeBank, ulong marketIndex)
            => SettleProfitAndLoss(ProgramIdKeyV3, mangoGroup, mangoAccountA, mangoAccountB, mangoCache, rootBank,
                nodeBank, marketIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccountA"></param>
        /// <param name="mangoAccountB"></param>
        /// <param name="mangoCache"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="marketIndex"></param>
        /// <returns></returns>
        public static TransactionInstruction SettleProfitAndLoss(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccountA,
            PublicKey mangoAccountB, PublicKey mangoCache, PublicKey rootBank, PublicKey nodeBank, ulong marketIndex)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccountA, false),
                AccountMeta.Writable(mangoAccountB, false),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(rootBank, false),
                AccountMeta.Writable(nodeBank, false)
            };
            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeSettleProfitAndLossData(marketIndex),
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="dexProgramIdKey"></param>
        /// <param name="openOrders"></param>
        /// <param name="spotMarket"></param>
        /// <param name="signer"></param>
        /// <returns></returns>
        public static TransactionInstruction InitSpotOpenOrders(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey dexProgramIdKey, PublicKey openOrders, PublicKey spotMarket, PublicKey signer)
            => InitSpotOpenOrders(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, dexProgramIdKey,
                openOrders, spotMarket, signer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="dexProgramIdKey"></param>
        /// <param name="openOrders"></param>
        /// <param name="spotMarket"></param>
        /// <param name="signer"></param>
        /// <returns></returns>
        public static TransactionInstruction InitSpotOpenOrders(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount,
            PublicKey owner, PublicKey dexProgramIdKey, PublicKey openOrders, PublicKey spotMarket, PublicKey signer)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(dexProgramIdKey, false),
                AccountMeta.Writable(openOrders, false),
                AccountMeta.ReadOnly(spotMarket, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false)
            };

            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeInitSpotOpenOrdersData(),
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="mangoPerpetualVault"></param>
        /// <param name="mangoRootBank"></param>
        /// <param name="mangoNodeBank"></param>
        /// <param name="mangoBankVault"></param>
        /// <param name="signer"></param>
        /// <returns></returns>
        public static TransactionInstruction RedeemMango(PublicKey mangoGroup, PublicKey mangoCache,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey mangoPerpetualVault,
            PublicKey mangoRootBank, PublicKey mangoNodeBank, PublicKey mangoBankVault, PublicKey signer)
            => RedeemMango(ProgramIdKeyV3, mangoGroup, mangoCache, mangoAccount, owner, perpetualMarket, mangoPerpetualVault,
                mangoRootBank, mangoNodeBank, mangoBankVault, signer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="mangoPerpetualVault"></param>
        /// <param name="mangoRootBank"></param>
        /// <param name="mangoNodeBank"></param>
        /// <param name="mangoBankVault"></param>
        /// <param name="signer"></param>
        /// <returns></returns>
        public static TransactionInstruction RedeemMango(PublicKey programIdKey, PublicKey mangoGroup, PublicKey mangoCache,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey mangoPerpetualVault,
            PublicKey mangoRootBank, PublicKey mangoNodeBank, PublicKey mangoBankVault, PublicKey signer)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(perpetualMarket, false),
                AccountMeta.Writable(mangoPerpetualVault, false),
                AccountMeta.ReadOnly(mangoRootBank, false),
                AccountMeta.Writable(mangoNodeBank, false),
                AccountMeta.Writable(mangoBankVault, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeRedeemMangoData(),
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static TransactionInstruction AddMangoAccountInfo(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, string info) =>
            AddMangoAccountInfo(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static TransactionInstruction AddMangoAccountInfo(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, string info)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
            };
            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeAddMangoAccountInfoData(info),
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelAllPerpOrders(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids, PublicKey asks,
            byte limit) => CancelAllPerpOrders(ProgramIdKeyV3, mangoGroup, mangoAccount, owner, perpetualMarket, bids, asks, limit);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="owner"></param>
        /// <param name="perpetualMarket"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static TransactionInstruction CancelAllPerpOrders(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids, PublicKey asks,
            byte limit)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.Writable(perpetualMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false)
            };
            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeCancelAllPerpOrdersData(limit),
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// Decodes an instruction created by the System Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint instruction = data.GetU32(MangoProgramLayouts.MethodOffset);
            MangoProgramInstructions.Values instructionValue =
                (MangoProgramInstructions.Values)Enum.Parse(typeof(MangoProgramInstructions.Values),
                    instruction.ToString());

            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKeyV3,
                InstructionName = MangoProgramInstructions.Names[instructionValue],
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>(),
                InnerInstructions = new List<DecodedInstruction>()
            };

            switch (instructionValue)
            {
                case MangoProgramInstructions.Values.InitMangoAccount:
                    MangoProgramData.DecodeInitMangoAccountData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.Deposit:
                    MangoProgramData.DecodeDepositData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.Withdraw:
                    MangoProgramData.DecodeWithdrawData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.PlaceSpotOrder:
                    MangoProgramData.DecodePlaceSpotOrderData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.PlacePerpOrder:
                    MangoProgramData.DecodePlacePerpOrderData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CancelPerpOrderByClientId:
                    MangoProgramData.DecodeCancelPerpOrderByClientIdData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CancelPerpOrder:
                    MangoProgramData.DecodeCancelPerpOrderData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.SettleFunds:
                    MangoProgramData.DecodeSettleFundsData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CancelSpotOrder:
                    MangoProgramData.DecodeCancelSpotOrderData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.SettleProfitAndLoss:
                    MangoProgramData.DecodeSettleProfitAndLossData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.InitSpotOpenOrders:
                    MangoProgramData.DecodeInitSpotOpenOrdersData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.RedeemMango:
                    MangoProgramData.DecodeRedeemMangoData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.AddMangoAccountInfo:
                    MangoProgramData.DecodeAddMangoAccountInfoData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CancelAllPerpOrders:
                    MangoProgramData.DecodeCancelAllPerpOrdersData(decodedInstruction, data, keys, keyIndices);
                    break;
            }

            return decodedInstruction;
        }
    }
}