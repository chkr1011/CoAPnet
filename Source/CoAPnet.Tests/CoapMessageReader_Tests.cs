using CoapTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoAPnet.Tests
{
    [TestClass]
    public class CoapMessageReader_Tests
    {
        [TestMethod]
        public void Read_Bits()
        {
            var writer = new CoapMessageWriter();
            writer.WriteBits(1, 2);
            writer.WriteBits(3, 2);
            writer.WriteBits(15, 4);

            var reader = new CoapMessageReader(writer.ToArray());

            Assert.AreEqual(1, reader.ReadBits(2));
            Assert.AreEqual(3, reader.ReadBits(2));
            Assert.AreEqual(15, reader.ReadBits(4));
        }

        [TestMethod]
        public void Read_Spanning_Bits()
        {
            var writer = new CoapMessageWriter();
            writer.WriteBits(1, 2);
            writer.WriteBits(3, 2);
            writer.WriteBits(15, 4);
            writer.WriteBits(254, 8);

            var reader = new CoapMessageReader(writer.ToArray());

            Assert.AreEqual(1, reader.ReadBits(2));
            Assert.AreEqual(3, reader.ReadBits(2));
            Assert.AreEqual(15, reader.ReadBits(4));
            Assert.AreEqual(254, reader.ReadBits(8));
        }
    }
}
