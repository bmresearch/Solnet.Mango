using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class TestI80F48
    {
        private byte[] MaxValueBytes = new byte[]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            127,
        };

        private byte[] MinValueBytes = new byte[]
        {
           0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128
        };

        private byte[] ZeroValueBytes = new byte[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private byte[] OneValueBytes = new byte[]
        {
            0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private byte[] OneNegValueBytes = new byte[]
        {
            0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
        };

        private byte[] OneFracValueBytes = new byte[]
        {
            0, 0, 0, 0, 0, 64, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private byte[] OneFracNegValueBytes = new byte[]
        {
            0, 0, 0, 0, 0, 192, 254, 255, 255, 255, 255, 255, 255, 255, 255, 255,
        };

        [TestMethod]
        public void TestCreateMaxRepresentation()
        {
            var value = new I80F48(BigInteger.Parse("170141183460469231731687303715884105727"));
        }

        [TestMethod]
        public void TestCreateMinRepresentation()
        {
            var value = new I80F48(BigInteger.Parse("-170141183460469231731687303715884105728"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCreateHigherThanMaxRepresentation()
        {
            var value = new I80F48(BigInteger.Parse("170141183460469231731687303715884105728"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCreateLowerThanMinRepresentation()
        {
            var value = new I80F48(BigInteger.Parse("-170141183460469231731687303715884105729"));
        }


        [TestMethod]
        public void TestFromArrayMaxValue()
        {
            var value = I80F48.Deserialize(MaxValueBytes);
        }

        [TestMethod]
        public void TestFromArrayMinValue()
        {
            var value = I80F48.Deserialize(MinValueBytes);
        }

        [TestMethod]
        public void TestFromArrayArbitraryValues()
        {
            var value = I80F48.Deserialize(ZeroValueBytes);
            Assert.AreEqual(0, value.ToDecimal());
            Assert.AreEqual(0, value.GetData());

            value = I80F48.Deserialize(OneValueBytes);
            Assert.AreEqual(1, value.ToDecimal());
            Assert.AreEqual(281474976710656, value.GetData());

            value = I80F48.Deserialize(OneNegValueBytes);
            Assert.AreEqual(-1, value.ToDecimal());
            Assert.AreEqual(-281474976710656, value.GetData());

            value = I80F48.Deserialize(OneFracValueBytes);
            Assert.AreEqual(1.25m, value.ToDecimal());
            Assert.AreEqual(351843720888320, value.GetData());

            value = I80F48.Deserialize(OneFracNegValueBytes);
            Assert.AreEqual(-1.25m, value.ToDecimal());
            Assert.AreEqual(-351843720888320, value.GetData());
        }

        [TestMethod]
        public void TestOperations()
        {
            var value = new I80F48(new BigInteger(281_474_976_710_656));
            var value2 = new I80F48(new BigInteger(281_474_976_710_656));

            var v3 = value + value2;
            var v3d = v3.ToDecimal();

            Assert.AreEqual(2m, v3d);

            var v4 = v3 * v3;
            var v4d = v4.ToDecimal();

            Assert.AreEqual(4m, v4d);

            var v5 = value / v4;
            var v5d = v5.ToDecimal();

            Assert.AreEqual(0.25m, v5d);
        }


        [TestMethod]
        public void SerDesMax()
        {
            var a = I80F48.MaxValue;

            var data = I80F48.Serialize(a);
            var a1 = I80F48.Deserialize(data);

            Assert.AreEqual(a, a1);
        }

        [TestMethod]
        public void SerDesMin()
        {
            var a = I80F48.MinValue;

            var data = I80F48.Serialize(a);
            var a1 = I80F48.Deserialize(data);

            Assert.AreEqual(a, a1);
        }

        [TestMethod]
        public void SerDesSmallNumber()
        {
            var a = (I80F48)0.00000000876123m;

            var data = I80F48.Serialize(a);
            var a1 = I80F48.Deserialize(data);

            Assert.AreEqual(a, a1);
        }

        [TestMethod]
        public void SerDesInt()
        {
            var a = (I80F48)ulong.MaxValue;

            var data = I80F48.Serialize(a);
            var a1 = I80F48.Deserialize(data);

            Assert.AreEqual(ulong.MaxValue, (ulong)a1);
        }

        [TestMethod]
        public void SerDesSmallNumberNeg()
        {
            var a = (I80F48)(-0.00000000876123m);

            var data = I80F48.Serialize(a);
            var a1 = I80F48.Deserialize(data);


            var bi = new BigInteger(-1);

            var bytes = new byte[16];

            bi.TryWriteBytes(bytes, out var wr);

            if(wr != 16)
            {
                bytes[wr - 1] = (byte)(bytes[wr - 1] & 0x7F);
                while (wr < 16) bytes[wr++] = 0xFF;
            }

            var bi2 = new BigInteger(bytes);

            Assert.AreEqual(a, a1);
        }

        [TestMethod]
        public void SerDesIntNeg()
        {
            var b = (I80F48)ulong.MaxValue;

            var data2 = I80F48.Serialize(b);
            var b1 = I80F48.Deserialize(data2);

            Assert.AreEqual(ulong.MaxValue, (ulong)b1);


            var a = (I80F48)int.MinValue;

            var data = I80F48.Serialize(a);
            var a1 = I80F48.Deserialize(data);

            Assert.AreEqual(int.MinValue, (int)a1);
        }
    }
}
