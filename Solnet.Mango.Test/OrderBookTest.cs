using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Models.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class OrderBookTest : TestBase
    {
        [TestMethod]
        public void GetBidsEmpty()
        {
            var ob = new OrderBook();

            Assert.AreEqual(0, ob.GetBids().Count);
        }

        [TestMethod]
        public void GetAsksEmpty()
        {
            var ob = new OrderBook();

            Assert.AreEqual(0, ob.GetAsks().Count);
        }

        [TestMethod]
        public void GetBest()
        {
            var bids = LoadOrderBookSide("Resources/MangoClient/LoadBidsOrderBookSide.txt").Result;
            var asks = LoadOrderBookSide("Resources/MangoClient/LoadAsksOrderBookSide.txt").Result;

            var bestBid = bids.GetBest();
            var bestAsk = asks.GetBest();

            Assert.AreEqual(1518L, bestBid.RawPrice);
            Assert.AreEqual(15684182L, bestBid.RawQuantity);
            Assert.AreEqual(1519L, bestAsk.RawPrice);
            Assert.AreEqual(10000L, bestAsk.RawQuantity);

            // just assert it doesn't have to get the order's list into memory again
            var sameOld = bids.GetBest();

            Assert.AreEqual(bestBid, sameOld);
        }

        [TestMethod]
        public void GetImpactPrice()
        {
            var bids = LoadOrderBookSide("Resources/MangoClient/LoadBidsOrderBookSide.txt").Result;
            var asks = LoadOrderBookSide("Resources/MangoClient/LoadAsksOrderBookSide.txt").Result;

            var bidPrice = bids.GetImpactPrice(65875000L);
            var askPrice = asks.GetImpactPrice(20000L);

            Assert.AreEqual(1499L, bidPrice);
            Assert.AreEqual(1819L, askPrice);

            var zero = bids.GetImpactPrice(1_000_000_000L);

            Assert.AreEqual(0, zero);
        }
    }
}
