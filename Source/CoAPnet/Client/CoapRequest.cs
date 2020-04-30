using System;
using System.Collections.Generic;

namespace CoAPnet.Client
{
    public class CoapRequest
    {
        public CoapRequestMethod Method { get; set; }

        /// <summary>
        /// This is only required when accessing virtual servers.
        /// </summary>
        public string UriHost { get; set; }

        /// <summary>
        /// This is only required when accessing virtual servers.
        /// </summary>
        public int? UriPort { get; set; }

        public string UriPath { get; set; }

        public List<string> UriQuery { get; set; }

        public ArraySegment<byte> Payload { get; set; }
    }
}

