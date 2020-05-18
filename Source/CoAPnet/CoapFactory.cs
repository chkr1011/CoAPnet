using CoAPnet.Client;
using CoAPnet.Logging;
using CoAPnet.LowLevelClient;
using System;

namespace CoAPnet
{
    public class CoapFactory
    {
        public CoapNetLogger DefaultLogger { get; } = new CoapNetLogger();

        public ILowLevelCoapClient CreateLowLevelClient()
        {
            return new LowLevelCoapClient(DefaultLogger);
        }

        public ILowLevelCoapClient CreateLowLevelClient(CoapNetLogger logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return new LowLevelCoapClient(logger);
        }

        public ICoapClient CreateClient()
        {
            return new CoapClient(DefaultLogger);
        }

        public ICoapClient CreateClient(CoapNetLogger logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return new CoapClient(logger);
        }
    }
}
