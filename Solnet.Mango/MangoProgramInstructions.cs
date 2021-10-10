using Solnet.Mango.Models;
using Solnet.Serum.Models;
using System.Collections.Generic;

namespace Solnet.Mango
{
    /// <summary>
    /// Represents the instruction types for the <see cref="MangoProgram"/> along with a friendly name so as not to use reflection.
    /// <remarks>
    /// For more information see:
    /// https://github.com/blockworks-foundation/mango-v3/
    /// </remarks>
    /// </summary>
    internal static class MangoProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="MangoProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.InitMangoAccount, "Initialize Mango Account" },
            { Values.Deposit, "Deposit" },
            { Values.Withdraw, "Withdraw" },
            { Values.PlaceSpotOrder, "Place Spot Order" },
            { Values.PlacePerpOrder, "Place Perp Order" },
            { Values.CancelPerpOrderByClientId, "Cancel Perp Order By Client Id" },
            { Values.CancelPerpOrder, "Cancel Perp Order" },
            { Values.ConsumeEvents, "Consume Events" },
            { Values.SettleFunds, "Settle Funds" },
            { Values.CancelSpotOrder, "Cancel Spot Order" },
            { Values.SettleProfitAndLoss, "Settle Profit & Loss" },
            { Values.SettleBorrow, "Settle Borrow" },
            { Values.SettleFees, "Settle Fees" },
            { Values.InitSpotOpenOrders, "Initialize Spot Open Orders Account" },
            { Values.RedeemMango, "Redeem Mango" },
            { Values.AddMangoAccountInfo, "Add Mango Account Info" },
            { Values.DepositMegaSerum, "Deposit MegaSerum" },
            { Values.WithdrawMegaSerum, "Withdraw MegaSerum" },
            { Values.CancelAllPerpOrders, "Cancel All Perp Orders" },
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="MangoProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            /// <summary>
            /// Initializes a mango account.
            /// </summary>
            InitMangoAccount = 1,

            /// <summary>
            /// Deposits funds into margin account to be used as collateral and earn interest.
            /// </summary>
            Deposit = 2,

            /// <summary>
            /// Withdraws funds that were deposited earlier.
            /// </summary>
            Withdraw = 3,

            /// <summary>
            /// Places an order on the Serum DEX using Mango's margin facilities.
            /// </summary>
            PlaceSpotOrder = 9,

            /// <summary>
            /// Places a perp order.
            /// </summary>
            PlacePerpOrder = 12,

            /// <summary>
            /// Cancels an order using Serum <see cref="Order"/>'s clientOrderId.
            /// </summary>
            CancelPerpOrderByClientId = 13,

            /// <summary>
            /// Cancels a perp order.
            /// </summary>
            CancelPerpOrder = 14,

            /// <summary>
            /// Consumes events on the Mango Perpetual <see cref="Models.EventQueue"/>.
            /// </summary>
            ConsumeEvents = 15,

            /// <summary>
            /// Settles all funds from Serum DEX open orders into <see cref="MangoAccount"/> positions.
            /// </summary>
            SettleFunds = 19,

            /// <summary>
            /// Cancels an order using Serum DEX instruction.
            /// </summary>
            CancelSpotOrder = 20,

            /// <summary>
            /// Take two MangoAccounts and settle profits and losses between them for a perp market.
            /// </summary>
            SettleProfitAndLoss = 22,

            /// <summary>
            /// Uses this token's position and deposits to reduce borrows.
            /// </summary>
            SettleBorrow = 23,

            /// <summary>
            /// Take an account that has losses in the selected perp market to account for fees accrued.
            /// </summary>
            SettleFees = 29,

            /// <summary>
            /// Initialize a spot <see cref="OpenOrdersAccount"/> on Serum.
            /// </summary>
            InitSpotOpenOrders = 32,

            /// <summary>
            /// Redeem the MNGO accrued in a PerpAccount for MNGO in MangoAccount deposits
            /// </summary>
            RedeemMango = 33,

            /// <summary>
            /// Adds info to a <see cref="MangoAccount"/>.
            /// </summary>
            AddMangoAccountInfo = 34,

            /// <summary>
            /// Deposits MSRM into the MSRM vault for <see cref="MangoGroup"/>.
            /// These MSRM are not at risk and are not counted towards collateral or any margin calculations.
            /// Depositing MSRM is a strictly altruistic act with no upside and no downside.
            /// </summary>
            DepositMegaSerum = 35,

            /// <summary>
            /// Withdraws MSRM from the MSRM vault for <see cref="MangoGroup"/>.
            /// These MSRM are not at risk and are not counted towards collateral or any margin calculations.
            /// Depositing MSRM is a strictly altruistic act with no upside and no downside.
            /// </summary>
            WithdrawMegaSerum = 36,

            /// <summary>
            /// Cancels all open perp orders.
            /// </summary>
            CancelAllPerpOrders = 39,
        };
    }
}