using System;

namespace CoAPnet.Protocol
{
    public class CoAPProtocolViolationException : Exception
    {
        public CoAPProtocolViolationException(string message) : base(message)
        {

        }
    }
}