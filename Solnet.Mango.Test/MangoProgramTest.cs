using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class MangoProgramTest
    {

        [TestMethod]
        public void TestDeriveMangoAccountAddress()
        {
            var mango = MangoProgram.CreateDevNet();

            var address = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", address);
        }
    }
}
