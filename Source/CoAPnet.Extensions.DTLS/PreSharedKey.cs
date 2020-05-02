namespace CoAPnet.Extensions.DTLS
{
    public class PreSharedKey : IDtlsCredentials
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
