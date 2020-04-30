using CoAPnet.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Transport
{
    public sealed class UdpCoapTransportLayer : ICoapTransportLayer
    {
        UdpClient _udpClient;
        CoapClientConnectOptions _connectOptions;

        public Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _connectOptions = options;

            Dispose();

            // ! Match the local address family with the address family of the host!
            if (IPAddress.TryParse(options.Host, out var ipAddress))
            {
                _udpClient = new UdpClient(0, ipAddress.AddressFamily);
            }
            else
            {
                _udpClient = new UdpClient(0, options.AddressFamily ?? AddressFamily.InterNetwork);
            }

            return Task.FromResult(0);
        }

        public async Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            // TODO: Replace with proper async implementation.
            // The Tas.Run is required to let the current thread
            // wait for messages in the UDP client.

            var receiveResult = await _udpClient.ReceiveAsync().ConfigureAwait(false);
            Array.Copy(receiveResult.Buffer, 0, buffer.Array, buffer.Offset, receiveResult.Buffer.Length);

            return receiveResult.Buffer.Length;
        }

        public Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            ThrowIfNotConnected();

            return _udpClient.SendAsync(buffer.Array, buffer.Count, _connectOptions.Host, _connectOptions.Port);
        }

        public void Dispose()
        {
#if NETSTANDARD1_3 || NETSTANDARD2_0
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
