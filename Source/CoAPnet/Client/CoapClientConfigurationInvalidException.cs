using System;

namespace CoAPnet.Client
{
    public class CoapClientConfigurationInvalidException : Exception
    {
        public CoapClientConfigurationInvalidException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
