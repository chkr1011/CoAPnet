using CoAPnet.Client;
using System;

namespace CoAPnet.Extensions.DTLS
{
    public static class CoapClientConnectOptionsBuilderExtensions
    {
        public static CoapClientConnectOptionsBuilder RegisterDtlsExtension(this CoapClientConnectOptionsBuilder clientConnectOptionsBuilder)
        {
            if (clientConnectOptionsBuilder is null) throw new ArgumentNullException(nameof(clientConnectOptionsBuilder));

            return clientConnectOptionsBuilder;
        }

        public static CoapClientConnectOptionsBuilder WithDtlsTransport(this CoapClientConnectOptionsBuilder clientConnectOptionsBuilder, CoapDtlsTransportOptions options)
        {
            if (clientConnectOptionsBuilder is null) throw new ArgumentNullException(nameof(clientConnectOptionsBuilder));
            if (options is null) throw new ArgumentNullException(nameof(options));

            clientConnectOptionsBuilder.WithTransportLayer(new DtlsCoapTransportLayer()
            {
                Credentials = options.Credentials
            });

            return clientConnectOptionsBuilder;
        }
    }
}
