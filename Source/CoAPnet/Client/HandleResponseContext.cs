namespace CoAPnet.Client
{
    public class HandleResponseContext
    {
        public uint SequenceNumber { get; set; }

        public CoapResponse Response { get; set; }
    }
}
