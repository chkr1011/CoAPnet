using System;
using System.Text;

namespace CoAPnet.Extensions.DTLS
{
    public class DtlsCoapTransportLayerBuilder
    {
        readonly DtlsCoapTransportLayer _transportLayer = new DtlsCoapTransportLayer();

        public DtlsCoapTransportLayerBuilder WithPreSharedKey(byte[] identity, byte[] key)
        {
            if (identity is null) throw new ArgumentNullException(nameof(identity));
            if (key is null) throw new ArgumentNullException(nameof(key));

            _transportLayer.Credentials = new PreSharedKey
            {
                Identity = identity,
                Key = key
            };

            return this;
        }

        public DtlsCoapTransportLayerBuilder WithPreSharedKey(string identity, byte[] key)
        {
            if (identity is null) throw new ArgumentNullException(nameof(identity));

            return WithPreSharedKey(Encoding.UTF8.GetBytes(identity), key);
        }

        public DtlsCoapTransportLayerBuilder WithPreSharedKey(string identity, string key)
        {
            if (identity is null) throw new ArgumentNullException(nameof(identity));

            return WithPreSharedKey(Encoding.UTF8.GetBytes(identity), Encoding.UTF8.GetBytes(key));
        }

        public DtlsCoapTransportLayer Build()
        {
            return _transportLayer;
        }
    }
}
