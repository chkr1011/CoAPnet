namespace CoAPnet.Extensions.DTLS
{
    public sealed class DtlsCoapTransportLayerOptions
    {
        public IDtlsCredentials Credentials
        {
            get;
            set;
        }

        public DtlsVersion DtlsVersion
        {
            get;
            set;
        } = DtlsVersion.V1_2;
    }
}
