using CoAPnet.Exceptions;
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

        public async Task ConnectAsync(CoapTransportLayerConnectOptions connectOptions, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information(nameof(CoapTransportLayerAdapter), $"Connecting with '{connectOptions.EndPoint}'...");
                await _transportLayer.ConnectAsync(connectOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new CoapCommunicationException("Error while connecting with CoAP server.", exception);
            }
        }

        public async Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Trace(nameof(CoapTransportLayerAdapter), $"Sending {buffer.Count} bytes...");
                await _transportLayer.SendAsync(buffer, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new CoapCommunicationException("Error while sending CoAP message.", exception);
            }
        }

        public async Task<int> ReceiveAsync(ArraySegment<byte> receiveBuffer, CancellationToken cancellationToken)
        {
            try
            {
                var receivedBytes = await _transportLayer.ReceiveAsync(receiveBuffer, cancellationToken).ConfigureAwait(false);

                _logger.Trace(nameof(CoapTransportLayerAdapter), $"Received {receivedBytes} bytes...");

                return receivedBytes;
            }
            catch (Exception exception)
            {
                throw new CoapCommunicationException("Error receiving CoAP messages.", exception);
            }
        }

        public void Dispose()
        {
            _transportLayer?.Dispose();
        }
    }
}
