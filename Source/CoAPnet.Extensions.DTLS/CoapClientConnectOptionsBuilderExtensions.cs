using CoAPnet.Client;
using System;

namespace CoAPnet.Extensions.DTLS
{
    public static class CoapClientConnectOptionsBuilderExtensions
    {
        public static CoapClientConnectOptionsBuilder WithDtlsTransportLayer(this CoapClientConnectOptionsBuilder clientConnectOptionsBuilder, DtlsCoapTransportLayerOptions options)
        {
            if (clientConnectOptionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(clientConnectOptionsBuilder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            clientConnectOptionsBuilder.WithTransportLayer(() => new DtlsCoapTransportLayer
            {
                Credentials = options.Credentials
            });

            return clientConnectOptionsBuilder;
        }

        public static CoapClientConnectOptionsBuilder WithDtlsTransportLayer(this CoapClientConnectOptionsBuilder clientConnectOptionsBuilder, Action<DtlsCoapTransportLayerOptionsBuilder> options)
        {
            if (clientConnectOptionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(clientConnectOptionsBuilder));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var builder = new DtlsCoapTransportLayerOptionsBuilder();
            options.Invoke(builder);
            return clientConnectOptionsBuilder.WithDtlsTransportLayer(builder.Build()).WithEncryptedPort();
        }
    }
}
