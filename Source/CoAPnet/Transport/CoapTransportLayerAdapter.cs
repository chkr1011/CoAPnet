using CoAPnet.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Transport
{
    public sealed class CoapTransportLayerAdapter : IDisposable
    {
        readonly ICoapTransportLayer _transportLayer;
        readonly CoapNetLogger _logger;

        public CoapTransportLayerAdapter(ICoapTransportLayer transportLayer, CoapNetLogger logger)
        {
            _transportLayer = transportLayer ?? throw new ArgumentNullException(nameof(transportLayer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task ConnectAsync(CoapTransportLayerConnectOptions connectOptions, CancellationToken cancellationToken)
        {
            _logger.Information(nameof(CoapTransportLayerAdapter), $"Connecting with '{connectOptions.EndPoint}'...");
            return _transportLayer.ConnectAsync(connectOptions, cancellationToken);
        }

        public Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            _logger.Trace(nameof(CoapTransportLayerAdapter), $"Sending {buffer.Count} bytes...");
            return _transportLayer.SendAsync(buffer, cancellationToken);
        }

        public async Task<int> ReceiveAsync(ArraySegment<byte> receiveBuffer, CancellationToken cancellationToken)
        {
            var receivedBytes = await _transportLayer.ReceiveAsync(receiveBuffer, cancellationToken).ConfigureAwait(false);

            _logger.Trace(nameof(CoapTransportLayerAdapter), $"Received {receivedBytes} bytes...");

            return receivedBytes;
        }

        public void Dispose()
        {
            _transportLayer?.Dispose();
        }
    }
}
