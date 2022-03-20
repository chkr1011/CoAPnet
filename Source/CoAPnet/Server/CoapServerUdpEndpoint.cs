using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Server
{
    public sealed class CoapServerUdpEndpoint : ICoapServerEndpoint
    {
        readonly byte[] _buffer = new byte[650000];

        readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public int Port { get; set; } = 5643;

        public IPAddress IpAddress { get; set; } = IPAddress.Any;

        public Task BindAsync(CancellationToken cancellationToken)
        {
            _socket.Bind(new IPEndPoint(IpAddress, Port));
            return Task.FromResult(0);
        }

        public Task<ICoapServerClient> AcceptAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            EndPoint ipEndPoint = new IPEndPoint(IpAddress, 0);
            var datagramSize = _socket.ReceiveFrom(_buffer, SocketFlags.None, ref ipEndPoint);

            cancellationToken.ThrowIfCancellationRequested();

            var datagram = new byte[datagramSize];
            Array.Copy(_buffer, 0, datagram, 0, datagramSize);

            var client = new CoapServerUdpEndpointClient();

            return Task.FromResult((ICoapServerClient)client);
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}