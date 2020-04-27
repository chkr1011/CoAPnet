using System;

namespace CoapTest
{
    public class CoAPProtocolViolationException : Exception
    {
        public CoAPProtocolViolationException(string message) : base(message)
        {

        }
    }
}