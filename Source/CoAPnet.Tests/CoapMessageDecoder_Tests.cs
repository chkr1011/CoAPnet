using CoapTest;
using CoapTest.Protocol.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace CoAPnet.Tests
{
    [TestClass]
    public class CoapMessageDecoder_Tests
    {
        [TestMethod]
        public void Encode_And_Decode()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            var message = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCodes.Post,
                Id = 0x5876,
                Payload = Encoding.UTF8.GetBytes("payloadOver13chars")
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPort(5648)
            };

            Enocde_And_Decode_Internal(message);
        }

        void Enocde_And_Decode_Internal(CoapMessage message)
        {
            var messageEncoder = new CoapMessageEncoder();
            var messageBuffer = messageEncoder.Encode(message);

            var messageDecoder = new CoapMessageDecoder();
            var decodedMessage = messageDecoder.Decode(messageBuffer);

            Assert.AreEqual(message.Type, decodedMessage.Type);
            Assert.AreEqual(message.Code, decodedMessage.Code);
            Assert.AreEqual(message.Token, decodedMessage.Token);
            CollectionAssert.AreEqual(message.Options, decodedMessage.Options);
            CollectionAssert.AreEqual(message.Payload.ToArray(), decodedMessage.Payload.ToArray());
        }
    }
}
