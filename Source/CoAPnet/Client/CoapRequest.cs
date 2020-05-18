using System;

namespace CoAPnet.Client
{
    public class CoapRequest
    {
        public CoapRequestMethod Method { get; set; } = CoapRequestMethod.Get;

        public CoapRequestOptions Options { get; set; } = new CoapRequestOptions();

        public ArraySegment<byte> Payload { get; set; }
    }
}

