using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Types;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class MangoUtilsTest
    {
        [TestMethod]
        public void TriggerPriceToNative()
        {
            var expected = new I80F48(0.0999999999999978683717927197m);
            var nativeTriggerPrice = MangoUtils.TriggerPriceToNative(100, 9, 6);

            Assert.AreEqual(expected, nativeTriggerPrice);
        }

        [TestMethod]
        public void TriggerPriceToNumber()
        {
            var nativeTriggerPrice = new I80F48(0.0999999999997669419826706871m);
            var expected = 99.99999999976338926899188660m;
            var triggerPrice = MangoUtils.TriggerPriceToNumber(nativeTriggerPrice, 9, 6);

            Assert.AreEqual(expected, triggerPrice);
        }
    }
}
