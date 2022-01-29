using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class TestBank : TestBase
    {
        [TestMethod]
        public void TestRootBank()
        {
            var prefix = "Resources/TokenBank/";

            //test 1 deposit data
        }

        [TestMethod]
        public void TestBTCRootBankInterestRates()
        {
            var prefix = "Resources/TokenBank/";

            var rootBank = LoadRootBank(prefix + "BTCRootBank.txt").Result;
            var nodeBank = LoadNodeBank(prefix + "BTCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<Models.NodeBank> { nodeBank };

            /*
             * https://github.com/blockworks-foundation/mango-client-v3/pull/50/files#diff-1d66cd90fc8bc7d5dc3ac7cd3d508660e2a9c554a48f39bc8e4ff9a03256519cR38
             * The expected values below were adjusted compared to the typescript test suite due to conversion / precision errors when parsing rust's I80F48 to a C# double
             */

            Assert.AreEqual(0.006096269142805435, rootBank.GetBorrowRate(6));
            Assert.AreEqual(0.0007432899492304304, rootBank.GetDepositRate(6));
        }

        [TestMethod]
        public void TestUSDCRootBankInterestRates()
        {
            var prefix = "Resources/TokenBank/";

            var rootBank = LoadRootBank(prefix + "USDCRootBank.txt").Result;
            var nodeBank = LoadNodeBank(prefix + "USDCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<Models.NodeBank> { nodeBank };

            /*
             * https://github.com/blockworks-foundation/mango-client-v3/pull/50/files#diff-1d66cd90fc8bc7d5dc3ac7cd3d508660e2a9c554a48f39bc8e4ff9a03256519cR56
             * The expected values below were adjusted compared to the typescript test suite due to conversion / precision errors when parsing rust's I80F48 to a C# double
             */

            Assert.AreEqual(0.23058349895659447, rootBank.GetBorrowRate(6));
            Assert.AreEqual(0.16874409787691286, rootBank.GetDepositRate(6));
        }
    }
}
