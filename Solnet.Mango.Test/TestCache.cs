using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class TestCache : TestBase
    {
        [TestMethod]
        public void TestCachePrices()
        {
            var prefix = "Resources/OneDeposit/";

            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;

            Assert.AreEqual(0.3364249999999984197529556695m, mangoCache.PriceCaches[0].Price.ToDecimal());
            Assert.AreEqual(47380.324999999999999289457264m, mangoCache.PriceCaches[1].Price.ToDecimal());
            Assert.AreEqual(3309.6954999999999991189270077m, mangoCache.PriceCaches[2].Price.ToDecimal());
            Assert.AreEqual(0.1726159999999978822415869217m, mangoCache.PriceCaches[3].Price.ToDecimal());
            Assert.AreEqual(8.793799999999997396571416175m, mangoCache.PriceCaches[4].Price.ToDecimal());
            Assert.AreEqual(1m, mangoCache.PriceCaches[5].Price.ToDecimal());
            Assert.AreEqual(1.0003999999999990677679306827m, mangoCache.PriceCaches[6].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[7].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[8].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[9].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[10].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[11].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[12].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[13].Price.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PriceCaches[14].Price.ToDecimal());
        }

        [TestMethod]
        public void TestCacheRootBankCaches()
        {
            var prefix = "Resources/OneDeposit/";

            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;

            Assert.AreEqual(1001923.8646082172201481341745m, mangoCache.RootBankCaches[0].DepositIndex.ToDecimal());
            Assert.AreEqual(1002515.4525785533782418212922m, mangoCache.RootBankCaches[0].BorrowIndex.ToDecimal());
            Assert.AreEqual(1000007.3724965391444108320229m, mangoCache.RootBankCaches[1].DepositIndex.ToDecimal());
            Assert.AreEqual(1000166.9852215921399931630731m, mangoCache.RootBankCaches[1].BorrowIndex.ToDecimal());
            Assert.AreEqual(1000000.1955488687582942475274m, mangoCache.RootBankCaches[2].DepositIndex.ToDecimal());
            Assert.AreEqual(1000001.1327325356510762333073m, mangoCache.RootBankCaches[2].BorrowIndex.ToDecimal());
            Assert.AreEqual(1000037.8214992379907037900466m, mangoCache.RootBankCaches[3].DepositIndex.ToDecimal());
            Assert.AreEqual(1000044.2892524101096505262376m, mangoCache.RootBankCaches[3].BorrowIndex.ToDecimal());
            Assert.AreEqual(1000000.0000132182767842436988m, mangoCache.RootBankCaches[4].DepositIndex.ToDecimal());
            Assert.AreEqual(1000000.1423597393804136856943m, mangoCache.RootBankCaches[4].BorrowIndex.ToDecimal());
            Assert.AreEqual(1000000.3524438650694534658214m, mangoCache.RootBankCaches[5].DepositIndex.ToDecimal());
            Assert.AreEqual(1000000.6615614642099352238347m, mangoCache.RootBankCaches[5].BorrowIndex.ToDecimal());
            Assert.AreEqual(1000473.2516160899858057575784m, mangoCache.RootBankCaches[6].DepositIndex.ToDecimal());
            Assert.AreEqual(1000524.3727921770212887508933m, mangoCache.RootBankCaches[6].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[7].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[7].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[8].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[8].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[9].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[9].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[10].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[10].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[11].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[11].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[12].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[12].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[13].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[13].BorrowIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[14].DepositIndex.ToDecimal());
            Assert.AreEqual(0m, mangoCache.RootBankCaches[14].BorrowIndex.ToDecimal());
            Assert.AreEqual(1000154.4227660753405508842206m, mangoCache.RootBankCaches[15].DepositIndex.ToDecimal());
            Assert.AreEqual(1000219.0086874350906356312407m, mangoCache.RootBankCaches[15].BorrowIndex.ToDecimal());
        }

        [TestMethod]
        public void TestCachePerpMarketCaches()
        {
            var prefix = "Resources/OneDeposit/";

            var mangoCache = LoadMangoCache(prefix + "MangoCache.txt").Result;

            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[0].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[0].ShortFunding.ToDecimal());
            Assert.AreEqual(-751864.70031280454435673732405m, mangoCache.PerpetualMarketCaches[1].LongFunding.ToDecimal());
            Assert.AreEqual(-752275.35579797613825192570403m, mangoCache.PerpetualMarketCaches[1].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[2].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[2].ShortFunding.ToDecimal());
            Assert.AreEqual(-636425.51790158202868497028248m, mangoCache.PerpetualMarketCaches[3].LongFunding.ToDecimal());
            Assert.AreEqual(-636425.51790158202868497028248m, mangoCache.PerpetualMarketCaches[3].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[4].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[4].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[5].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[5].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[6].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[6].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[7].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[7].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[8].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[8].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[9].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[9].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[10].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[10].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[11].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[11].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[12].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[12].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[13].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[13].ShortFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[14].LongFunding.ToDecimal());
            Assert.AreEqual(0m, mangoCache.PerpetualMarketCaches[14].ShortFunding.ToDecimal());
        }
    }
}
