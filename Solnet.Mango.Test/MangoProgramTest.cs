using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Serum;
using Solnet.Serum.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
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
        public void DeriveMangoAccountAddress()
        {
            var mango = MangoProgram.CreateDevNet();

            var address = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", address);
        }

        [TestMethod]
        public void DeriveAdvancedOrdersAccountAddress()
        {
            var mango = MangoProgram.CreateDevNet();
            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var address = mango.DeriveAdvancedOrdersAccountAddress(mangoAccount);

            Assert.AreEqual("AZofyy49f3sY6bt3F1vgMse92eZdFuCM8jkV1USofneA", address);
        }

        [TestMethod]
        public void CreateMangoAccount()
        {
            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var address = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var ix = mango.CreateMangoAccount(Constants.DevNetMangoGroup, address, new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            Assert.AreEqual(4, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 55, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, ix.Data);
        }

        [TestMethod]
        public void CloseMangoAccount()
        {
            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var address = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var ix = mango.CloseMangoAccount(Constants.DevNetMangoGroup, address, new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"));

            Assert.AreEqual(3, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 50, 0, 0, 0 }, ix.Data);
        }

        [TestMethod]
        public void InitAdvancedOrders()
        {
            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var address = mango.DeriveAdvancedOrdersAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"));

            var ix = mango.InitAdvancedOrders(Constants.DevNetMangoGroup, mangoAccount, new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), address);

            Assert.AreEqual(5, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 42, 0, 0, 0 }, ix.Data);
        }

        [TestMethod]
        public void Deposit()
        {
            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var ix = mango.Deposit(
                Constants.DevNetMangoGroup,
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z"), // devnet mango cache
                new("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT"), // devnet root bank for SOL
                new("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe"), // devnet node bank for SOL
                new("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy"), // devnet node bank vault for SOL
                new("BSK24XvJda991ySKhgj8S15XrneYd1k6ekCZpQexrF76"), // devnet token account for wSOL
                1_000_000);

            Assert.AreEqual(9, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 2, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0 }, ix.Data);
        }

        [TestMethod]
        public void Withdraw()
        {
            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var openOrders = new List<PublicKey>();

            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                openOrders.Add(SystemProgram.ProgramIdKey);
            }
            var ix = mango.Withdraw(
                Constants.DevNetMangoGroup,
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z"), // devnet mango cache
                new("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT"), // devnet root bank for SOL
                new("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe"), // devnet node bank for SOL
                new("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy"), // devnet node bank vault for SOL
                new("BSK24XvJda991ySKhgj8S15XrneYd1k6ekCZpQexrF76"), // devnet token account for wSOL
                new("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax"), // devnet mango signer
                openOrders,
                1_000_000, false);

            Assert.AreEqual(25, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 3, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0, 0 }, ix.Data);
        }

        [TestMethod]
        public void WithdrawAllowBorrow()
        {
            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var openOrders = new List<PublicKey>();

            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                openOrders.Add(SystemProgram.ProgramIdKey);
            }

            var ix = mango.Withdraw(
                Constants.DevNetMangoGroup,
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z"), // devnet mango cache
                new("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT"), // devnet root bank for SOL
                new("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe"), // devnet node bank for SOL
                new("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy"), // devnet node bank vault for SOL
                new("BSK24XvJda991ySKhgj8S15XrneYd1k6ekCZpQexrF76"), // devnet token account for wSOL
                new("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax"), // devnet mango signer
                openOrders,
                1_000_000, true);

            Assert.AreEqual(25, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 3, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0, 1 }, ix.Data);
        }

        [TestMethod]
        public void CreateSpotOpenOrders()
        {
            var serum = SerumProgram.CreateDevNet();

            Assert.AreEqual(SerumProgram.DevNetProgramIdKeyV3, serum.ProgramIdKey);
            Assert.AreEqual("Serum Program", serum.ProgramName);

            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var ix = mango.InitSpotOpenOrders(
                Constants.DevNetMangoGroup,
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("2kMXoEVQabmHBxoRtkbzAUAMUvYtd5aGAKW9xdyeArwi"),
                new("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR"), // devnet sol/usdc market
                new("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax") // devnet mango signer
                );

            Assert.AreEqual(8, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 32, 0, 0, 0 }, ix.Data);
        }

        [TestMethod]
        public void CloseSpotOpenOrders()
        {
            var serum = SerumProgram.CreateDevNet();

            Assert.AreEqual(SerumProgram.DevNetProgramIdKeyV3, serum.ProgramIdKey);
            Assert.AreEqual("Serum Program", serum.ProgramName);

            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var ix = mango.CloseSpotOpenOrders(
                Constants.DevNetMangoGroup,
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("2kMXoEVQabmHBxoRtkbzAUAMUvYtd5aGAKW9xdyeArwi"),
                new("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR"), // devnet sol/usdc market
                new("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax") // devnet mango signer
                );

            Assert.AreEqual(7, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 51, 0, 0, 0 }, ix.Data);
        }

        [TestMethod]
        public void PlaceSpotOrder()
        {
            var serum = SerumProgram.CreateDevNet();

            Assert.AreEqual(SerumProgram.DevNetProgramIdKeyV3, serum.ProgramIdKey);
            Assert.AreEqual("Serum Program", serum.ProgramName);

            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var order = new OrderBuilder()
                .SetSide(Side.Sell)
                .SetOrderType(OrderType.ImmediateOrCancel)
                .SetSelfTradeBehavior(SelfTradeBehavior.AbortTransaction)
                .SetClientOrderId(1_000_000UL)
                .SetPrice(105)
                .SetQuantity(25)
                .Build();

            var openOrders = new List<PublicKey>();
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                if (i == 3)
                {
                    openOrders.Add(new("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC"));
                }
                else
                {
                    openOrders.Add(SystemProgram.ProgramIdKey);
                }
            }

