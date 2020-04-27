namespace CoAPnet.Client
{
    public class CoapClientConnectOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public ICoapTransportLayer TransportLayer { get; set; }
    }
}
