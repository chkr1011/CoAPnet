using System;

namespace CoAPnet.Exceptions
{
    public class CoapProtocolViolationException : Exception
    {
        public CoapProtocolViolationException(string message) : base(message)
        {
        }
    }
}