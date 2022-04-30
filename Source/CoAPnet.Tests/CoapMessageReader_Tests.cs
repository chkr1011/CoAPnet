using CoAPnet.Protocol.Encoding;
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
            Assert.IsFalse(reader.EndOfStream);
            Assert.AreEqual(1, reader.ReadBits(2));
            Assert.IsTrue(reader.EndOfStream);
            Assert.AreEqual(3, reader.ReadBits(2));
            Assert.IsTrue(reader.EndOfStream);
            Assert.AreEqual(15, reader.ReadBits(4));
            Assert.IsTrue(reader.EndOfStream);
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
            Assert.IsFalse(reader.EndOfStream);
            Assert.AreEqual(1, reader.ReadBits(2));
            Assert.IsFalse(reader.EndOfStream);
            Assert.AreEqual(3, reader.ReadBits(2));
            Assert.IsFalse(reader.EndOfStream);
            Assert.AreEqual(15, reader.ReadBits(4));
            Assert.IsFalse(reader.EndOfStream);
            Assert.AreEqual(254, reader.ReadBits(8));
            Assert.IsTrue(reader.EndOfStream);
        }
    }
}
