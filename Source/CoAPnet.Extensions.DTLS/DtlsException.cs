using System;
using CoAPnet.Exceptions;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class DtlsException : CoapCommunicationException
    {
        public DtlsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public byte ReceivedAlert { get; set; }
    }
}
