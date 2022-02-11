using Solnet.Mango.Models;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Programs.Abstract;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
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
    /// https://github.com/blockworks-foundation/
    /// </remarks>
    /// </summary>
    public class MangoProgram : BaseProgram
    {
        /// <summary>
        /// The public key of the MNGO token mint.
        /// </summary>
        public static readonly PublicKey MangoToken = new("MangoCzJ36AjZyKwVj3VnYU4GTonjfVEnJmvvWaxLac");

        /// <summary>
        /// The public key of the Mango V3 program on <see cref="Cluster.DevNet"/>.
        /// </summary>
        public static readonly PublicKey DevNetProgramIdKeyV3 = new("4skJ85cdxQAFVKbcGgfun8iZPL7BadVYXG3kGEGkufqA");

        /// <summary>
        /// The public key of the Mango V3 program on <see cref="Cluster.MainNet"/>.
        /// </summary>
        public static readonly PublicKey MainNetProgramIdKeyV3 = new("mv3ekLzLbnVPNxjSKvqBpU3ZeZXPQdEC3bp5MDEBG68");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string DefaultProgramName = "Mango Program V3";

        /// <summary>
        /// The dex program id key.
        /// </summary>
        private PublicKey _dexProgramIdKey;

        /// <summary>
        /// Initialize the <see cref="MangoProgram"/> with the given program id key and program name.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="dexProgramIdKey">The serum dex program id key.</param>
        /// <param name="programName">The program name.</param>
        public MangoProgram(PublicKey programIdKey, PublicKey dexProgramIdKey, string programName = DefaultProgramName) : base(programIdKey, programName) 
        {
            _dexProgramIdKey = dexProgramIdKey;
        }

        /// <summary>
        /// Initialize the <see cref="MangoProgram"/> for <see cref="Cluster.DevNet"/>.
        /// </summary>
        /// <returns>The <see cref="MangoProgram"/> instance.</returns>
        public static MangoProgram CreateDevNet() => new MangoProgram(DevNetProgramIdKeyV3, SerumProgram.DevNetProgramIdKeyV3);

        /// <summary>
        /// Initialize the <see cref="MangoProgram"/> for <see cref="Cluster.MainNet"/>.
        /// </summary>
        /// <returns>The <see cref="MangoProgram"/> instance.</returns>
        public static MangoProgram CreateMainNet() => new MangoProgram(MainNetProgramIdKeyV3, SerumProgram.MainNetProgramIdKeyV3);

        /// <summary>
        /// Initializes an instruction to initialize a given account as a <see cref="MangoAccount"/>.
        /// </summary>
        /// <remarks>
        /// DEPRECATED: If you use this method after Mango v3.3 you will not be able to upgrade and close your account.
        /// </remarks>
        /// <param name="mangoGroup">The public key of the <see cref="MangoGroup"/> account.</param>
        /// <param name="mangoAccount">The public key of the account to initialize as <see cref="MangoAccount"/>.</param>
        /// <param name="owner">The public key of the owner of the <see cref="MangoAccount"/>.</param>
        /// <returns>The transaction instruction.</returns>
        [Obsolete("Please use CreateMangoAccount whenever possible instead. If you use this method after Mango v3.3 you will not be able to upgrade and close your account.")]
        public TransactionInstruction InitializeMangoAccount(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner) => InitializeMangoAccount(ProgramIdKey, mangoGroup, mangoAccount, owner);

        /// <summary>
        /// Initializes an instruction to initialize a given account as a <see cref="MangoAccount"/>.
        /// </summary>
        /// <remarks>
        /// DEPRECATED: If you use this method after Mango v3.3 you will not be able to upgrade and close your account.
        /// </remarks>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="mangoGroup">The public key of the <see cref="MangoGroup"/> account.</param>
        /// <param name="mangoAccount">The public key of the account to initialize as <see cref="MangoAccount"/>.</param>
        /// <param name="owner">The public key of the owner of the <see cref="MangoAccount"/>.</param>
        /// <returns>The transaction instruction.</returns>
        [Obsolete("Please use CreateMangoAccount whenever possible instead. If you use this method after Mango v3.3 you will not be able to upgrade and close your account.")]
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.Deposit"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="nodeBank">The node bank.</param>
        /// <param name="nodeBankVault">The node bank's vault.</param>
        /// <param name="ownerTokenAccount">The owner's token account.</param>
        /// <param name="quantity">The amount to deposit.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction Deposit(PublicKey mangoGroup, PublicKey mangoAccount, PublicKey owner,
            PublicKey mangoCache, PublicKey rootBank, PublicKey nodeBank, PublicKey nodeBankVault, PublicKey ownerTokenAccount,
            ulong quantity)
            => Deposit(ProgramIdKey, mangoGroup, mangoAccount, owner, mangoCache, rootBank, nodeBank, nodeBankVault,
                ownerTokenAccount, quantity);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.Deposit"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="nodeBank">The node bank.</param>
        /// <param name="nodeBankVault">The node bank's vault.</param>
        /// <param name="ownerTokenAccount">The owner's token account.</param>
        /// <param name="quantity">The amount to deposit.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction Deposit(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey rootBank,
            PublicKey nodeBank, PublicKey nodeBankVault, PublicKey ownerTokenAccount, ulong quantity)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(rootBank, false),
                AccountMeta.Writable(nodeBank, false),
                AccountMeta.Writable(nodeBankVault, false),
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.Withdraw"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="nodeBank">The node bank.</param>
        /// <param name="nodeBankVault">The node bank's vault.</param>
        /// <param name="ownerTokenAccount">The owner's token account.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="quantity">The amount to withdraw.</param>
        /// <param name="allowBorrow">Whether to allow borrowing or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction Withdraw(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey rootBank,
            PublicKey nodeBank, PublicKey nodeBankVault, PublicKey ownerTokenAccount, PublicKey signer,
            IList<PublicKey> openOrdersAccounts, ulong quantity, bool allowBorrow)
            => Withdraw(ProgramIdKey, mangoGroup, mangoAccount, owner, mangoCache, rootBank, nodeBank, nodeBankVault,
                ownerTokenAccount, signer, openOrdersAccounts, quantity, allowBorrow);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.Withdraw"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="nodeBank">The node bank.</param>
        /// <param name="nodeBankVault">The node bank's vault.</param>
        /// <param name="ownerTokenAccount">The owner's token account.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="quantity">The amount to withdraw.</param>
        /// <param name="allowBorrow">Whether to allow borrowing or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction Withdraw(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey rootBank,
            PublicKey nodeBank, PublicKey nodeBankVault, PublicKey ownerTokenAccount, PublicKey signer,
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
                AccountMeta.Writable(nodeBankVault, false),
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="spotMarket">The spot market account.</param>
        /// <param name="bids">The spot market bids account.</param>
        /// <param name="asks">The spot market asks account.</param>
        /// <param name="dexRequestQueue">The spot market request queue.</param>
        /// <param name="dexEventQueue">The spot market event queue.</param>
        /// <param name="dexBase">The spot market base vault.</param>
        /// <param name="dexQuote">The spot market quote vault.</param>
        /// <param name="baseRootBank">The root bank of the base mint.</param>
        /// <param name="baseNodeBank">The node bank of the base mint.</param>
        /// <param name="baseVault">The vault of the base mint's node bank.</param>
        /// <param name="quoteRootBank">The root bank of the quote mint.</param>
        /// <param name="quoteNodeBank">The node bank of the quote mint.</param>
        /// <param name="quoteVault">The vault of the quote mint's node bank.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="dexSigner">The dex signer (derived from the spot market's vault signer nonce).</param>
        /// <param name="serumVault">The spot market's vault.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="marketIndex">The market's index.</param>
        /// <param name="order">The order.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        [Obsolete("This method is sub optimal, please use PlaceSpotOrder2 instead.")]
        public TransactionInstruction PlaceSpotOrder(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey mangoCache, PublicKey spotMarket, PublicKey bids, PublicKey asks,
            PublicKey dexRequestQueue, PublicKey dexEventQueue, PublicKey dexBase, PublicKey dexQuote,
            PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey baseVault, PublicKey quoteRootBank,
            PublicKey quoteNodeBank, PublicKey quoteVault, PublicKey signer, PublicKey dexSigner, PublicKey serumVault,
            IList<PublicKey> openOrdersAccounts, int marketIndex, Order order)
            => PlaceSpotOrder(ProgramIdKey, mangoGroup, mangoAccount, owner, mangoCache, _dexProgramIdKey,
                spotMarket, bids, asks, dexRequestQueue, dexEventQueue, dexBase, dexQuote, baseRootBank, baseNodeBank,
                baseVault, quoteRootBank, quoteNodeBank, quoteVault, signer, dexSigner, serumVault, openOrdersAccounts,
                marketIndex, order);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="dexProgramIdKey">The serum dex program id key.</param>
        /// <param name="spotMarket">The spot market account.</param>
        /// <param name="bids">The spot market bids account.</param>
        /// <param name="asks">The spot market asks account.</param>
        /// <param name="dexRequestQueue">The spot market request queue.</param>
        /// <param name="dexEventQueue">The spot market event queue.</param>
        /// <param name="dexBase">The spot market base vault.</param>
        /// <param name="dexQuote">The spot market quote vault.</param>
        /// <param name="baseRootBank">The root bank of the base mint.</param>
        /// <param name="baseNodeBank">The node bank of the base mint.</param>
        /// <param name="baseVault">The vault of the base mint's node bank.</param>
        /// <param name="quoteRootBank">The root bank of the quote mint.</param>
        /// <param name="quoteNodeBank">The node bank of the quote mint.</param>
        /// <param name="quoteVault">The vault of the quote mint's node bank.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="dexSigner">The dex signer (derived from the spot market's vault signer nonce).</param>
        /// <param name="serumVault">The spot market's vault.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="marketIndex">The market's index.</param>
        /// <param name="order">The order.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        [Obsolete("This method is sub optimal, please use PlaceSpotOrder2 instead.")]
        public static TransactionInstruction PlaceSpotOrder(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey dexProgramIdKey, PublicKey spotMarket,
            PublicKey bids, PublicKey asks, PublicKey dexRequestQueue, PublicKey dexEventQueue, PublicKey dexBase,
            PublicKey dexQuote, PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey baseVault, PublicKey quoteRootBank,
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
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(dexSigner, false),
                AccountMeta.ReadOnly(serumVault, false)
            };

            keys.AddRange(openOrdersAccounts.Select((t, i) => i == marketIndex
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder2"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="spotMarket">The spot market account.</param>
        /// <param name="bids">The spot market bids account.</param>
        /// <param name="asks">The spot market asks account.</param>
        /// <param name="dexRequestQueue">The spot market request queue.</param>
        /// <param name="dexEventQueue">The spot market event queue.</param>
        /// <param name="dexBase">The spot market base vault.</param>
        /// <param name="dexQuote">The spot market quote vault.</param>
        /// <param name="baseRootBank">The root bank of the base mint.</param>
        /// <param name="baseNodeBank">The node bank of the base mint.</param>
        /// <param name="baseVault">The vault of the base mint's node bank.</param>
        /// <param name="quoteRootBank">The root bank of the quote mint.</param>
        /// <param name="quoteNodeBank">The node bank of the quote mint.</param>
        /// <param name="quoteVault">The vault of the quote mint's node bank.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="dexSigner">The dex signer (derived from the spot market's vault signer nonce).</param>
        /// <param name="serumVault">The spot market's vault.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="marketIndex">The market's index.</param>
        /// <param name="order">The order.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction PlaceSpotOrder2(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey mangoCache, PublicKey spotMarket, PublicKey bids, PublicKey asks,
            PublicKey dexRequestQueue, PublicKey dexEventQueue, PublicKey dexBase, PublicKey dexQuote,
            PublicKey baseRootBank, PublicKey baseNodeBank, PublicKey baseVault, PublicKey quoteRootBank,
            PublicKey quoteNodeBank, PublicKey quoteVault, PublicKey signer, PublicKey dexSigner, PublicKey serumVault,
            IList<PublicKey> openOrdersAccounts, int marketIndex, Order order)
            => PlaceSpotOrder2(ProgramIdKey, mangoGroup, mangoAccount, owner, mangoCache,
                _dexProgramIdKey, spotMarket, bids, asks, dexRequestQueue, dexEventQueue, dexBase, dexQuote,
                baseRootBank, baseNodeBank,baseVault, quoteRootBank, quoteNodeBank, quoteVault, signer, dexSigner,
                serumVault, openOrdersAccounts, marketIndex, order);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.PlaceSpotOrder2"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="dexProgramIdKey">The serum dex program id key.</param>
        /// <param name="spotMarket">The spot market account.</param>
        /// <param name="bids">The spot market bids account.</param>
        /// <param name="asks">The spot market asks account.</param>
        /// <param name="dexRequestQueue">The spot market request queue.</param>
        /// <param name="dexEventQueue">The spot market event queue.</param>
        /// <param name="dexBase">The spot market base vault.</param>
        /// <param name="dexQuote">The spot market quote vault.</param>
        /// <param name="baseRootBank">The root bank of the base mint.</param>
        /// <param name="baseNodeBank">The node bank of the base mint.</param>
        /// <param name="baseVault">The vault of the base mint's node bank.</param>
        /// <param name="quoteRootBank">The root bank of the quote mint.</param>
        /// <param name="quoteNodeBank">The node bank of the quote mint.</param>
        /// <param name="quoteVault">The vault of the quote mint's node bank.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="dexSigner">The dex signer (derived from the spot market's vault signer nonce).</param>
        /// <param name="serumVault">The spot market's vault.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="marketIndex">The market's index.</param>
        /// <param name="order">The order.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
            keys.AddRange(openOrdersAccounts.Select((t, i) => i == marketIndex
                ? AccountMeta.Writable(t, false)
                : AccountMeta.ReadOnly(t, false)));

            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodePlaceSpotOrder2Data(order),
                ProgramId = programIdKey,
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.PlacePerpOrder"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="eventQueue">The perp market event queue.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="side">The side of the order.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="reduceOnly">Whether the order is reduce only or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction PlacePerpOrder(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey perpetualMarket,
            PublicKey bids, PublicKey asks, PublicKey eventQueue, IList<PublicKey> openOrdersAccounts,
            Side side, PerpOrderType orderType, long price, long quantity, ulong clientOrderId, bool reduceOnly = false)
            => PlacePerpOrder(ProgramIdKey, mangoGroup, mangoAccount, owner, mangoCache, perpetualMarket,
                bids, asks, eventQueue, openOrdersAccounts, side, orderType, price, quantity, clientOrderId, reduceOnly);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.PlacePerpOrder"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="eventQueue">The perp market event queue.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="side">The side of the order.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="reduceOnly">Whether the order is reduce only or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction PlacePerpOrder(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey mangoCache, PublicKey perpetualMarket,
            PublicKey bids, PublicKey asks, PublicKey eventQueue, IList<PublicKey> openOrdersAccounts,
            Side side, PerpOrderType orderType, long price, long quantity, ulong clientOrderId, bool reduceOnly = false)
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelPerpOrderByClientId"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="invalidIdOk">Whether an invalid id is ok or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CancelPerpOrderByClientId(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids,
            PublicKey asks, ulong clientOrderId, bool invalidIdOk)
            => CancelPerpOrderByClientId(ProgramIdKey, mangoGroup, mangoAccount, owner, perpetualMarket,
                bids, asks, clientOrderId, invalidIdOk);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelPerpOrderByClientId"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="invalidIdOk">Whether an invalid id is ok or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelPerpOrder"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="invalidIdOk">Whether an invalid id is ok or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CancelPerpOrder(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids,
            PublicKey asks, BigInteger orderId, bool invalidIdOk)
            => CancelPerpOrder(ProgramIdKey, mangoGroup, mangoAccount, owner, perpetualMarket, bids, asks, orderId,
                invalidIdOk);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelPerpOrder"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="invalidIdOk">Whether an invalid id is ok or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SettleFunds"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="openOrders">The open orders accounts.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="dexBase">The spot market base vault.</param>
        /// <param name="dexQuote">The spot market quote vault.</param>
        /// <param name="baseRootBank">The root bank of the base mint.</param>
        /// <param name="baseNodeBank">The node bank of the base mint.</param>
        /// <param name="quoteRootBank">The root bank of the quote mint.</param>
        /// <param name="quoteNodeBank">The node bank of the quote mint.</param>
        /// <param name="baseVault">The vault of the base mint's node bank.</param>
        /// <param name="quoteVault">The vault of the quote mint's node bank.</param>
        /// <param name="dexSigner">The dex signer (derived from the spot market's vault signer nonce).</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction SettleFunds(PublicKey mangoGroup, PublicKey mangoCache,
            PublicKey mangoAccount, PublicKey owner, PublicKey spotMarket,
            PublicKey openOrders, PublicKey signer, PublicKey dexBase, PublicKey dexQuote, PublicKey baseRootBank,
            PublicKey baseNodeBank, PublicKey quoteRootBank, PublicKey quoteNodeBank, PublicKey baseVault,
            PublicKey quoteVault, PublicKey dexSigner)
            => SettleFunds(ProgramIdKey, mangoGroup, mangoCache, mangoAccount, owner, _dexProgramIdKey, spotMarket,
                openOrders, signer, dexBase, dexQuote, baseRootBank, baseNodeBank, quoteRootBank, quoteNodeBank,
                baseVault, quoteVault, dexSigner);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SettleFunds"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="dexProgramIdKey">The serum dex program id key.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="openOrders">The open orders accounts.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="dexBase">The spot market base vault.</param>
        /// <param name="dexQuote">The spot market quote vault.</param>
        /// <param name="baseRootBank">The root bank of the base mint.</param>
        /// <param name="baseNodeBank">The node bank of the base mint.</param>
        /// <param name="quoteRootBank">The root bank of the quote mint.</param>
        /// <param name="quoteNodeBank">The node bank of the quote mint.</param>
        /// <param name="baseVault">The vault of the base mint's node bank.</param>
        /// <param name="quoteVault">The vault of the quote mint's node bank.</param>
        /// <param name="dexSigner">The dex signer (derived from the spot market's vault signer nonce).</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SettleFees"/> method.
        /// </summary>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="perpMarket"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="bankVault"></param>
        /// <param name="feesVault"></param>
        /// <param name="signer">The mango signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction SettleFees(PublicKey mangoGroup, PublicKey mangoCache,
            PublicKey perpMarket, PublicKey mangoAccount, PublicKey rootBank, PublicKey nodeBank,
            PublicKey bankVault, PublicKey feesVault, PublicKey signer)
            => SettleFees(ProgramIdKey, mangoGroup, mangoCache, perpMarket, mangoAccount,
                rootBank, nodeBank, bankVault, feesVault, signer);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SettleFees"/> method.
        /// </summary>
        /// <param name="programIdKey"></param>
        /// <param name="mangoGroup"></param>
        /// <param name="mangoCache"></param>
        /// <param name="perpMarket"></param>
        /// <param name="mangoAccount"></param>
        /// <param name="rootBank"></param>
        /// <param name="nodeBank"></param>
        /// <param name="bankVault"></param>
        /// <param name="feesVault"></param>
        /// <param name="signer">The mango signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction SettleFees(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoCache, PublicKey perpMarket, PublicKey mangoAccount, PublicKey rootBank, PublicKey nodeBank,
            PublicKey bankVault, PublicKey feesVault, PublicKey signer)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.Writable(perpMarket, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(rootBank, false),
                AccountMeta.Writable(nodeBank, false),
                AccountMeta.Writable(bankVault, false),
                AccountMeta.Writable(feesVault, false),
                AccountMeta.ReadOnly(signer, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                Keys = keys,
                Data = MangoProgramData.EncodeSettleFeesData(),
                ProgramId = programIdKey
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelSpotOrder"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="bids">The spot market bids.</param>
        /// <param name="asks">The spot market asks.</param>
        /// <param name="openOrders">The open orders accounts.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="eventQueue">The spot market event queue</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="side">The side of the order.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CancelSpotOrder(PublicKey mangoGroup,
            PublicKey owner, PublicKey mangoAccount, PublicKey spotMarket,
            PublicKey bids, PublicKey asks, PublicKey openOrders, PublicKey signer, PublicKey eventQueue,
            BigInteger orderId, Side side) => CancelSpotOrder(ProgramIdKey, mangoGroup, owner, mangoAccount,
            _dexProgramIdKey, spotMarket, bids, asks, openOrders, signer, eventQueue, orderId, side);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelSpotOrder"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="dexProgramIdKey">The serum dex program id key.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="bids">The spot market bids.</param>
        /// <param name="asks">The spot market asks.</param>
        /// <param name="openOrders">The open orders accounts.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <param name="eventQueue">The spot market event queue</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="side">The side of the order.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SettleProfitAndLoss"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccountA">The mango account A.</param>
        /// <param name="mangoAccountB">The mango account B.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="nodeBank">The node bank.</param>
        /// <param name="marketIndex">The market's index.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction SettleProfitAndLoss(PublicKey mangoGroup, PublicKey mangoAccountA,
            PublicKey mangoAccountB, PublicKey mangoCache, PublicKey rootBank, PublicKey nodeBank, ulong marketIndex)
            => SettleProfitAndLoss(ProgramIdKey, mangoGroup, mangoAccountA, mangoAccountB, mangoCache, rootBank,
                nodeBank, marketIndex);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SettleProfitAndLoss"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccountA">The mango account A.</param>
        /// <param name="mangoAccountB">The mango account B.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="rootBank">The root bank.</param>
        /// <param name="nodeBank">The node bank.</param>
        /// <param name="marketIndex">The market's index.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.InitSpotOpenOrders"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="openOrders">The open orders account.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction InitSpotOpenOrders(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey openOrders, PublicKey spotMarket, PublicKey signer)
            => InitSpotOpenOrders(ProgramIdKey, mangoGroup, mangoAccount, owner, _dexProgramIdKey,
                openOrders, spotMarket, signer);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.InitSpotOpenOrders"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="dexProgramIdKey">The serum dex program id key.</param>
        /// <param name="openOrders">The open orders account.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.RedeemMango"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">the mango cache.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="mangoPerpetualVault">The mango perp vault.</param>
        /// <param name="mangoRootBank">The mango token root bank.</param>
        /// <param name="mangoNodeBank">The mango token node bank</param>
        /// <param name="mangoBankVault">The mango token's node bank's vault.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction RedeemMango(PublicKey mangoGroup, PublicKey mangoCache,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey mangoPerpetualVault,
            PublicKey mangoRootBank, PublicKey mangoNodeBank, PublicKey mangoBankVault, PublicKey signer)
            => RedeemMango(ProgramIdKey, mangoGroup, mangoCache, mangoAccount, owner, perpetualMarket, mangoPerpetualVault,
                mangoRootBank, mangoNodeBank, mangoBankVault, signer);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.RedeemMango"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoCache">the mango cache.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="mangoPerpetualVault">The mango perp vault.</param>
        /// <param name="mangoRootBank">The mango token root bank.</param>
        /// <param name="mangoNodeBank">The mango token node bank</param>
        /// <param name="mangoBankVault">The mango token's node bank's vault.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.AddMangoAccountInfo"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="info">The info to add to the account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction AddMangoAccountInfo(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, string info) =>
            AddMangoAccountInfo(ProgramIdKey, mangoGroup, mangoAccount, owner, info);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.AddMangoAccountInfo"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="info">The info to add to the account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelAllPerpOrders"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="limit">The maximum number of orders to cancel.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CancelAllPerpOrders(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpetualMarket, PublicKey bids, PublicKey asks,
            byte limit) => CancelAllPerpOrders(ProgramIdKey, mangoGroup, mangoAccount, owner, perpetualMarket, bids, asks, limit);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelAllPerpOrders"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="perpetualMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="limit">The maximum number of orders to cancel.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
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
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.InitAdvancedOrders"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction InitAdvancedOrders(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey advancedOrders)
            => InitAdvancedOrders(ProgramIdKey, mangoGroup, mangoAccount, owner, advancedOrders);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.InitAdvancedOrders"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction InitAdvancedOrders(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey advancedOrders)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.Writable(advancedOrders, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeInitAdvancedOrdersData()
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.AddPerpTriggerOrder"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="side">The side of the order.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="triggerCondition">The trigger condition.</param>
        /// <param name="triggerPrice">The trigger price.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="reduceOnly">Whether the order is reduce only or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction AddPerpTriggerOrder(PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey advancedOrders, PublicKey mangoCache, PublicKey perpMarket,
            List<PublicKey> openOrdersAccounts, PerpOrderType orderType, Side side, long price, long quantity,
            TriggerCondition triggerCondition, I80F48 triggerPrice, ulong clientOrderId, bool reduceOnly = false)
            => AddPerpTriggerOrder(ProgramIdKey, mangoGroup, mangoAccount, owner, advancedOrders, mangoCache,
                perpMarket, openOrdersAccounts, orderType, side, price, quantity, triggerCondition, triggerPrice, clientOrderId, reduceOnly);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.AddPerpTriggerOrder"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <param name="mangoCache">The mango cache.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <param name="openOrdersAccounts">The open orders accounts.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="side">The side of the order.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="triggerCondition">The trigger condition.</param>
        /// <param name="triggerPrice">The trigger price.</param>
        /// <param name="clientOrderId">The client order id.</param>
        /// <param name="reduceOnly">Whether the order is reduce only or not.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction AddPerpTriggerOrder(PublicKey programIdKey, PublicKey mangoGroup, PublicKey mangoAccount,
            PublicKey owner, PublicKey advancedOrders, PublicKey mangoCache, PublicKey perpMarket,
            List<PublicKey> openOrdersAccounts, PerpOrderType orderType, Side side, long price, long quantity,
            TriggerCondition triggerCondition, I80F48 triggerPrice, ulong clientOrderId, bool reduceOnly = false)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.ReadOnly(mangoAccount, false),
                AccountMeta.Writable(owner, true),
                AccountMeta.Writable(advancedOrders, false),
                AccountMeta.ReadOnly(mangoCache, false),
                AccountMeta.ReadOnly(perpMarket, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };
            keys.AddRange(openOrdersAccounts.Select(key => AccountMeta.ReadOnly(key, false)));

            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeAddPerpTriggerOrderData(orderType, side, triggerCondition, reduceOnly, clientOrderId, price, quantity, triggerPrice)
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.RemoveAdvancedOrder"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <param name="orderIndex">The order index.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction RemoveAdvancedOrder(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey advancedOrders, byte orderIndex)
            => RemoveAdvancedOrder(ProgramIdKey, mangoGroup, mangoAccount, owner, advancedOrders, orderIndex);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.RemoveAdvancedOrder"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <param name="orderIndex">The order index.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction RemoveAdvancedOrder(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey advancedOrders, byte orderIndex)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.ReadOnly(mangoAccount, false),
                AccountMeta.Writable(owner, true),
                AccountMeta.Writable(advancedOrders, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };

            return new TransactionInstruction()
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeRemoveAdvancedOrderData(orderIndex)
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CloseMangoAccount"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CloseMangoAccount(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner)
            => CloseMangoAccount(ProgramIdKey, mangoGroup, mangoAccount, owner);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CloseMangoAccount"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction CloseMangoAccount(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.Writable(owner, true),
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeCloseMangoAccountData()
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CloseAdvancedOrders"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CloseAdvancedOrders(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey advancedOrders)
            => CloseAdvancedOrders(ProgramIdKey, mangoGroup, mangoAccount, owner, advancedOrders);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CloseAdvancedOrders"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="advancedOrders">The advanced orders account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction CloseAdvancedOrders(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey advancedOrders)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.Writable(owner, true),
                AccountMeta.Writable(advancedOrders, false),
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeCloseAdvancedOrdersData()
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CloseSpotOpenOrders"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="openOrdersAccount">The open orders account.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CloseSpotOpenOrders(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey openOrdersAccount, PublicKey spotMarket,
            PublicKey signer)
            => CloseSpotOpenOrders(ProgramIdKey, mangoGroup, mangoAccount, owner, _dexProgramIdKey, openOrdersAccount, spotMarket, signer);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CloseSpotOpenOrders"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="dexProgramIdKey">The serum dex program id key.</param>
        /// <param name="openOrdersAccount">The open orders account.</param>
        /// <param name="spotMarket">The spot market.</param>
        /// <param name="signer">The mango group signer.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction CloseSpotOpenOrders(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey dexProgramIdKey, PublicKey openOrdersAccount, PublicKey spotMarket,
            PublicKey signer)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.Writable(owner, true),
                AccountMeta.ReadOnly(dexProgramIdKey, false),
                AccountMeta.Writable(openOrdersAccount, false),
                AccountMeta.ReadOnly(spotMarket, false),
                AccountMeta.ReadOnly(signer, false)
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeCloseSpotOpenOrdersData()
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelPerpOrdersSide"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="side">The order side.</param>
        /// <param name="limit">The maximum number of orders to close.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CancelPerpOrdersSide(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpMarket, PublicKey bids,
            PublicKey asks, Side side, byte limit)
            => CancelPerpOrdersSide(ProgramIdKey, mangoGroup, mangoAccount, owner, perpMarket, bids, asks, side, limit);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CancelPerpOrdersSide"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="perpMarket">The perp market.</param>
        /// <param name="bids">The perp market bids.</param>
        /// <param name="asks">The perp market asks.</param>
        /// <param name="side">The order side.</param>
        /// <param name="limit">The maximum number of orders to close.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction CancelPerpOrdersSide(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey perpMarket, PublicKey bids,
            PublicKey asks, Side side, byte limit)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.Writable(perpMarket, false),
                AccountMeta.Writable(bids, false),
                AccountMeta.Writable(asks, false),
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeCancelPerpOrdersSideData(side, limit)
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CreateMangoAccount"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="accountNum">The account number for the PDA seeds.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction CreateMangoAccount(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, ulong accountNum)
            => CreateMangoAccount(ProgramIdKey, mangoGroup, mangoAccount, owner, accountNum);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.CreateMangoAccount"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="accountNum">The account number for the PDA seeds.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction CreateMangoAccount(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, ulong accountNum)
        {

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeCreateMangoAccountData(accountNum)
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.UpgradeMangoAccountV0V1"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction UpgradeMangoAccountV0V1(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner)
            => UpgradeMangoAccountV0V1(ProgramIdKey, mangoGroup, mangoAccount, owner);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.UpgradeMangoAccountV0V1"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction UpgradeMangoAccountV0V1(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner)
        {

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeUpgradeMangoAccountV0V1Data()
            };
        }

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SetDelegate"/> method.
        /// </summary>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="delegateAccount">The delegate account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public TransactionInstruction SetDelegate(PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey delegateAccount)
            => SetDelegate(ProgramIdKey, mangoGroup, mangoAccount, owner, delegateAccount);

        /// <summary>
        /// Initialize a new <see cref="TransactionInstruction"/> for the <see cref="MangoProgramInstructions.Values.SetDelegate"/> method.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="mangoGroup">The mango group.</param>
        /// <param name="mangoAccount">The mango account.</param>
        /// <param name="owner">The mango account owner.</param>
        /// <param name="delegateAccount">The delegate account.</param>
        /// <returns>The <see cref="TransactionInstruction"/>.</returns>
        public static TransactionInstruction SetDelegate(PublicKey programIdKey, PublicKey mangoGroup,
            PublicKey mangoAccount, PublicKey owner, PublicKey delegateAccount)
        {

            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(mangoGroup, false),
                AccountMeta.Writable(mangoAccount, false),
                AccountMeta.ReadOnly(owner, true),
                AccountMeta.ReadOnly(delegateAccount, false)
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Keys = keys,
                Data = MangoProgramData.EncodeSetDelegateData()
            };
        }

        /// <summary>
        /// Derives the <see cref="PublicKey"/> of an <see cref="AdvancedOrdersAccount"/>.
        /// </summary>
        /// <param name="mangoAccount">The mango account.</param>
        /// <returns>The derived <see cref="PublicKey"/> if it was found, otherwise null.</returns>
        public PublicKey DeriveAdvancedOrdersAccountAddress(PublicKey mangoAccount)
        {
            if (ProgramIdKey == MainNetProgramIdKeyV3)
            {
                return MangoUtils.DeriveAdvancedOrdersAccountAddress(ProgramIdKey, mangoAccount);
            }
            else if (ProgramIdKey == DevNetProgramIdKeyV3)
            {
                return MangoUtils.DeriveAdvancedOrdersAccountAddress(ProgramIdKey, mangoAccount);
            }

            return MangoUtils.DeriveAdvancedOrdersAccountAddress(ProgramIdKey, mangoAccount);
        }

        /// <summary>
        /// Derives the <see cref="PublicKey"/> of a <see cref="MangoAccount"/> of <see cref="MetaData.Version"/> 1.
        /// </summary>
        /// <param name="owner">The owner of the mango account.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The derived <see cref="PublicKey"/> if it was found, otherwise null.</returns>
        public PublicKey DeriveMangoAccountAddress(PublicKey owner, ulong accountNumber)
        {
            if(ProgramIdKey == MainNetProgramIdKeyV3)
            {
                return MangoUtils.DeriveMangoAccountAddress(ProgramIdKey, Constants.MangoGroup, owner, accountNumber);
            } else if(ProgramIdKey == DevNetProgramIdKeyV3)
            {
                return MangoUtils.DeriveMangoAccountAddress(ProgramIdKey, Constants.DevNetMangoGroup, owner, accountNumber);
            }

            return MangoUtils.DeriveMangoAccountAddress(ProgramIdKey, Constants.MangoGroup, owner, accountNumber);
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
                PublicKey = DevNetProgramIdKeyV3,
                InstructionName = MangoProgramInstructions.Names[instructionValue],
                ProgramName = DefaultProgramName,
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
                case MangoProgramInstructions.Values.PlaceSpotOrder2:
                    MangoProgramData.DecodePlaceSpotOrder2Data(decodedInstruction, data, keys, keyIndices);
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
                case MangoProgramInstructions.Values.SettleFees:
                    MangoProgramData.DecodeSettleFeesData(decodedInstruction, keys, keyIndices);
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
                case MangoProgramInstructions.Values.InitAdvancedOrders:
                    MangoProgramData.DecodeInitAdvancedOrdersData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.AddPerpTriggerOrder:
                    MangoProgramData.DecodeAddPerpTriggerOrderData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.RemoveAdvancedOrder:
                    MangoProgramData.DecodeRemoveAdvancedOrderData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CloseAdvancedOrders:
                    MangoProgramData.DecodeCloseAdvancedOrdersData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CloseMangoAccount:
                    MangoProgramData.DecodeCloseMangoAccountData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CloseSpotOpenOrders:
                    MangoProgramData.DecodeCloseSpotOpenOrdersData(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CreateMangoAccount:
                    MangoProgramData.DecodeCreateMangoAccountData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.UpgradeMangoAccountV0V1:
                    MangoProgramData.DecodeUpgradeMangoAccountV0V1Data(decodedInstruction, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.CancelPerpOrdersSide:
                    MangoProgramData.DecodeCancelPerpOrdersSideData(decodedInstruction, data, keys, keyIndices);
                    break;
                case MangoProgramInstructions.Values.SetDelegate:
                    MangoProgramData.DecodeSetDelegateData(decodedInstruction, keys, keyIndices);
                    break;
            }

            return decodedInstruction;
        }
    }
}