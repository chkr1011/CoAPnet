using CoAPnet;
using CoAPnet.Client;
using CoAPnet.Extensions.DTLS;
using CoAPnet.Protocol;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoAP.TestClient
{
    class Program
    {
        static async Task Main()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            var coapClient = new CoapFactory().CreateClient();

            Console.WriteLine("CONNECTING...");

            var request = new CoapRequest
            {
                Method = CoapRequestMethod.Get,
                Uri = "15001"
            };

            await coapClient.ConnectAsync(new CoapClientConnectOptions
            {
                Host = "192.168.1.228",
                Port = 5684,
                TransportLayer = new UdpWithDtlsCoapTransportLayer()
                {
                    Credentials = new PreSharedKey
                    {
                        Identity = Encoding.ASCII.GetBytes("IDENTITY"),
                        Key = Encoding.ASCII.GetBytes("lqxbBH6o2eAKSo5A")
                    }
                }
            }, CancellationToken.None);

            var response = await coapClient.Request(request, CancellationToken.None);

            Console.WriteLine("DATA RECEIVED");

            Console.WriteLine("Code = " + response.StatusCode);
            Console.WriteLine("Payload = " + Encoding.ASCII.GetString(response.Payload.ToArray()));

            Console.ReadLine();
        }
    }
}