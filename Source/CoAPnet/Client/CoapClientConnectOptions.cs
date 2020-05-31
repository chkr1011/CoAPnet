using CoAPnet.Protocol;
using CoAPnet.Transport;
using System;

namespace CoAPnet.Client
{
    public class CoapClientConnectOptions
    {
        public string Host
        {
            get; set;
        }

        public int Port { get; set; } = CoapDefaultPort.Unencrypted;

        public TimeSpan CommunicationTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public Func<ICoapTransportLayer> TransportLayerFactory { get; set; } = () => new UdpCoapTransportLayer();
    }
}
