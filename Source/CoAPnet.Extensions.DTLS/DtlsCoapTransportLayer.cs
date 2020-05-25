using CoAPnet.Transport;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class DtlsCoapTransportLayer : ICoapTransportLayer
    {
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

        public Task ConnectAsync(CoapTransportLayerConnectOptions connectOptions, CancellationToken cancellationToken)
        {
            try
            {
                _udpTransport = new UdpTransport(connectOptions);

                var clientProtocol = new DtlsClientProtocol(new SecureRandom());
                _dtlsClient = new DtlsClient(ConvertProtocolVersion(DtlsVersion), (PreSharedKey)Credentials);
                _dtlsTransport = clientProtocol.Connect(_dtlsClient, _udpTransport);
            }
            catch
            {
                if (_dtlsClient.ReceivedAlert != 0)
                {
                    throw new DtlsException($"Received alert {AlertDescription.GetText(_dtlsClient.ReceivedAlert)}.", null)
                    {
                        ReceivedAlert = _dtlsClient.ReceivedAlert
                    };
                }

                throw;
            }

            return Task.FromResult(0);
        }

        public Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            int received;
            do
            {
                received = _dtlsTransport.Receive(buffer.Array, buffer.Offset, buffer.Count, 100);
            }
            while (received == 0 && !cancellationToken.IsCancellationRequested);

            return Task.FromResult(received);
        }

        public Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
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
