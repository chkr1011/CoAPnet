using CoAPnet.Transport;
using System;

namespace CoAPnet.Client
{
    public class CoapClientConnectOptionsBuilder
    {
        readonly CoapClientConnectOptions _options = new CoapClientConnectOptions
        {
            TransportLayer = new UdpCoapTransportLayer() // This is the protocols default transport.
        };

        public CoapClientConnectOptionsBuilder WithHost(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            _options.Host = value;
            return this;
        }

        public CoapClientConnectOptionsBuilder WithPort(int value)
        {
            _options.Port = value;
            return this;
        }

        public CoapClientConnectOptionsBuilder WithTransportLayer(ICoapTransportLayer value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            _options.TransportLayer = value;
            return this;
        }

        public CoapClientConnectOptionsBuilder WithUdpTransportLayer()
        {
            _options.TransportLayer = new UdpCoapTransportLayer();
            return this;
        }

        public CoapClientConnectOptionsBuilder WithTcpTransportLayer()
        {
            _options.TransportLayer = new TcpCoapTransportLayer();
            return this;
        }

        public CoapClientConnectOptions Build()
        {
            if (_options.TransportLayer == null)
            {
                throw new CoapClientConfigurationInvalidException("Transport layer is not set.", null);
            }

            return _options;
        }
    }
}
