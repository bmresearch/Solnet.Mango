using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            Assert.AreEqual(0, mangoAccount.GetHealth(mangoGroup, mangoCache, Models.HealthType.Initialization));
            Assert.AreEqual(0, mangoAccount.GetHealth(mangoGroup, mangoCache, Models.HealthType.Maintenance));
            Assert.AreEqual(100, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, Models.HealthType.Initialization));
            Assert.AreEqual(100, mangoAccount.GetHealthRatio(mangoGroup, mangoCache, Models.HealthType.Maintenance));
            Assert.AreEqual(0, mangoAccount.GetEquity(mangoGroup, mangoCache));
            Assert.AreEqual(false, mangoAccount.IsLiquidatable(mangoGroup, mangoCache));
        }
    }
}
