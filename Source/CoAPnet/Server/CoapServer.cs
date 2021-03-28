using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CoAPnet.Protocol;

namespace CoAPnet.Server
{
    public interface ICoapServer : IDisposable
    {
        Task StartAsync();

        Task StopAsync();
    }

    public sealed class CoapServer : ICoapServer
    {
        public ICoapRequestHandler RequestHandler { get; set; }

        public List<ICoapServerEndpoint> Endpoints { get; } = new List<ICoapServerEndpoint>();

        CancellationTokenSource _cancellationTokenSource;

        public Task StartAsync()
        {
            return Task.FromResult(0);
        }

        public Task StopAsync()
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }

    public interface ICoapRequestHandler
    {
        Task HandleCoapRequest(CoapRequestContext context, CancellationToken cancellationToken);
    }

    public class CoapRequestContext
    {
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public ICoapServerClient Client { get; set; }

        public CoapMessage Request { get; set; }

        public CoapMessage Response { get; set; }

        public bool CloseConnection { get; set; }
    }

    public interface ICoapServerEndpoint : IDisposable
    {
        Task BindAsync(CancellationToken cancellationToken);

        Task<ICoapServerClient> AcceptAsync(CancellationToken cancellationToken);
    }

    public interface ICoapServerClient : IDisposable
    {
        Task<CoapMessage> ReceiveAsync(CancellationToken cancellationToken);

        Task SendAsync(CancellationToken cancellationToken);
    }

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

    public sealed class CoapServerUdpEndpointClient : ICoapServerClient
    {
        public CoapServerUdpEndpointClient()
        {
            
        }

        public Task<CoapMessage> ReceiveAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
