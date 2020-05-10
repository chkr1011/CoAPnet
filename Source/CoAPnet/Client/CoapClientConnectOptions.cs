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

        public int Port { get; set; } = CoapDefaultPort.Unencrpyted;

        public TimeSpan CommunicationTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public ICoapTransportLayer TransportLayer
        {
            get; set;
        }
    }
}
