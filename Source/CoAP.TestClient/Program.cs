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
        static async Task Main()
        {
            using (var coapClient = new CoapFactory().CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("GW-B8D7AF2B3EA3.fritz.box")
                    .WithPort(5684)
                    .WithDtlsTransportLayer(new DtlsCoapTransportLayerOptionsBuilder()
                        .WithPreSharedKey("IDENTITY", "lqxbBH6o2eAKSo5A")
                        .Build())
                    .Build();

                await coapClient.ConnectAsync(connectOptions, CancellationToken.None);

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001/65550")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Put)
                    .WithPath("15001/65550")
                    .WithPayload("{\"3311\": [{\"5850\": 1}]}")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);
            }
        }

        static void PrintResponse(CoapResponse response)
        {
            Console.WriteLine("> RESPONSE");
            Console.WriteLine("   + Status         = " + response.StatusCode);
            Console.WriteLine("   + Status code    = " + (int)response.StatusCode);
            Console.WriteLine("   + Content format = " + response.Options.ContentFormat);
            Console.WriteLine("   + Max age        = " + response.Options.MaxAge);
            Console.WriteLine("   + E tag          = " + response.Options.ETag);
            Console.WriteLine("   + Payload        = " + Encoding.UTF8.GetString(response.Payload.ToArray()));
            Console.WriteLine();
        }
    }
}