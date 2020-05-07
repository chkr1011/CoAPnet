using Org.BouncyCastle.Crypto.Tls;
using System;

namespace CoAPnet.Extensions.DTLS
{
    public class PreSharedKeyWrapper : TlsPskIdentity
    {
        readonly PreSharedKey _preSharedKey;

        public PreSharedKeyWrapper(PreSharedKey preSharedKey)
        {
            _preSharedKey = preSharedKey ?? throw new ArgumentNullException(nameof(preSharedKey));
        }

        public byte[] GetPsk()
        {
            return _preSharedKey.Key;
        }

        public byte[] GetPskIdentity()
        {
            return _preSharedKey.Identity;
        }

        public void NotifyIdentityHint(byte[] psk_identity_hint)
        {
        }

        public void SkipIdentityHint()
        {
        }
    }
}
