namespace CoAPnet.Client
{
    public sealed class HandleResponseContext
    {
        public uint SequenceNumber { get; set; }

        public CoapResponse Response { get; set; }
    }
}
