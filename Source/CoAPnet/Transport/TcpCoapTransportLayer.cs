using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Transport
{
    public sealed class TcpCoapTransportLayer : ICoapTransportLayer
    {
        TcpClient _tcpClient;
        NetworkStream _networkStream;

        public async Task ConnectAsync(CoapTransportLayerConnectOptions options, CancellationToken cancellationToken)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Dispose();

            _tcpClient = new TcpClient();
            using (cancellationToken.Register(Dispose))
            {
                await _tcpClient.ConnectAsync(options.EndPoint.Address, options.EndPoint.Port).ConfigureAwait(false);
                _networkStream = _tcpClient.GetStream();
            }
        }

        public void Dispose()
        {
#if NETSTANDARD1_3 || NETSTANDARD2_0 || NET5_0
            _tcpClient?.Dispose();
#else
            _tcpClient?.Close();
#endif
            
            _networkStream?.Dispose();
        }

        public Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            return _networkStream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count, cancellationToken);
        }

        public Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            return _networkStream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, cancellationToken);
        }
    }
}
