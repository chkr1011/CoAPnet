using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Transport
{
    public sealed class UdpCoapTransportLayer : ICoapTransportLayer
    {
        CoapTransportLayerConnectOptions _connectOptions;
        UdpClient _udpClient;

        public Task ConnectAsync(CoapTransportLayerConnectOptions options, CancellationToken cancellationToken)
        {
            _connectOptions = options ?? throw new ArgumentNullException(nameof(options));

            Dispose();

            _udpClient = new UdpClient(0, options.EndPoint.AddressFamily);

            return Task.FromResult(0);
        }

        public async Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
#if NET6_0_OR_GREATER
            var receiveResult = await _udpClient.ReceiveAsync(cancellationToken).ConfigureAwait(false);
#else
            var receiveResult = await _udpClient.ReceiveAsync().ConfigureAwait(false);
#endif

            Array.Copy(receiveResult.Buffer, 0, buffer.Array, buffer.Offset, receiveResult.Buffer.Length);

            return receiveResult.Buffer.Length;
        }

        public Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            ThrowIfNotConnected();

            return _udpClient.SendAsync(buffer.Array, buffer.Count, _connectOptions.EndPoint);
        }

        public void Dispose()
        {
#if NET452
            _udpClient?.Close();
#else
            _udpClient?.Dispose();
#endif
        }

        void ThrowIfNotConnected()
        {
            if (_udpClient == null)
            {
                throw new InvalidOperationException("The CoAP transport layer is not connected.");
            }
        }
    }
}