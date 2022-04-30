using System;
using CoAPnet.Logging;
using CoAPnet.Protocol;
using CoAPnet.Protocol.Encoding;
using CoAPnet.Protocol.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace CoAPnet.Tests
{
    [TestClass]
    public sealed class CoapMessageDecoder_Tests
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
                Payload = Encoding.UTF8.GetBytes("payloadOver13chars"),
                Options = new List<CoapMessageOption>
                {
                    optionBuilder.CreateETag(new byte[12]),
                    optionBuilder.CreateUriPort(5648),
                    optionBuilder.CreateContentFormat(CoapMessageContentFormat.ApplicationJson)
                }
            };

            EncodeAndDecodeInternal(message);
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
                Payload = Encoding.UTF8.GetBytes("123456789012"),
                Options = new List<CoapMessageOption>
                {
                    optionBuilder.CreateUriPort(5648),
                    optionBuilder.CreateContentFormat(CoapMessageContentFormat.ApplicationJson)
                }
            };

            EncodeAndDecodeInternal(message);
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
                Payload = null,
                Options = new List<CoapMessageOption>
                {
                    optionBuilder.CreateUriPort(50),
                    optionBuilder.CreateContentFormat(CoapMessageContentFormat.ApplicationJson)
                }
            };

            EncodeAndDecodeInternal(message);
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
                Payload = null,
                Options = new List<CoapMessageOption>
                {
                    optionBuilder.CreateUriPort(66000),
                    optionBuilder.CreateContentFormat(CoapMessageContentFormat.ApplicationJson)
                }
            };

            EncodeAndDecodeInternal(message);
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
                Payload = null,
                Options = new List<CoapMessageOption>
                {
                    optionBuilder.CreateUriPath("a"),
                    optionBuilder.CreateUriPath("b"),
                    optionBuilder.CreateUriPath("c")
                }
            };

            EncodeAndDecodeInternal(message);
        }

        static void EncodeAndDecodeInternal(CoapMessage message)
        {
            var messageEncoder = new CoapMessageEncoder();
            var messageBuffer = messageEncoder.Encode(message);

            // Use a larger buffer to ensure that reading within the bounds works.
            var largerMessageBuffer = new byte[messageBuffer.Count * 2];
            Array.Copy(messageBuffer.Array, messageBuffer.Offset, largerMessageBuffer, 0, messageBuffer.Count);
            Array.Copy(messageBuffer.Array, messageBuffer.Offset, largerMessageBuffer, messageBuffer.Count, messageBuffer.Count);

            messageBuffer = new ArraySegment<byte>(largerMessageBuffer, 0, messageBuffer.Count);
            
            var messageDecoder = new CoapMessageDecoder(new CoapNetLogger());
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
