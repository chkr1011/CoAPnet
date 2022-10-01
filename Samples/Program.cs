using CoAPnet;
using CoAPnet.Client;
using CoAPnet.Extensions.DTLS;
#pragma warning disable CS8321

static async Task RequestStatusFromTradfriGateway()
{
    /*
     * This sample connects to a IKEA Tradri gateway and reads the status.
     * Keys, path etc. are only samples but the usage of the library is correct.
     */
    var coapFactory = new CoapFactory();
    using var coapClient = coapFactory.CreateClient();

    var connectOptions = new CoapClientConnectOptionsBuilder()
        .WithHost("GW-B8D7AF2B3EA3.fritz.box")
        .WithDtlsTransportLayer(o =>
            o.WithPreSharedKey("Client_identity", "7x3A1gqWvu9cBGD7"))
        .Build();

    using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
    {
        await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token);
    }

    var request = new CoapRequestBuilder()
        .WithMethod(CoapRequestMethod.Get)
        .WithPath("15001")
        .Build();

    using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
    {
        var response = await coapClient.RequestAsync(request, cancellationTokenSource.Token);
        
        // Do something with the response.
        Console.WriteLine(response.StatusCode);
    }
    
    // Is also supported to make several more requests using the client instance.
}