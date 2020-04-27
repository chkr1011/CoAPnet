using System;

namespace CoAPnet.Client
{
    public class CoapClientConnectOptionsBuilder
    {
        readonly CoapClientConnectOptions _options = new CoapClientConnectOptions();

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

        public CoapClientConnectOptions Build()
        {
            return _options;
        }
    }
}
