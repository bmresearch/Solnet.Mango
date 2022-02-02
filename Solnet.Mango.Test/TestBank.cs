using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Models.Banks;
using System.Collections.Generic;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class TestBank : TestBase
    {
        [TestMethod]
        public void TestOneDepositRootBank()
        {
            var prefix = "Resources/OneDeposit/";

            var rootBank = LoadRootBank(prefix + "RootBank0.txt").Result;

            Assert.AreEqual(0.6999999999999992894572642399m, rootBank.OptimalUtilization.ToDecimal());
            Assert.AreEqual(0.0599999999999987210230756318m, rootBank.OptimalRate.ToDecimal());
            Assert.AreEqual(1.5m, rootBank.MaxRate.ToDecimal());
            Assert.AreEqual(0.0599999999999987210230756318m, rootBank.OptimalRate.ToDecimal());
            Assert.AreEqual(1UL, rootBank.NumNodeBanks);
            Assert.AreEqual(1000154.4227660735583071982546m, rootBank.DepositIndex.ToDecimal());
            Assert.AreEqual(1000219.0086786301008849875416m, rootBank.BorrowIndex.ToDecimal());
            Assert.AreEqual(1633359485UL, rootBank.LastUpdated);
        }

        [TestMethod]
        public void TestBTCRootBankInterestRates()
        {
            var prefix = "Resources/TokenBank/";

            var rootBank = LoadRootBank(prefix + "BTCRootBank.txt").Result;
            var nodeBank = LoadNodeBank(prefix + "BTCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<NodeBank> { nodeBank };

            Assert.AreEqual(0.0060962691428017024009022862m, rootBank.GetBorrowRate(6).ToDecimal());
            Assert.AreEqual(0.0007432899492272326824604534m, rootBank.GetDepositRate(6).ToDecimal());
        }

        [TestMethod]
        public void TestUSDCRootBankInterestRates()
        {
            var prefix = "Resources/TokenBank/";

            var rootBank = LoadRootBank(prefix + "USDCRootBank.txt").Result;
            var nodeBank = LoadNodeBank(prefix + "USDCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<NodeBank> { nodeBank };

            Assert.AreEqual(0.2305834989565909154407563619m, rootBank.GetBorrowRate(6).ToDecimal());
            Assert.AreEqual(0.1687440978769068067322223214m, rootBank.GetDepositRate(6).ToDecimal());
        }
    }
}
