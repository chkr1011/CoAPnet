using System;
using System.Text;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class DtlsCoapTransportLayerOptionsBuilder
    {
        readonly DtlsCoapTransportLayerOptions _options = new DtlsCoapTransportLayerOptions();

        public DtlsCoapTransportLayerOptionsBuilder WithDtlsVersion(DtlsVersion version)
        {
            _options.DtlsVersion = version;
            return this;
        }
        
        public DtlsCoapTransportLayerOptionsBuilder WithPreSharedKey(byte[] identity, byte[] key)
        {
            if (identity is null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _options.Credentials = new PreSharedKey
            {
                Identity = identity,
                Key = key
            };

            return this;
        }

        public DtlsCoapTransportLayerOptionsBuilder WithPreSharedKey(string identity, byte[] key)
        {
            if (identity is null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return WithPreSharedKey(Encoding.UTF8.GetBytes(identity), key);
        }

        public DtlsCoapTransportLayerOptionsBuilder WithPreSharedKey(string identity, string key)
        {
            if (identity is null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return WithPreSharedKey(Encoding.UTF8.GetBytes(identity), Encoding.UTF8.GetBytes(key));
        }

        public DtlsCoapTransportLayerOptions Build()
        {
            return _options;
        }
    }
}
