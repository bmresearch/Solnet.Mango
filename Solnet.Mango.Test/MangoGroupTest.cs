using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Models.Banks;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class MangoGroupTest : TestBase
    {
        [TestMethod]
        public void GetOracleIndex()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetOracleIndex(mangoGroup.Oracles[0]));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetOracleIndexException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetOracleIndex(new Account()));
        }

        [TestMethod]
        public void GetSpotMarketIndex()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetSpotMarketIndex(mangoGroup.SpotMarkets[0].Market));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetSpotMarketIndexException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetSpotMarketIndex(new Account()));
        }

        [TestMethod]
        public void GetPerpMarketIndex()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetPerpMarketIndex(mangoGroup.PerpetualMarkets[0].Market));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetPerpMarketIndexException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetPerpMarketIndex(new Account()));
        }

        [TestMethod]
        public void GetRootBankIndex()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetRootBankIndex(mangoGroup.Tokens[0].RootBank));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetRootBankIndexException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetRootBankIndex(new Account()));
        }

        [TestMethod]
        public void GetTokenIndex()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetTokenIndex(mangoGroup.Tokens[0].Mint));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetTokenIndexException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            Assert.AreEqual(0, mangoGroup.GetTokenIndex(new Account()));
        }

        [TestMethod]
        public void GetBorrowRate()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;
            var rootBank = LoadRootBank("Resources/TokenBank/USDCRootBank.txt").Result;
            var nodeBank = LoadNodeBank("Resources/TokenBank/USDCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<NodeBank> { nodeBank };

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }
            rba[Models.Constants.QuoteIndex] = rootBank;

            mangoGroup.RootBankAccounts = rba;

            var borrowRate = mangoGroup.GetBorrowRate(Models.Constants.QuoteIndex);

            Assert.AreEqual(0.2305834989565909154407563619m, borrowRate.ToDecimal());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetBorrowRateException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }

            mangoGroup.RootBankAccounts = rba;

            var borrowRate = mangoGroup.GetBorrowRate(Models.Constants.QuoteIndex);
        }

        [TestMethod]
        public void GetDepositRate()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;
            var rootBank = LoadRootBank("Resources/TokenBank/USDCRootBank.txt").Result;
            var nodeBank = LoadNodeBank("Resources/TokenBank/USDCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<NodeBank> { nodeBank };

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }
            rba[Models.Constants.QuoteIndex] = rootBank;

            mangoGroup.RootBankAccounts = rba;

            var depositRate = mangoGroup.GetDepositRate(Models.Constants.QuoteIndex);

            Assert.AreEqual(0.1687440978769068067322223214m, depositRate.ToDecimal());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetDepositRateException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }

            mangoGroup.RootBankAccounts = rba;

            var borrowRate = mangoGroup.GetDepositRate(Models.Constants.QuoteIndex);
        }

        [TestMethod]
        public void GetUiTotalDeposit()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;
            var rootBank = LoadRootBank("Resources/TokenBank/USDCRootBank.txt").Result;
            var nodeBank = LoadNodeBank("Resources/TokenBank/USDCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<NodeBank> { nodeBank };

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }
            rba[Models.Constants.QuoteIndex] = rootBank;

            mangoGroup.RootBankAccounts = rba;

            var totalDeposits = mangoGroup.GetUiTotalDeposit(Models.Constants.QuoteIndex);

            Assert.AreEqual(78859784.365391541334979308431m, totalDeposits.ToDecimal());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetUiTotalDepositException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }

            mangoGroup.RootBankAccounts = rba;

            var totalDeposits = mangoGroup.GetUiTotalDeposit(Models.Constants.QuoteIndex);
        }

        [TestMethod]
        public void GetUiTotalBorrow()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;
            var rootBank = LoadRootBank("Resources/TokenBank/USDCRootBank.txt").Result;
            var nodeBank = LoadNodeBank("Resources/TokenBank/USDCNodeBank.txt").Result;

            rootBank.NodeBankAccounts = new List<NodeBank> { nodeBank };

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }
            rba[Models.Constants.QuoteIndex] = rootBank;

            mangoGroup.RootBankAccounts = rba;

            var totalDeposits = mangoGroup.GetUiTotalBorrow(Models.Constants.QuoteIndex);

            Assert.AreEqual(57710648.124091633099396858597m, totalDeposits.ToDecimal());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetUiTotalBorrowException()
        {
            var mangoGroup = LoadMangoGroup("Resources/Empty/MangoGroup.txt").Result;

            var rba = new List<RootBank>();
            for (int i = 0; i < Models.Constants.MaxTokens; i++)
            {
                rba.Add(null);
            }

            mangoGroup.RootBankAccounts = rba;

            var totalDeposits = mangoGroup.GetUiTotalBorrow(Models.Constants.QuoteIndex);
        }
    }
}
