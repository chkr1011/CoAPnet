using CoAPnet.Protocol.Options;

namespace CoAPnet.Client
{
    public sealed class CoapResponseOptions
    {
        public CoapMessageContentFormat? ContentFormat
        {
            get; set;
        }

        public int MaxAge
        {
            get; set;
        }

        public byte[] ETag
        {
            get; set;
        }
    }
}

