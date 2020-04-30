using CoAPnet.Transport;
using System.Net.Sockets;

namespace CoAPnet.Client
{
    public class CoapClientConnectOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public AddressFamily? AddressFamily { get; set; }

        public ICoapTransportLayer TransportLayer { get; set; }
    }
}
