using CoAPnet.Transport;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using System;
using System.Threading;
using System.Threading.Tasks;
using CoAPnet.Internal;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class DtlsCoapTransportLayer : ICoapTransportLayer
    {
        readonly SecureRandom _secureRandom = new SecureRandom();

        UdpTransport _udpTransport;
        DtlsTransport _dtlsTransport;
        DtlsClient _dtlsClient;

        public IDtlsCredentials Credentials
        {
            get; set;
        }

        public DtlsVersion DtlsVersion
        {
            get; set;
        } = DtlsVersion.V1_2;

        public async Task ConnectAsync(CoapTransportLayerConnectOptions connectOptions, CancellationToken cancellationToken)
        {
            if (connectOptions == null)
            {
                throw new ArgumentNullException(nameof(connectOptions));
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _udpTransport = new UdpTransport(connectOptions);
                var clientProtocol = new DtlsClientProtocol(_secureRandom);
                _dtlsClient = new DtlsClient(ConvertProtocolVersion(DtlsVersion), (PreSharedKey)Credentials);

                using (cancellationToken.Register(() =>
                {
                    _udpTransport.Close();
                }))
                {
                    _dtlsTransport = await Task.Run(() => clientProtocol.Connect(_dtlsClient, _udpTransport), cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                _udpTransport?.Dispose();

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                if (_dtlsClient.ReceivedAlert != 0)
                {
                    throw new DtlsException($"Received alert {AlertDescription.GetText(_dtlsClient.ReceivedAlert)}.", null)
                    {
                        ReceivedAlert = _dtlsClient.ReceivedAlert
                    };
                }

                throw;
            }
        }

        public Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (cancellationToken.Register(() => _udpTransport.Close()))
            {
                var received = _dtlsTransport.Receive(buffer.Array, buffer.Offset, buffer.Count, 0);
                return Task.FromResult(received);
            }
        }

        public Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _dtlsTransport.Send(buffer.Array, buffer.Offset, buffer.Count);
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            _dtlsTransport?.Close();
            _udpTransport?.Dispose();
        }

        static ProtocolVersion ConvertProtocolVersion(DtlsVersion dtlsVersion)
        {
            if (dtlsVersion == DtlsVersion.V1_0)
            {
                return ProtocolVersion.DTLSv10;
            }

            if (dtlsVersion == DtlsVersion.V1_2)
            {
                return ProtocolVersion.DTLSv12;
            }

            throw new NotSupportedException();
        }
    }
}
