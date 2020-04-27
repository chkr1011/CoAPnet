using CoapTest;
using CoapTest.Protocol.Encoding;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Waher.Runtime.Inventory;
using Waher.Security.DTLS;

namespace CoAP.TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var optionBuilder = new CoapMessageOptionFactory();

            //var message = new CoapMessage
            //{
            //    Type = CoapMessageType.Confirmable,
            //    Code = CoapMessageCodes.Post,
            //    Id = 50,
            //    Payload = Encoding.ASCII.GetBytes("{\"9090\":\"IDENTITY\"}")
            //};

            //message.Options = new List<CoapMessageOption>
            //{
            //    optionBuilder.CreateUriPort(5648),
            //    optionBuilder.CreateUriPath("15011"),
            //    optionBuilder.CreateUriPath("9063"),
            //};

            var message = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCodes.Get,
                Id = 50
            };

            message.Options = new List<CoapMessageOption>
            {
                optionBuilder.CreateUriPort(5648),
                optionBuilder.CreateUriPath("15001"),
            };

            var messageEncoder = new CoapMessageEncoder();
            var buffer = messageEncoder.Encode(message);

            Console.WriteLine("CONNECTING...");

            Types.Initialize(typeof(Waher.Security.DTLS.Ciphers.PskCipher).Assembly);

            var udpClient = new UdpClient(0, AddressFamily.InterNetwork);
            var ikeaGatewayEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.228"), 5684);

            var psk = new PresharedKey(Encoding.ASCII.GetBytes("IDENTITY"), Encoding.ASCII.GetBytes("lqxbBH6o2eAKSo5A"));

            using (var dtlsClient = new DtlsOverUdp(udpClient, DtlsMode.Client, null, null))
            {
                dtlsClient.OnDatagramReceived += (s, e) =>
                {
                    var decoder = new CoapMessageDecoder();
                    var message = decoder.Decode(e.Datagram);

                    Console.WriteLine("DATA RECEIVED");

                    Console.WriteLine("Code = " + message.Code);
                    Console.WriteLine("Type = " + message.Type);
                    Console.WriteLine("ID = " + message.Id);
                    Console.WriteLine("Payload = " + Encoding.ASCII.GetString(message.Payload.ToArray()));

                };

                dtlsClient.Send(buffer.ToArray(), ikeaGatewayEndpoint, psk, (s, e) =>
                {
                    Console.WriteLine("Successful = " + e.Successful);
                }, null);

                Console.ReadLine();
            }
        }
    }
}