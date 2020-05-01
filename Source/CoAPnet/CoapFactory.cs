using CoAPnet.Client;
using CoAPnet.LowLevelClient;

namespace CoAPnet
{
    public class CoapFactory
    {
        public ILowLevelCoapClient CreateLowLevelClient()
        {
            return new LowLevelCoapClient();
        }

        public ICoapClient CreateClient()
        {
            return new CoapClient();
        }
    }
}
