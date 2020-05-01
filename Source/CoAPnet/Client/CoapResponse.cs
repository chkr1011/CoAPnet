using System;

namespace CoAPnet.Client
{
    public class CoapResponse
    {
        public CoapResponseStatusCode StatusCode { get; set; }

        public CoapResposeOptions Options { get; set; }

        public ArraySegment<byte> Payload { get; set; }
    }
}

