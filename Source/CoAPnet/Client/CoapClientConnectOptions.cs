using CoAPnet.Transport;

namespace CoAPnet.Client
{
    public class CoapClientConnectOptions
    {
        public string Host
        {
            get; set;
        }

        public int Port { get; set; } = 5683; // Default IANA port.

        public ICoapTransportLayer TransportLayer
        {
            get; set;
        }
    }
}
