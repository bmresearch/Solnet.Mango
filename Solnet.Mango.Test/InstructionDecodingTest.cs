using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class InstructionDecodingTest
    {
        private const string CreateInitAccountAndAddInfoMessage = "AgAEBgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4xLDRRtH4Ze1GxzHBPIYFJR6C3wI1sYogL3OugLMKQb0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhBqfVFxksXFEhjMlMPUrxf1ja7gibof1E49vZigAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMU/FoRaGfwosKTudIQhkxNpfx+mTRQxczI0V3JV+ojoSAwICAAE0AAAAAIDV1QEAAAAAyBAAAAAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMQUEAwEABAQBAAAABQMDAQAkIgAAAA4AAAAAAAAAU29sbmV0IFRlc3QgdjEAAAAAAAAAAAAA";

        private const string CreateAccountAndAddInfoMessage = "AQACBQpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4yiEgLUxe5THUGq5sQ2Rv8F8NRHbDK0ZojLyq5m4ZIiGYAxig2FEvystJRkjr1kfkvxGi/EKTL1QaZFDVZrfaJAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjHS6qj2L5fLL9vU1O/8M8N6i3y85WY4/YMyCEgUJ/1Z4AIEBAECAAMMNwAAAAEAAAAAAAAABAMBAgAkIgAAAA4AAAAAAAAAU29sbmV0IFRlc3QgdjEAAAAAAAAAAAAA";

        [ClassInitialize]
        public static void Setup(TestContext tc)
        {
            InstructionDecoder.Register(MangoProgram.DevNetProgramIdKeyV3, MangoProgram.Decode);
            // do not assert the program key of the decoded instruction because it defaults to mainnet
        }

        [TestMethod]
        public void TestDecodeCreateAndAddAccountInfo()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CreateInitAccountAndAddInfoMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, ix.Count);
            Assert.AreEqual("Initialize Mango Account", ix[1].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[1].ProgramName);
            Assert.AreEqual(0, ix[1].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[1].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("EEoHecXUMTx5omC1cTHDuv7VKGTmzq8M5d86GnBHYMPS", ix[1].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[1].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("SysvarRent111111111111111111111111111111111", ix[1].Values.GetValueOrDefault("Sysvar Rent").ToString());

            Assert.AreEqual("Add Mango Account Info", ix[2].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[2].ProgramName);
            Assert.AreEqual(0, ix[2].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[2].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("EEoHecXUMTx5omC1cTHDuv7VKGTmzq8M5d86GnBHYMPS", ix[2].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[2].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("Solnet Test v1", ix[2].Values.GetValueOrDefault("Account Info"));
        }

        [TestMethod]
        public void TestCreateAccountAndAddInfo()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CreateAccountAndAddInfoMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, ix.Count);
            Assert.AreEqual("Create Mango Account", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());

            Assert.AreEqual("Add Mango Account Info", ix[1].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[1].ProgramName);
            Assert.AreEqual(0, ix[1].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[1].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[1].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[1].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("Solnet Test v1", ix[1].Values.GetValueOrDefault("Account Info"));
        }
    }
}
