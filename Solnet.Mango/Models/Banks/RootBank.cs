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

namespace Solnet.Mango.Models.Banks
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
            /// The offset at which the metadata begins.
            /// </summary>
            internal const int MetadataOffset = 0;

            /// <summary>
            /// The offset at which the optimal utilization begins.
            /// </summary>
            internal const int OptimalUtilizationOffset = 8;

            /// <summary>
            /// The offset at which the optimal rate begins.
            /// </summary>
            internal const int OptimalRateOffset = 24;

            /// <summary>
            /// The offset at which the maximum rate begins.
            /// </summary>
            internal const int MaxRateOffset = 40;

            /// <summary>
            /// The offset at which the number of node banks begins.
            /// </summary>
            internal const int NumNodeBanksOffset = 56;

            /// <summary>
            /// The offset at which the node banks begin.
            /// </summary>
            internal const int NodeBanksOffset = 64;

            /// <summary>
            /// The offset at which the deposit index begins.
            /// </summary>
            internal const int DepositIndexOffset = 320;

            /// <summary>
            /// The offset at which the borrow index begins.
            /// </summary>
            internal const int BorrowIndexOffset = 336;

            /// <summary>
            /// The offset at which the last update timestamp begins.
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
                logger?.LogInformation($"Could not fetch node bank accounts.");
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
        public I80F48 GetNativeTotalDeposit()
        {
            var nodeBanks = NodeBankAccounts.Where(x => x != null);
            var sum = I80F48.Zero;
            foreach(var nb in nodeBanks)
            {
                sum += nb.Deposits;
            }
            return sum * DepositIndex;
        }

        /// <summary>
        /// Gets the total amount of borrows.
        /// </summary>
        /// <returns>The total amount of borrows.</returns>
        public I80F48 GetNativeTotalBorrows()
        {
            var nodeBanks = NodeBankAccounts.Where(x => x != null);
            var sum = I80F48.Zero;
            foreach (var nb in nodeBanks)
            {
                sum += nb.Borrows;
            }
            return sum * BorrowIndex;
        }

        /// <summary>
        /// Gets the total amount of deposits humanized.
        /// </summary>
        /// <param name="decimals">The token's decimals.</param>
        /// <returns>The total amount of deposits.</returns>
        public I80F48 GetUiTotalDeposit(byte decimals)
            => MangoUtils.HumanizeNative(GetNativeTotalDeposit(), decimals);

        /// <summary>
        /// Gets the total amount of borrows humanized.
        /// </summary>
        /// <param name="decimals">The token's decimals.</param>
        /// <returns>The total amount of borrows.</returns>
        public I80F48 GetUiTotalBorrow(byte decimals)
            => MangoUtils.HumanizeNative(GetNativeTotalBorrows(), decimals);

        /// <summary>
        /// Gets the borrow rate for this asset.
        /// </summary>
        /// <returns>The borrow rate.</returns>
        public I80F48 GetBorrowRate(byte decimals)
        {
            I80F48 totalDeposits = GetUiTotalDeposit(decimals);
            I80F48 totalBorrows = GetUiTotalBorrow(decimals);

            I80F48 utilization = (totalBorrows / totalDeposits);

            if (utilization > OptimalUtilization)
            {
                I80F48 extraUtil = utilization - OptimalUtilization;
                I80F48 slope = (MaxRate - OptimalRate) / (I80F48.One - OptimalUtilization);
                return OptimalRate + (slope * extraUtil);
            }
            else
            {
                I80F48 slope = OptimalRate / OptimalUtilization;
                return slope * utilization;
            }
        }

        /// <summary>
        /// Gets the deposit rate for this asset.
        /// </summary>
        /// <returns>The deposit rate.</returns>
        public I80F48 GetDepositRate(byte decimals)
        {
            I80F48 borrowRate = GetBorrowRate(decimals);
            I80F48 totalDeposits = GetUiTotalDeposit(decimals);
            I80F48 totalBorrows = GetUiTotalBorrow(decimals);

            if(totalDeposits == I80F48.Zero)
            {
                if (totalBorrows == I80F48.Zero) return I80F48.Zero;
                return I80F48.MaxValue;
            } else
            {
                return (totalBorrows / totalDeposits) * borrowRate;
            }
        }

        /// <summary>
        /// Deserialize a span of bytes into a <see cref="RootBank"/> instance.
        /// </summary>
        /// <param name="data">The data to deserialize into the structure.</param>
        /// <returns>The <see cref="RootBank"/> structure.</returns>
        public static RootBank Deserialize(byte[] data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"data length is invalid, expected {Layout.Length} but got {data.Length}");
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