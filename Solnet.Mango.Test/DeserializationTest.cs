using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Models;
using Solnet.Mango.Models.Banks;
using Solnet.Mango.Models.Caches;
using Solnet.Mango.Models.Events;
using Solnet.Mango.Models.Matching;
using Solnet.Mango.Models.Perpetuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class DeserializationTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AdvancedOrderException()
        {
            var buf = new byte[AdvancedOrder.Layout.Length + 1];

            AdvancedOrder.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AdvancedOrdersAccountException()
        {
            var buf = new byte[AdvancedOrdersAccount.Layout.Length+1];

            AdvancedOrdersAccount.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NodeBankException()
        {
            var buf = new byte[NodeBank.Layout.Length+1];

            NodeBank.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RootBankException()
        {
            var buf = new byte[RootBank.Layout.Length+1];

            RootBank.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MangoGroupException()
        {
            var buf = new byte[MangoGroup.Layout.Length+1];

            MangoGroup.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MangoAccountException()
        {
            var buf = new byte[MangoAccount.Layout.Length+1];

            MangoAccount.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MangoCacheException()
        {
            var buf = new byte[MangoCache.Layout.Length+1];

            MangoCache.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PerpMarketCacheException()
        {
            var buf = new byte[PerpMarketCache.Layout.Length+1];

            PerpMarketCache.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PriceCacheException()
        {
            var buf = new byte[PriceCache.Layout.Length+1];

            PriceCache.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RootBankCacheException()
        {
            var buf = new byte[RootBankCache.Layout.Length+1];

            RootBankCache.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PerpAccountException()
        {
            var buf = new byte[PerpAccount.Layout.Length+1];

            PerpAccount.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PerpMarketException()
        {
            var buf = new byte[PerpMarket.Layout.Length+1];

            PerpMarket.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PerpMarketInfoException()
        {
            var buf = new byte[PerpMarketInfo.Layout.Length+1];

            PerpMarketInfo.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SpotMarketInfoException()
        {
            var buf = new byte[SpotMarketInfo.Layout.Length+1];

            SpotMarketInfo.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OrderBookSideException()
        {
            var buf = new byte[OrderBookSide.Layout.Length+1];

            OrderBookSide.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueueHeaderException()
        {
            var buf = new byte[QueueHeader.Layout.Length+1];

            QueueHeader.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NodeException()
        {
            var buf = new byte[Node.Layout.Length+1];

            Node.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MetadataException()
        {
            var buf = new byte[MetaData.Layout.Length+1];

            MetaData.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiquidateEventException()
        {
            var buf = new byte[LiquidateEvent.Layout.Length+1];

            LiquidateEvent.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OutEventException()
        {
            var buf = new byte[OutEvent.Layout.Length+1];

            OutEvent.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FillEventException()
        {
            var buf = new byte[FillEvent.Layout.Length+1];

            FillEvent.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LiquidityMiningInfoException()
        {
            var buf = new byte[LiquidityMiningInfo.Layout.Length+1];

            LiquidityMiningInfo.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PerpTriggerOrderException()
        {
            var buf = new byte[PerpTriggerOrder.Layout.Length+1];

            PerpTriggerOrder.Deserialize(buf);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TokenInfoException()
        {
            var buf = new byte[TokenInfo.Layout.Length+1];

            TokenInfo.Deserialize(buf);
        }
    }
}
