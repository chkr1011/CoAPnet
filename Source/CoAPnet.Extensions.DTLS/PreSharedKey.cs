namespace CoAPnet.Extensions.DTLS
{
    public sealed class PreSharedKey : IDtlsCredentials
    {
        public byte[] Identity
        {
            get; set;
        }

        public byte[] Key
        {
            get; set;
        }
    }
}
