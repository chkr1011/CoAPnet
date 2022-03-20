using System.Collections.Generic;
using CoAPnet.Protocol;

namespace CoAPnet.Server
{
    public class CoapRequestContext
    {
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public ICoapServerClient Client { get; set; }

        public CoapMessage Request { get; set; }

        public CoapMessage Response { get; set; }

        public bool CloseConnection { get; set; }
    }
}