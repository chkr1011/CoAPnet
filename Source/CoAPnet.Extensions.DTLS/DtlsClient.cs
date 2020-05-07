using Org.BouncyCastle.Crypto.Tls;

namespace CoAPnet.Extensions.DTLS
{
    public class DtlsClient : DefaultTlsClient
    {
        readonly PreSharedKey _preSharedKey;

        public DtlsClient(PreSharedKey preSharedKey)
        {
            _preSharedKey = preSharedKey;
        }

        public override ProtocolVersion MinimumVersion => ProtocolVersion.DTLSv12;

        public override ProtocolVersion ClientVersion => ProtocolVersion.DTLSv12;

        public override int[] GetCipherSuites()
        {
            return new int[] {
                CipherSuite.TLS_PSK_WITH_AES_128_CCM,
                CipherSuite.TLS_PSK_WITH_AES_128_CCM_8,
                CipherSuite.TLS_PSK_WITH_AES_256_CCM,
                CipherSuite.TLS_PSK_WITH_AES_256_CCM_8,
            };
        }

        public override void NotifySecureRenegotiation(bool secureRenegotiation)
        {
            // This must be overridden. Otherwise we get "handshake_failure(40)".
        }

        public override TlsKeyExchange GetKeyExchange()
        {
            var keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(mSelectedCipherSuite);

            switch (keyExchangeAlgorithm)
            {
                case KeyExchangeAlgorithm.PSK:
                case KeyExchangeAlgorithm.RSA_PSK:
                case KeyExchangeAlgorithm.DHE_PSK:
                case KeyExchangeAlgorithm.ECDHE_PSK:
                    {
                        var psk = new PreSharedKeyWrapper(_preSharedKey);

                        return new TlsPskKeyExchange(
                            keyExchangeAlgorithm,
                            mSupportedSignatureAlgorithms,
                            psk,
                            null,
                            null,
                            null,
                            mNamedCurves,
                            mClientECPointFormats,
                            mServerECPointFormats);
                    }

                default:
                    {
                        throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                    }
            }
        }

        public override TlsAuthentication GetAuthentication()
        {
            return null;
        }

        private class PreSharedKeyWrapper : TlsPskIdentity
        {
            readonly PreSharedKey _preSharedKey;

            public PreSharedKeyWrapper(PreSharedKey preSharedKey)
            {
                _preSharedKey = preSharedKey ?? throw new System.ArgumentNullException(nameof(preSharedKey));
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
}
