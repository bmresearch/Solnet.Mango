using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Serum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    public abstract class TestBase
    {
        private async Task<byte[]> LoadData(string path)
        {
            var accountData = await File.ReadAllTextAsync(path);

            return Convert.FromBase64String(accountData);
        }

        protected async Task<MangoAccount> LoadMangoAccount(string path)
        {
            return MangoAccount.Deserialize(await LoadData(path));
        }

        protected async Task<MangoGroup> LoadMangoGroup(string path)
        {
            return MangoGroup.Deserialize(await LoadData(path));
        }

        protected async Task<MangoCache> LoadMangoCache(string path)
        {
            return MangoCache.Deserialize(await LoadData(path));
        }

        protected async Task<RootBank> LoadRootBank(string path)
        {
            return RootBank.Deserialize(await LoadData(path));
        }

        protected async Task<NodeBank> LoadNodeBank(string path)
        {
            return NodeBank.Deserialize(await LoadData(path));
        }
        protected async Task<OpenOrdersAccount> LoadOpenOrdersAccount(string path)
        {
            return OpenOrdersAccount.Deserialize(await LoadData(path));
        }

        protected void LogAccountStatus(MangoGroup mangoGroup, MangoCache mangoCache, MangoAccount mangoAccount)
        {
            if (mangoGroup.RootBankAccounts.Count != 0)
            {
                for (int token = 0; token < mangoGroup.Tokens.Count; token++)
                {
                    if (mangoGroup.Tokens[token].RootBank.Key == SystemProgram.ProgramIdKey.Key) continue;
                    System.Diagnostics.Debug.WriteLine(
                        $"Token: {mangoGroup.Tokens[token].Mint}\t" +
                        $"Deposits: {mangoAccount.GetUiDeposit(mangoGroup.RootBankAccounts[token], mangoGroup, token).ToDecimal():N6}\t" +
                        $"Borrows: {mangoAccount.GetUiBorrow(mangoGroup.RootBankAccounts[token], mangoGroup, token).ToDecimal():N6}\t" +
                        $"MaxWithBorrow: {mangoAccount.GetMaxWithBorrowForToken(mangoGroup, mangoCache, token).ToDecimal():N6}\t" +
                        $"Net: {mangoAccount.GetUiNet(mangoCache.RootBankCaches[token], mangoGroup, token).ToDecimal():N6}\t");
                }
            }

            System.Diagnostics.Debug.WriteLine(
                $"Account Value: {mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Account Equity UI: {mangoAccount.GetUiEquity(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Account Maintenance Health: {mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Account Initialization Health: {mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n" +
                $"Account Maintenance Health Ratio: {mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Account Initialization Health Ratio: {mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n" +
                $"Leverage: {mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Assets Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Liabilities Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache).ToDecimal():N6}\n" +
                $"Assets Maintenance Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Liabilities Maintenance Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal():N6}\n" +
                $"Assets Initialization Value: {mangoAccount.GetAssetsValue(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n" +
                $"Liabilities Initialization Value: {mangoAccount.GetLiabilitiesValue(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal():N6}\n");
        }
    }
}
