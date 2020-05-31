using CoAPnet.Protocol;
using CoAPnet.Transport;
using System;

namespace CoAPnet.Client
{
    public class CoapClientConnectOptionsBuilder
    {
        readonly CoapClientConnectOptions _options = new CoapClientConnectOptions
        {
            TransportLayerFactory = () => new UdpCoapTransportLayer() // This is the protocols default transport.
        };

        public CoapClientConnectOptionsBuilder WithHost(string value)
        {
            _options.Host = value ?? throw new ArgumentNullException(nameof(value));
            return this;
        }

        public CoapClientConnectOptionsBuilder WithPort(int value)
        {
            _options.Port = value;
            return this;
        }

        public CoapClientConnectOptionsBuilder WithEncryptedPort()
        {
            return WithPort(CoapDefaultPort.Encrypted);
        }

        public CoapClientConnectOptionsBuilder WithUnencryptedPort()
        {
            return WithPort(CoapDefaultPort.Unencrypted);
        }

        public CoapClientConnectOptionsBuilder WithTcpTransportLayer()
        {
            _options.TransportLayerFactory = () => new TcpCoapTransportLayer();
            return this;
        }

        public CoapClientConnectOptionsBuilder WithTransportLayer(Func<ICoapTransportLayer> transportLayerFactory)
        {
            _options.TransportLayerFactory = transportLayerFactory ?? throw new ArgumentNullException(nameof(transportLayerFactory));
            return this;
        }

        public CoapClientConnectOptionsBuilder WithUdpTransportLayer()
        {
            _options.TransportLayerFactory = () => new UdpCoapTransportLayer();
            return this;
        }

        public CoapClientConnectOptions Build()
        {
            if (_options.TransportLayerFactory == null)
            {
                throw new CoapClientConfigurationInvalidException("Transport layer is not set.", null);
            }

            return _options;
        }
    }
}