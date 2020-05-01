using System;

namespace CoAPnet.Exceptions
{
    public class CoAPProtocolViolationException : Exception
    {
        public CoAPProtocolViolationException(string message) : base(message)
        {
        }
    }
}