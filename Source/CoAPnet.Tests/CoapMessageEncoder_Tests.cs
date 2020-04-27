using CoapTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoAPnet.Tests
{
    [TestClass]
    public class CoapMessageEncoder_Tests
    {
        [TestMethod]
        public void Encode_Simple_Get_Request()
        {
            var expected = Convert.FromBase64String("QAHLDnIWEP9wYXlsb2Fk");

            // v:1 t:CON c:GET i:cb0e {} [ Uri-Port:5648 ] :: 'payload'

            var optionBuilder = new CoapMessageOptionFactory();

            var message = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCodes.Get,
                Id = 0xcb0e,
                Payload = Encoding.UTF8.GetBytes("payload")
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPort(5648)
            };

            var messageEncoder = new CoapMessageEncoder();
            var messageBuffer = messageEncoder.Encode(message);

            AssertDataIsEqual(expected, messageBuffer.ToArray());
        }

        [TestMethod]
        public void Encode_Simple_Post_Request()
        {
            var expected = Convert.FromBase64String("QAJYdnIWEP9wYXlsb2FkT3ZlcjEzY2hhcnM=");

            //v:1 t:CON c:POST i:5876 {} [ Uri-Port:5648 ] :: 'payloadOver13chars'

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

            var messageEncoder = new CoapMessageEncoder();
            var messageBuffer = messageEncoder.Encode(message);

            AssertDataIsEqual(expected, messageBuffer.ToArray());
        }

        void AssertDataIsEqual(byte[] expected, byte[] actual)
        {
            Assert.AreEqual(ToHexString(expected), ToHexString(actual));
        }

        string ToHexString(byte[] data)
        {
            return string.Join("-", data.Select(d => d.ToString("X").PadLeft(2, '0')));
        }
    }
}
