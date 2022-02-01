using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Models;
using Solnet.Serum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class TestHealth : TestBase
    {
        [TestMethod]
        public void TestEmptyHealth()
        {
            var prefix = "Resources/Empty/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;
            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(0m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(0m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(100m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(100m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(0m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(0m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }

        [TestMethod]
        public void TestOneDepositHealth()
        {
            var prefix = "Resources/OneDeposit/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;
            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(37904260000.05905822642118252475m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(42642292500.06652466908819931746m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(100m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(100m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(47380.325000073896308805387889m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(0m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }

        [TestMethod]
        public void TestAccountOneHealth()
        {
            var prefix = "Resources/AccountOne/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;

            var openOrdersAccounts = new List<OpenOrdersAccount>();
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                openOrdersAccounts.Add(null);
            }
            openOrdersAccounts[3] = LoadOpenOrdersAccount(prefix + "OpenOrders3.txt").Result;
            openOrdersAccounts[6] = LoadOpenOrdersAccount(prefix + "OpenOrders6.txt").Result;
            openOrdersAccounts[7] = LoadOpenOrdersAccount(prefix + "OpenOrders7.txt").Result;
            mangoAccount.OpenOrdersAccounts = openOrdersAccounts;

            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(454884281.15520619643754685058m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(901472688.63722587052636470162m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(10.488604676089252620840852614m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(20.785925232226531988999340683m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(1374.8238319241928202529834380m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(3.1545408891717698907086742111m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }

        [TestMethod]
        public void TestAccountTwoHealth()
        {
            var prefix = "Resources/AccountTwo/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;

            var openOrdersAccounts = new List<OpenOrdersAccount>();
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                openOrdersAccounts.Add(null);
            }
            openOrdersAccounts[2] = LoadOpenOrdersAccount(prefix + "OpenOrders2.txt").Result;
            openOrdersAccounts[3] = LoadOpenOrdersAccount(prefix + "OpenOrders3.txt").Result;
            mangoAccount.OpenOrdersAccounts = openOrdersAccounts;

            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(7516159604.9885432195310457359m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(9618709877.520870773008571319m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(24.806800043657162291310669389m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(31.746187568175088244970538653m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(11915.529421941821698993635437m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(3.5053179904544187195369886467m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }

        [TestMethod]
        public void TestAccountThreeHealth()
        {
            var prefix = "Resources/AccountThree/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;

            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(341025333625.51856223547208202m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(683477170424.20340250929429260m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(4.526520188456473192673001904m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(9.503973530764042720875295345m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(1025929.0072228639101155067692m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(6.5015747278792268559755029855m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }

        [TestMethod]
        public void TestAccountFourHealth()
        {
            var prefix = "Resources/AccountFour/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;


            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(-848086876487.0495042743630023m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(-433869053006.07361789143756425m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(-9.306553530875660840138152707m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(-4.9878179847269166202750056982m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(-19651.229525127163459075063656m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(-421.56937096615545002009639575m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(true, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }

        [TestMethod]
        public void TestAccountFiveHealth()
        {
            var prefix = "Resources/AccountFive/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;

            var openOrdersAccounts = new List<OpenOrdersAccount>();
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                openOrdersAccounts.Add(null);
            }
            openOrdersAccounts[0] = LoadOpenOrdersAccount(prefix + "OpenOrders0.txt").Result;
            openOrdersAccounts[1] = LoadOpenOrdersAccount(prefix + "OpenOrders1.txt").Result;
            openOrdersAccounts[2] = LoadOpenOrdersAccount(prefix + "OpenOrders2.txt").Result;
            openOrdersAccounts[3] = LoadOpenOrdersAccount(prefix + "OpenOrders3.txt").Result;
            openOrdersAccounts[8] = LoadOpenOrdersAccount(prefix + "OpenOrders8.txt").Result;
            mangoAccount.OpenOrdersAccounts = openOrdersAccounts;

            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(15144959918141.091751351958553m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(15361719060997.682760216140338m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(878.8891307782332518172552227m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(946.4449882088800336532585789m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(15578523.351110761827218453845m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(0.0988404770787312259017198812m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }

        [TestMethod]
        public void TestAccountSixHealth()
        {
            var prefix = "Resources/AccountSix/";

            var mangoAccount = LoadMangoAccount(prefix + "MangoAccount.txt").Result;
            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;
            var mangoGroup = LoadMangoGroup(prefix + "MangoGroup.txt").Result;

            var openOrdersAccounts = new List<OpenOrdersAccount>();
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                openOrdersAccounts.Add(null);
            }
            openOrdersAccounts[0] = LoadOpenOrdersAccount(prefix + "OpenOrders0.txt").Result;
            openOrdersAccounts[1] = LoadOpenOrdersAccount(prefix + "OpenOrders1.txt").Result;
            openOrdersAccounts[2] = LoadOpenOrdersAccount(prefix + "OpenOrders2.txt").Result;
            openOrdersAccounts[3] = LoadOpenOrdersAccount(prefix + "OpenOrders3.txt").Result;
            openOrdersAccounts[8] = LoadOpenOrdersAccount(prefix + "OpenOrders8.txt").Result;
            mangoAccount.OpenOrdersAccounts = openOrdersAccounts;

            LogAccountStatus(mangoGroup, mangoCache, mangoAccount);

            Assert.AreEqual(14480970069238.336864874501611m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(15030566251990.170260826183338m, mangoAccount.GetHealth(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(215.03167137712999590348772472m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Initialization).ToDecimal());
            Assert.AreEqual(236.77769605824430243501410587m, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, HealthType.Maintenance).ToDecimal());
            Assert.AreEqual(15580383.036820138153533576997m, mangoAccount.ComputeValue(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(0.0791375876026165769872022793m, mangoAccount.GetLeverage(mangoGroup, mangoCache).ToDecimal());
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }
    }
}
