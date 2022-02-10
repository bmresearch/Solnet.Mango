using Solnet.Mango.Models;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public static class ExampleHelpers
    {
        public static void LogAccountStatus(IMangoClient mangoClient, MangoGroup mangoGroup, MangoCache mangoCache, MangoAccount mangoAccount,
            List<I80F48> breakEvenPrices = null)
        {
            if (mangoGroup.RootBankAccounts.Count != 0)
            {
                Console.WriteLine($"- - - - - - - - - - ACCOUNT DEPOSITS - - - - - - - - - -");
                for (int token = 0; token < mangoGroup.Tokens.Count; token++)
                {
                    if (mangoGroup.Tokens[token].RootBank.Key == SystemProgram.ProgramIdKey.Key) continue;
                    Console.WriteLine(
                        $"Token: {mangoGroup.Tokens[token].Mint,-50}" +
                        $"Deposits: {mangoAccount.GetUiDeposit(mangoGroup.RootBankAccounts[token], mangoGroup, token).ToDecimal(),-25:N4}" +
                        $"Borrows: {mangoAccount.GetUiBorrow(mangoGroup.RootBankAccounts[token], mangoGroup, token).ToDecimal(),-25:N4}" +
                        $"MaxWithBorrow: {mangoAccount.GetMaxWithBorrowForToken(mangoGroup, mangoCache, token).ToDecimal(),-25:N4}" +
                        $"Net: {mangoAccount.GetUiNet(mangoCache.RootBankCaches[token], mangoGroup, token).ToDecimal(),-25:N4}");
                }
            }

               Console.WriteLine($"- - - - - - ACCOUNT PERP POSITIONS - - - - - -");
            for (int p = 0; p < mangoGroup.PerpetualMarkets.Count; p++)
            {
                if (mangoGroup.PerpetualMarkets[p].Market.Equals(SystemProgram.ProgramIdKey)) continue;
                var perpMarket = mangoClient.GetPerpMarket(mangoGroup.PerpetualMarkets[p].Market);
                var notional = mangoAccount.PerpetualAccounts[p].GetNotionalSize(mangoGroup, mangoCache, perpMarket.ParsedResult, p);
                var absNotional = notional < 0 ? notional * -1 : notional;
                var msg = //$"Market: {mangoGroup.PerpetualMarkets[p].Market,-49}" +
                    $"Position Size: {mangoAccount.PerpetualAccounts[p].GetUiBasePosition(perpMarket.ParsedResult, mangoGroup.Tokens[p].Decimals),-25:N4}" +
                    $"Notional Size: ${absNotional,-25:N4}";

                msg += breakEvenPrices != null ?
                    $"PNL: {mangoAccount.PerpetualAccounts[p].GetProfitAndLoss(mangoGroup, mangoCache, perpMarket.ParsedResult, breakEvenPrices[p], p),-25:N4}" : "";

                Console.WriteLine(msg);

            }

            Console.WriteLine($"- - - - - - - - - - ACCOUNT STATUS - - - - - - - - - -");
            Console.WriteLine(
                $"Account Value: ${mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal():N4}\n" +
                $"Account Maintenance Health: {mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N4}\n" +
                $"Account Initialization Health: {mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N4}\n" +
                $"Account Maintenance Health Ratio: {mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N4}\n" +
                $"Account Initialization Health Ratio: {mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N4}\n" +
                $"Leverage: x{mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal():N4}\n" +
                $"Assets Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache).ToDecimal():N4}\n" +
                $"Liabilities Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache).ToDecimal():N4}\n" +
                $"Assets Maintenance Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N4}\n" +
                $"Liabilities Maintenance Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N4}\n" +
                $"Assets Initialization Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N4}\n" +
                $"Liabilities Initialization Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N4}\n");
        }
        public static async Task<TransactionMetaSlotInfo> RequestAirdrop(IRpcClient rpcClient, PublicKey dest, ulong amount)
        {
            var airdropTxSig = rpcClient.RequestAirdrop(dest, amount).Result;

            return await PollTx(rpcClient, airdropTxSig, Commitment.Confirmed);
        }

        public static void DecodeAndLogMessage(byte[] msg)
        {
            Console.WriteLine("Message Data: " + Convert.ToBase64String(msg));

            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(Message.Deserialize(msg));

            string aggregate = ix.Aggregate(
                "Decoded Instructions:",
                (s, instruction) =>
                {
                    s += $"\n\tProgram: {instruction.ProgramName}\n\t\t\t Instruction: {instruction.InstructionName}\n";
                    return instruction.Values.Aggregate(
                        s,
                        (current, entry) =>
                            current +
                            $"\t\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                });
            Console.WriteLine(aggregate);
        }

        public static string PrettyPrintTransactionSimulationLogs(string[] logMessages)
        {
            return logMessages.Aggregate("", (current, log) => current + $"\t\t{log}\n");
        }

        /// <summary>
        /// Submits a transaction and logs the output from SimulateTransaction.
        /// </summary>
        /// <param name="tx">The transaction data ready to simulate or submit to the network.</param>
        public static string SubmitTxSendAndLog(IRpcClient rpcClient, byte[] tx)
        {
            Console.WriteLine($"Tx Data: {Convert.ToBase64String(tx)}");

            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx, commitment: Commitment.Confirmed);
            string logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            RequestResult<string> txReq = rpcClient.SendTransaction(tx, commitment: Commitment.Confirmed);
            Console.WriteLine($"Tx Signature: {txReq.Result}");

            return txReq.Result;
        }

        /// <summary>
        /// Polls the rpc client until a transaction signature has been confirmed.
        /// </summary>
        /// <param name="signature">The first transaction signature.</param>
        public static async Task<TransactionMetaSlotInfo> PollTx(IRpcClient rpcClient, string signature, Commitment commitment)
        {
            if (signature == null) return null;
            RequestResult<TransactionMetaSlotInfo> txMeta = await rpcClient.GetTransactionAsync(signature, commitment);
            while (!txMeta.WasSuccessful)
            {
                Thread.Sleep(2500);
                txMeta = await rpcClient.GetTransactionAsync(signature, commitment);
            }
            return txMeta.Result;
        }
    }
}