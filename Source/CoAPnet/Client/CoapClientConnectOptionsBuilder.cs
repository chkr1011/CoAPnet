using CoAPnet.Transport;
using System;

namespace CoAPnet.Client
{
    public class CoapClientConnectOptionsBuilder
    {
        private readonly CoapClientConnectOptions _options = new CoapClientConnectOptions
        {
            TransportLayer = new UdpCoapTransportLayer() // This is the protocols default transport.
        };

        public CoapClientConnectOptions Build()
        {
            if (_options.TransportLayer == null)
            {
                throw new CoapClientConfigurationInvalidException("Transport layer is not set.", null);
            }

            return _options;
        }

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

        public CoapClientConnectOptionsBuilder WithTcpTransportLayer()
        {
            _options.TransportLayer = new TcpCoapTransportLayer();
            return this;
        }

        public CoapClientConnectOptionsBuilder WithTransportLayer(ICoapTransportLayer value)
        {
            _options.TransportLayer = value ?? throw new ArgumentNullException(nameof(value));
            return this;
        }

        public CoapClientConnectOptionsBuilder WithUdpTransportLayer()
        {
            _options.TransportLayer = new UdpCoapTransportLayer();
            return this;
        }
    }
}