using Microsoft.Extensions.Logging;
using Solnet.Mango.Types;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Mango.Models
{
    /// <summary>
    /// Represents a root bank for a token's lending and borrowing info.
    /// </summary>
    public class RootBank
    {
        /// <summary>
        /// The layout of the <see cref="RootBank"/>.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The length of the <see cref="RootBank"/> structure.
            /// </summary>
            internal const int Length = 424;

            /// <summary>
            /// 
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            internal const int OptimalUtilizationOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            internal const int OptimalRateOffset = 24;

            /// <summary>
            /// 
            /// </summary>
            internal const int MaxRateOffset = 40;

            /// <summary>
            /// 
            /// </summary>
            internal const int NumNodeBanksOffset = 56;

            /// <summary>
            /// 
            /// </summary>
            internal const int NodeBanksOffset = 64;

            /// <summary>
            /// 
            /// </summary>
            internal const int DepositIndexOffset = 320;

            /// <summary>
            /// 
            /// </summary>
            internal const int BorrowIndexOffset = 336;

            /// <summary>
            /// 
            /// </summary>
            internal const int LastUpdatedOffset = 352;
        }

        /// <summary>
        /// The account metadata.
        /// </summary>
        public MetaData Metadata;

        /// <summary>
        /// The optimal utilization for this asset.
        /// </summary>
        public I80F48 OptimalUtilization;

        /// <summary>
        /// The optimal rate for this asset.
        /// </summary>
        public I80F48 OptimalRate;

        /// <summary>
        /// The maximum rate for this asset.
        /// </summary>
        public I80F48 MaxRate;

        /// <summary>
        /// The number of node banks this root bank has.
        /// </summary>
        public ulong NumNodeBanks;

        /// <summary>
        /// The node bank public keys.
        /// </summary>
        public List<PublicKey> NodeBanks;

        /// <summary>
        /// The deposit index.
        /// </summary>
        public I80F48 DepositIndex;

        /// <summary>
        /// The borrow index.
        /// </summary>
        public I80F48 BorrowIndex;

        /// <summary>
        /// Timestamp of the last update of the root bank.
        /// </summary>
        public ulong LastUpdated;

        /// <summary>
        /// The loaded node banks.
        /// </summary>
        public List<NodeBank> NodeBankAccounts;

        /// <summary>
        /// Loads the node banks for this root bank. This is an asynchronous operation.
        /// </summary>
        /// <param name="rpcClient">A rpc client instance.</param>
        /// <param name="logger">A logger instance.</param>
        public async Task<RequestResult<ResponseValue<List<AccountInfo>>>> LoadNodeBanksAsync(IRpcClient rpcClient,
            ILogger logger = null)
        {
            IEnumerable<PublicKey> filteredNodes = NodeBanks.Where(x => !x.Equals(SystemProgram.ProgramIdKey));
            RequestResult<ResponseValue<List<AccountInfo>>> nodeBankAccounts =
                await rpcClient.GetMultipleAccountsAsync(filteredNodes.Select(x => x.Key).ToList());
            if (!nodeBankAccounts.WasRequestSuccessfullyHandled)
            {
                logger?.LogInformation("Could not fetch node banks.");
                return nodeBankAccounts;
            }
            logger?.LogInformation($"Successfully fetched {nodeBankAccounts.Result.Value.Count} node banks.");
            foreach (AccountInfo account in nodeBankAccounts.Result.Value)
            {
                NodeBankAccounts.Add(NodeBank.Deserialize(Convert.FromBase64String(account.Data[0])));
            }
            return nodeBankAccounts;
        }

        /// <summary>
        /// Loads the node banks for this root bank.
        /// </summary>
        /// <param name="rpcClient">A rpc client instance.</param>
        /// <param name="logger">A logger instance.</param>
        public RequestResult<ResponseValue<List<AccountInfo>>> LoadNodeBanks(IRpcClient rpcClient,
            ILogger logger = null) => LoadNodeBanksAsync(rpcClient, logger).Result;

        /// <summary>
        /// Gets the index of the given node bank key.
        /// </summary>
        /// <param name="nodeBankKey">The node bank key.</param>
        /// <returns>The index.</returns>
        public int GetNodeBankIndex(PublicKey nodeBankKey)
        {
            for (int i = 0; i < NodeBanks.Count; i++)
            {
                if (NodeBanks[i].Equals(nodeBankKey)) return i;
            }

            throw new Exception("This Node Bank does not belong to this RootBank");
        }
        
        /// <summary>
        /// Gets the total amount of deposits.
        /// </summary>
        /// <returns>The total amount of deposits.</returns>
        public double GetNativeTotalDeposit()
        {
            return NodeBankAccounts.Where(x => x != null).Sum(nodeBank => nodeBank.Deposits.Value) * DepositIndex.Value;
        }

        /// <summary>
        /// Gets the total amount of borrows.
        /// </summary>
        /// <returns>The total amount of borrows.</returns>
        public double GetNativeTotalBorrows()
        {
            return NodeBankAccounts.Where(x => x != null).Sum(nodeBank => nodeBank.Borrows.Value) * BorrowIndex.Value;
        }

        /// <summary>
        /// Gets the total amount of deposits humanized.
        /// </summary>
        /// <param name="decimals">The token's decimals.</param>
        /// <returns>The total amount of deposits.</returns>
        public double GetUiTotalDeposit(byte decimals)
            => MangoUtils.HumanizeNative(GetNativeTotalDeposit(), decimals);

        /// <summary>
        /// Gets the total amount of borrows humanized.
        /// </summary>
        /// <param name="decimals">The token's decimals.</param>
        /// <returns>The total amount of borrows.</returns>
        public double GetUiTotalBorrow(byte decimals)
            => MangoUtils.HumanizeNative(GetNativeTotalBorrows(), decimals);

        /// <summary>
        /// Gets the borrow rate for this asset.
        /// </summary>
        /// <returns>The borrow rate.</returns>
        public double GetBorrowRate(byte decimals)
        {
            double totalDeposits = GetUiTotalDeposit(decimals);
            double totalBorrows = GetUiTotalBorrow(decimals);

            switch (totalDeposits)
            {
                case 0 when totalBorrows == 0:
                    return 0;
                case 0:
                    return double.MaxValue;
            }

            double utilization = (totalBorrows / totalDeposits);
            if (utilization > OptimalUtilization.Value)
            {
                double extraUtil = utilization - OptimalUtilization.Value;
                double slope = (MaxRate.Value - OptimalRate.Value) / ( 1 - OptimalUtilization.Value);
                return OptimalRate.Value + (slope * extraUtil);
            }
            else
            {
                double slope = OptimalRate.Value / OptimalUtilization.Value;
                return slope * utilization;
            }
        }

        /// <summary>
        /// Gets the deposit rate for this asset.
        /// </summary>
        /// <returns>The deposit rate.</returns>
        public double GetDepositRate(byte decimals)
        {
            double borrowRate = GetBorrowRate(decimals);
            double totalDeposits = GetUiTotalDeposit(decimals);
            double totalBorrows = GetUiTotalBorrow(decimals);
            return totalDeposits switch
            {
                0 when totalBorrows == 0 => 0,
                0 => double.MaxValue,
                _ => (totalBorrows / totalDeposits) * borrowRate
            };
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="RootBank"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBank"/> structure.</returns>
        public static RootBank Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length) throw new ArgumentException("data length is invalid");
            ReadOnlySpan<byte> span = data.AsSpan();
            List<PublicKey> nodeBanks = new(Constants.MaxNodeBanks);
            ReadOnlySpan<byte> nodeBanksBytes =
                span.Slice(Layout.NodeBanksOffset, Constants.MaxNodeBanks * PublicKey.PublicKeyLength);

            for (int i = 0; i < Constants.MaxNodeBanks - 1; i++)
            {
                nodeBanks.Add(nodeBanksBytes.GetPubKey(i * PublicKey.PublicKeyLength));
            }

            return new RootBank
            {
                Metadata = MetaData.Deserialize(span.Slice(Layout.MetadataOffset, MetaData.Layout.Length)),
                OptimalUtilization = I80F48.Deserialize(span.Slice(Layout.OptimalUtilizationOffset, I80F48.Length)),
                OptimalRate = I80F48.Deserialize(span.Slice(Layout.OptimalRateOffset, I80F48.Length)),
                MaxRate = I80F48.Deserialize(span.Slice(Layout.MaxRateOffset, I80F48.Length)),
                NumNodeBanks = span.GetU64(Layout.NumNodeBanksOffset),
                NodeBanks = nodeBanks,
                DepositIndex = I80F48.Deserialize(span.Slice(Layout.DepositIndexOffset, I80F48.Length)),
                BorrowIndex = I80F48.Deserialize(span.Slice(Layout.BorrowIndexOffset, I80F48.Length)),
                LastUpdated = span.GetU64(Layout.LastUpdatedOffset),
                NodeBankAccounts = new List<NodeBank>()
            };
        }
    }
}