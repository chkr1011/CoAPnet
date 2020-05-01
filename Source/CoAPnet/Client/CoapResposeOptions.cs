using CoAPnet.Protocol;

namespace CoAPnet.Client
{
    public class CoapResposeOptions
    {
        public CoapMessageContentFormat? ContentFormat { get; set; }

        public int? MaxAge { get; set; }
    }
}

