using CoAPnet.Protocol;
using CoAPnet.Protocol.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace CoAPnet.Tests
{
    [TestClass]
    public class CoapMessageDecoder_Tests
    {
        [TestMethod]
        public void Encode_And_Decode_Full()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            var message = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCodes.Post,
                Id = 0x5876,
                Token = new byte[] { 1, 2, 3, 4 },
                Payload = Encoding.UTF8.GetBytes("payloadOver13chars")
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPort(5648)
            };

            Enocde_And_Decode_Internal(message);
        }

        [TestMethod]
        public void Encode_And_Decode_Payload_Length_12()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            var message = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCodes.Put,
                Id = ushort.MaxValue,
                Payload = Encoding.UTF8.GetBytes("123456789012")
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPort(5648)
            };

            Enocde_And_Decode_Internal(message);
        }

        [TestMethod]
        public void Encode_And_Decode_No_Payload()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            var message = new CoapMessage
            {
                Type = CoapMessageType.Acknowledgement,
                Code = CoapMessageCodes.Get,
                Id = 0x5876,
                Token = new byte[] { 1, 2, 3, 4 },
                Payload = null
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPort(50)
            };

            Enocde_And_Decode_Internal(message);
        }

        [TestMethod]
        public void Encode_And_Decode_No_Token()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            var message = new CoapMessage
            {
                Type = CoapMessageType.NonConfirmable,
                Code = CoapMessageCodes.Put,
                Id = 0x5876,
                Token = null,
                Payload = null
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPort(66000)
            };

            Enocde_And_Decode_Internal(message);
        }

        [TestMethod]
        public void Encode_And_Decode_Multiple_Uri_Path()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            var message = new CoapMessage
            {
                Type = CoapMessageType.Acknowledgement,
                Code = CoapMessageCodes.Delete,
                Id = 0x50,
                Token = null,
                Payload = null
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPath("a"),
                optionBuilder.CreateUriPath("b"),
                optionBuilder.CreateUriPath("c")
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
            CollectionAssert.AreEqual(message.Token, decodedMessage.Token);
            CollectionAssert.AreEqual(message.Options, decodedMessage.Options);

            if (message.Payload.Array == null && message.Payload.Array == null)
            {
                return;
            }

            CollectionAssert.AreEqual(message.Payload.ToArray(), decodedMessage.Payload.ToArray());
        }
    }
}
