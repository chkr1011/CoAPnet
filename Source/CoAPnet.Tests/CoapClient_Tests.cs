using System.Threading;
using System.Threading.Tasks;
using CoAPnet.Client;
using CoAPnet.Exceptions;
using CoAPnet.Extensions.DTLS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoAPnet.Tests
{
    [TestClass]
    public class CoapClient_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(CoapCommunicationException))]
        public async Task Connect_Invalid_Host()
        {
            using (var coapClient = new CoapFactory().CreateClient())
            {
                await coapClient.ConnectAsync(new CoapClientConnectOptions()
                {
                    Host = "invalid_host"
                }, CancellationToken.None);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CoapCommunicationException))]
        public async Task Connect_Valid_Host_With_Invalid_Port_Dtls()
        {
            using (var coapClient = new CoapFactory().CreateClient())
            {
                var options = new CoapClientConnectOptionsBuilder()
                    .WithHost("127.0.0.1")
                    .WithPort(5555)
                    .WithDtlsTransportLayer(o =>
                    {
                        o.WithPreSharedKey("a", "b");
                    })
                    .Build();

                await coapClient.ConnectAsync(options, CancellationToken.None);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CoapCommunicationTimedOutException))]
        public async Task Timeout_When_No_Response_Received()
        {
            using (var coapClient = new CoapFactory().CreateClient())
            {
                var options = new CoapClientConnectOptionsBuilder()
                    .WithHost("123.123.123.1")
                    .WithPort(5555)
                    .Build();

                await coapClient.ConnectAsync(options, CancellationToken.None);

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .Build();

                await coapClient.RequestAsync(request, CancellationToken.None);
            }
        }
    }
}
