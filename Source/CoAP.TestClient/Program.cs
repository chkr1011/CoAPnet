using CoAPnet;
using CoAPnet.Client;
using CoAPnet.Extensions.DTLS;
using CoAPnet.Logging;
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
            //await Main2();
            //return;

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

                await coapClient.ConnectAsync(connectOptions, CancellationToken.None).ConfigureAwait(false);

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
            Console.WriteLine("   + E tag          = " + ByteArrayToString(response.Options.ETag));
            Console.WriteLine("   + Payload        = " + Encoding.UTF8.GetString(response.Payload.ToArray()));
            Console.WriteLine();
        }

        static string ByteArrayToString(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return string.Empty;
            }

            var hex = new StringBuilder(buffer.Length * 2);
            hex.Append("0x");

            foreach (var @byte in buffer)
            {
                hex.AppendFormat("{0:x2}", @byte);
            }

            return hex.ToString();
        }

        static async Task Main2()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("coap.me")
                    .Build();

                await coapClient.ConnectAsync(connectOptions, CancellationToken.None).ConfigureAwait(false);

                //// hello

                //var request = new CoapRequestBuilder()
                //    .WithMethod(CoapRequestMethod.Get)
                //    .WithPath("hello")
                //    .Build();

                //var response = await coapClient.RequestAsync(request, CancellationToken.None);
                //PrintResponse(response);

                //// broken

                //request = new CoapRequestBuilder()
                //    .WithMethod(CoapRequestMethod.Get)
                //    .WithPath("broken")
                //    .Build();

                //response = await coapClient.RequestAsync(request, CancellationToken.None);
                //PrintResponse(response);

                //// secret

                //request = new CoapRequestBuilder()
                //    .WithMethod(CoapRequestMethod.Get)
                //    .WithPath("secret")
                //    .Build();

                //response = await coapClient.RequestAsync(request, CancellationToken.None);
                //PrintResponse(response);

                //// large-create

                //request = new CoapRequestBuilder()
                //    .WithMethod(CoapRequestMethod.Get)
                //    .WithPath("large-create")
                //    .Build();

                //response = await coapClient.RequestAsync(request, CancellationToken.None);
                //PrintResponse(response);

                //// location1/location2/location3

                //request = new CoapRequestBuilder()
                //    .WithMethod(CoapRequestMethod.Get)
                //    .WithPath("location1/location2/location3")
                //    .Build();

                //response = await coapClient.RequestAsync(request, CancellationToken.None);
                //PrintResponse(response);

                // large

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("large")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                //// separate

                //request = new CoapRequestBuilder()
                //    .WithMethod(CoapRequestMethod.Get)
                //    .WithPath("separate")
                //    .Build();

                //response = await coapClient.RequestAsync(request, CancellationToken.None);
                //PrintResponse(response);


                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}