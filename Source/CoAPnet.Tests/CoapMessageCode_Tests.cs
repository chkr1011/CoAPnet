using CoAPnet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoAPnet.Tests
{
    [TestClass]
    public class CoapMessageCode_Tests
    {
        [TestMethod]
        public void Code_Equals()
        {
            var codeA = CoapMessageCodes.Get;
            var codeB = CoapMessageCodes.Get;

            // Do not use Assert.AreEqual becuase we want to test the _Equals_ implementation of the code.
            Assert.IsTrue(codeA.Equals(codeB));
        }
    }
}
