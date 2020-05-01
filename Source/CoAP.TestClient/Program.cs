using CoAPnet;
using CoAPnet.Client;
using CoAPnet.Extensions.DTLS;
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
            using (_coapClient = new CoapFactory().CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("GW-B8D7AF2B3EA3.fritz.box")
                    .WithPort(5684)
                    .WithTransportLayer(new DtlsCoapTransportLayerBuilder()
                        .WithPreSharedKey("IDENTITY", "lqxbBH6o2eAKSo5A")
                        .Build())
                    .Build();

                await _coapClient.ConnectAsync(connectOptions, CancellationToken.None);

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001")
                    .Build();

                await SendRequest(request).ConfigureAwait(false);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001/65550")
                    .Build();

                await SendRequest(request).ConfigureAwait(false);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Put)
                    .WithPath("15001/65550")
                    .WithPayload("{\"3311\": [{\"5850\": 0}]}")
                    .Build();

                await SendRequest(request).ConfigureAwait(false);
            }
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