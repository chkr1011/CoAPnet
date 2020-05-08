using System;

namespace CoAPnet.Client
{
    public class CoapResponse
    {
        public CoapResponseStatusCode StatusCode
        {
            get; set;
        }

        public CoapResponseOptions Options
        {
            get; set;
        }

        public byte[] Payload
        {
            get; set;
        }
    }
}

