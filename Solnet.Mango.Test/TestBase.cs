using Solnet.Mango.Models;
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
    }
}
