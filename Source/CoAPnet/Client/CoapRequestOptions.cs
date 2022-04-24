using System.Collections.Generic;

namespace CoAPnet.Client
{
    public sealed class CoapRequestOptions
    {
        /// <summary>
        /// This is only required when accessing virtual servers.
        /// </summary>
        public string UriHost
        {
            get; set;
        }

        /// <summary>
        /// This is only required when accessing virtual servers.
        /// </summary>
        public int? UriPort
        {
            get; set;
        }

        public string UriPath
        {
            get; set;
        }

        public ICollection<string> UriQuery
        {
            get; set;
        }
    }
}