#pragma warning disable CS0618 // Type or member is obsolete
            var ix = mango.PlaceSpotOrder(
                Constants.DevNetMangoGroup,
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z"),
                new("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR"),
                new("8ezpneRznTJNZWFSLeQvtPCagpsUVWA7djLSzqp3Hx4p"),
                new("8gJhxSwbLJkDQbqgzbJ6mDvJYnEVWB6NHWEN9oZZkwz7"),
                new("4hm9fSbga3vosq4K4czPFwy1tmigc4io4ZLJqgC2s4Yv"),
                new("48be6VKEq86awgUjfvbKDmEzXr4WNR7hzDxfF6ZPptmd"),
                new("EfwvntttcP253P1g8Jqo18Rywyb33deGe69Txn5LSgwQ"),
                new("HGsnYZr6yodSHXHNKdtprooS9XP4h19hrzXfgJLuA3MB"),
                new("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT"),
                new("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe"),
                new("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy"),
                new("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN"),
                new("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM"),
                new("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC"),
                new("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax"),
                new("21YuRgN6iHgsucfXT6Yzo2dV7dzuAdz2vxExahY3MueT"),
                new("7nS8AgndAVYCfSTaXabwqcBWBc3xCLwwcPiMWzuTbfrf"),
                openOrders,
                3,
                order);
#pragma warning restore CS0618 // Type or member is obsolete

            var expectedData =
                new byte[] { 
                    9, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    2, 0, 0, 0, 1, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0,
                    0, 255, 255 
                };
            Assert.AreEqual(38, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(expectedData, ix.Data);
        }

        [TestMethod]
        public void PlaceSpotOrder2()
        {
            var serum = SerumProgram.CreateDevNet();

            Assert.AreEqual(SerumProgram.DevNetProgramIdKeyV3, serum.ProgramIdKey);
            Assert.AreEqual("Serum Program", serum.ProgramName);

            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var order = new OrderBuilder()
                .SetSide(Side.Sell)
                .SetOrderType(OrderType.ImmediateOrCancel)
                .SetSelfTradeBehavior(SelfTradeBehavior.AbortTransaction)
                .SetClientOrderId(1_000_000UL)
                .SetPrice(105)
                .SetQuantity(25)
                .Build();

            var openOrders = new List<PublicKey>();
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                if (i == 3)
                {
                    openOrders.Add(new("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC"));
                }
                else
                {
                    openOrders.Add(SystemProgram.ProgramIdKey);
                }
            }

            var ix = mango.PlaceSpotOrder2(
                Constants.DevNetMangoGroup,
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z"),
                new("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR"),
                new("8ezpneRznTJNZWFSLeQvtPCagpsUVWA7djLSzqp3Hx4p"),
                new("8gJhxSwbLJkDQbqgzbJ6mDvJYnEVWB6NHWEN9oZZkwz7"),
                new("4hm9fSbga3vosq4K4czPFwy1tmigc4io4ZLJqgC2s4Yv"),
                new("48be6VKEq86awgUjfvbKDmEzXr4WNR7hzDxfF6ZPptmd"),
                new("EfwvntttcP253P1g8Jqo18Rywyb33deGe69Txn5LSgwQ"),
                new("HGsnYZr6yodSHXHNKdtprooS9XP4h19hrzXfgJLuA3MB"),
                new("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT"),
                new("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe"),
                new("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy"),
                new("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN"),
                new("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM"),
                new("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC"),
                new("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax"),
                new("21YuRgN6iHgsucfXT6Yzo2dV7dzuAdz2vxExahY3MueT"),
                new("7nS8AgndAVYCfSTaXabwqcBWBc3xCLwwcPiMWzuTbfrf"),
                openOrders,
                3,
                order);

            var expectedData =
                new byte[] {
                    41, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    2, 0, 0, 0, 1, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0,
                    0, 255, 255
                };
            Assert.AreEqual(37, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(expectedData, ix.Data);
        }

        [TestMethod]
        public void SettleFunds()
        {
            var serum = SerumProgram.CreateDevNet();

            Assert.AreEqual(SerumProgram.DevNetProgramIdKeyV3, serum.ProgramIdKey);
            Assert.AreEqual("Serum Program", serum.ProgramName);

            var mango = MangoProgram.CreateDevNet();

            Assert.AreEqual(MangoProgram.DevNetProgramIdKeyV3, mango.ProgramIdKey);
            Assert.AreEqual("Mango Program V3", mango.ProgramName);

            var mangoAccount = mango.DeriveMangoAccountAddress(new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"), 1);

            var ix = mango.SettleFunds(
                Constants.DevNetMangoGroup,
                new("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z"),
                mangoAccount,
                new("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh"),
                new("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR"),
                new("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC"),
                new("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax"),
                new("EfwvntttcP253P1g8Jqo18Rywyb33deGe69Txn5LSgwQ"),
                new("HGsnYZr6yodSHXHNKdtprooS9XP4h19hrzXfgJLuA3MB"),
                new("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT"),
                new("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe"),
                new("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN"),
                new("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM"),
                new("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy"),
                new("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC"),
                new("21YuRgN6iHgsucfXT6Yzo2dV7dzuAdz2vxExahY3MueT"));

            Assert.AreEqual(18, ix.Keys.Count);
            CollectionAssert.AreEqual(Encoders.Base58.DecodeData(MangoProgram.DevNetProgramIdKeyV3), ix.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 19, 0, 0, 0 }, ix.Data);
        }
    }
}
