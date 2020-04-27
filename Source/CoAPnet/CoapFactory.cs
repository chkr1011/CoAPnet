using CoAPnet.Client;

namespace CoAPnet
{
    public class CoapFactory
    {
        public CoapClient CreateClient()
        {
            return new CoapClient();
        }
    }
}
