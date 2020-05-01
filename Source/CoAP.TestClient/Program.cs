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
    static class Program
    {
        static ICoapClient _coapClient;

        static async Task Main()
        {
            var optionBuilder = new CoapMessageOptionFactory();

            _coapClient = new CoapFactory().CreateClient();

            Console.WriteLine("< CONNECTING...");

            var request = new CoapRequest
            {
                Method = CoapRequestMethod.Get,
                UriPath = "15001"
            };

            await _coapClient.ConnectAsync(new CoapClientConnectOptions
            {
                Host = "GW-B8D7AF2B3EA3.fritz.box",
                Port = 5684,
                TransportLayer = new DtlsCoapTransportLayer()
                {
                    Credentials = new PreSharedKey
                    {
                        Identity = Encoding.ASCII.GetBytes("IDENTITY"),
                        Key = Encoding.ASCII.GetBytes("lqxbBH6o2eAKSo5A")
                    }
                }
            }, CancellationToken.None);

            await SendRequest(request).ConfigureAwait(false);

            request = new CoapRequest
            {
                Method = CoapRequestMethod.Get,
                UriPath = "15001/65550"
            };

            await SendRequest(request).ConfigureAwait(false);

            request = new CoapRequest
            {
                Method = CoapRequestMethod.Put,
                UriPath = "15001/65550",
                Payload = Encoding.ASCII.GetBytes("{\"3311\": [{\"5850\": 1}]}")
            };

            await SendRequest(request).ConfigureAwait(false);
        }

        static async Task SendRequest(CoapRequest request)
        {
            var response = await _coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
            PrintResponse(response);
        }

        static void PrintResponse(CoapResponse response)
        {
            Console.WriteLine("> RESPONSE");
            Console.WriteLine("   + Status         = " + response.StatusCode);
            Console.WriteLine("   + Status code    = " + (int)response.StatusCode);
            Console.WriteLine("   + Content format = " + response.Options.ContentFormat);
            Console.WriteLine("   + Max age        = " + response.Options.MaxAge);
            Console.WriteLine("   + Payload        = " + Encoding.UTF8.GetString(response.Payload.ToArray()));
            Console.WriteLine();
        }
    }
}