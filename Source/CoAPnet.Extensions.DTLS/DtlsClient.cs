using Org.BouncyCastle.Crypto.Tls;
using System;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class DtlsClient : DefaultTlsClient
    {
        readonly PreSharedKey _preSharedKey;

        public DtlsClient(ProtocolVersion protocolVersion, PreSharedKey preSharedKey)
        {
            MinimumVersion = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));
            _preSharedKey = preSharedKey ?? throw new ArgumentNullException(nameof(preSharedKey));
        }

        public byte ReceivedAlert { get; private set; }

        public override ProtocolVersion MinimumVersion { get; }

        public override ProtocolVersion ClientVersion => MinimumVersion;

        public override int[] GetCipherSuites()
        {
            return new [] {
                CipherSuite.TLS_PSK_WITH_AES_128_CCM,
                CipherSuite.TLS_PSK_WITH_AES_128_CCM_8,
                CipherSuite.TLS_PSK_WITH_AES_256_CCM,
                CipherSuite.TLS_PSK_WITH_AES_256_CCM_8
            };
        }

        public override void NotifyAlertReceived(byte alertLevel, byte alertDescription)
        {
            ReceivedAlert = alertDescription;

            base.NotifyAlertReceived(alertLevel, alertDescription);
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
    }
}
