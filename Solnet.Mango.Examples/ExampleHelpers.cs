using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Mango.Examples
{
    public static class ExampleHelpers
    {
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

            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);
            string logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            RequestResult<string> txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");

            return txReq.Result;
        }

        /// <summary>
        /// Polls the rpc client until a transaction signature has been confirmed.
        /// </summary>
        /// <param name="signature">The first transaction signature.</param>
        public static async void PollConfirmedTx(IRpcClient rpcClient, string signature)
        {
            RequestResult<TransactionMetaSlotInfo> txMeta = await rpcClient.GetConfirmedTransactionAsync(signature);
            while (!txMeta.WasSuccessful)
            {
                Thread.Sleep(2500);
                txMeta = await rpcClient.GetConfirmedTransactionAsync(signature);
            }
        }


    }
}