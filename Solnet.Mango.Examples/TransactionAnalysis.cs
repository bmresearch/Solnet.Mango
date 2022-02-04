using Solnet.Programs;
using Solnet.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Mango.Examples
{
    public class TransactionAnalysis : IRunnableExample
    {
        private static readonly IRpcClient RpcClient = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);

        public TransactionAnalysis() { }

        public void Run()
        {
            string txHash = string.Empty;
            while (true)
            {
                Console.WriteLine($"Paste transaction hash:");
                txHash = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(txHash)) break;

                Console.WriteLine($"Handling transaction: {txHash}");
                var tx = RpcClient.GetTransaction(txHash);

                if (!tx.WasRequestSuccessfullyHandled)
                {
                    Console.WriteLine($"Could not find transaction.");
                    continue;
                }

                List<DecodedInstruction> ix =
                    InstructionDecoder.DecodeInstructions(tx.Result);

                string aggregate = ix.Aggregate(
                    "Decoded Instructions:",
                    (s, instruction) =>
                    {
                        s += $"\nProgram: {instruction.ProgramName}\n\t\t Instruction: {instruction.InstructionName}\n";
                        return instruction.Values.Aggregate(
                            s,
                            (current, entry) =>
                                current +
                                $"\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                    });
                Console.WriteLine(aggregate);
            }
        }
    }
}