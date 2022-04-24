using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CoAPnet.Client;
using CoAPnet.Exceptions;
using CoAPnet.Logging;
using CoAPnet.Protocol;
using CoAPnet.Protocol.Encoding;
using CoAPnet.Transport;

namespace CoAPnet.LowLevelClient
{
    public sealed class LowLevelCoapClient : ILowLevelCoapClient
    {
        readonly CoapNetLogger _logger;
        readonly CoapMessageDecoder _messageDecoder;
        readonly CoapMessageEncoder _messageEncoder = new CoapMessageEncoder();

        // The size of the receive buffer is large enough so that a whole
        // UDP datagram will fit into the buffer at once.
        readonly ArraySegment<byte> _receiveBuffer = new ArraySegment<byte>(new byte[65535]);
        readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1, 1);

        CoapClientConnectOptions _connectOptions;
        CoapTransportLayerAdapter _transportLayerAdapter;

        public LowLevelCoapClient(CoapNetLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _messageDecoder = new CoapMessageDecoder(logger);
        }

        public async Task SendAsync(CoapMessage message, CancellationToken cancellationToken)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var requestMessageBuffer = _messageEncoder.Encode(message);

            await _syncRoot.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _transportLayerAdapter.SendAsync(requestMessageBuffer, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _syncRoot.Release();
            }
        }

        public async Task<CoapMessage> ReceiveAsync(CancellationToken cancellationToken)
        {
            var datagramLength = await _transportLayerAdapter.ReceiveAsync(_receiveBuffer, cancellationToken)
                .ConfigureAwait(false);

            if (datagramLength == 0)
            {
                return null;
            }

            Debug.Assert(_receiveBuffer.Array != null);
            return _messageDecoder.Decode(new ArraySegment<byte>(_receiveBuffer.Array, 0, datagramLength));
        }

        public void Dispose()
        {
            _transportLayerAdapter?.Dispose();
        }

        public async Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken)
        {
            _connectOptions = options ?? throw new ArgumentNullException(nameof(options));

            var transportLayer = options.TransportLayerFactory?.Invoke();

            if (transportLayer == null)
            {
                throw new InvalidOperationException("No CoAP transport layer is set.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            _transportLayerAdapter = new CoapTransportLayerAdapter(transportLayer, _logger);
            try
            {
                var transportLayerConnectOptions = new CoapTransportLayerConnectOptions
                {
                    EndPoint = await ResolveIpEndPoint(options).ConfigureAwait(false)
                };

                await _transportLayerAdapter.ConnectAsync(transportLayerConnectOptions, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                _transportLayerAdapter.Dispose();
                throw;
            }
        }

        async Task<IPEndPoint> ResolveIpEndPoint(CoapClientConnectOptions connectOptions)
        {
            if (IPAddress.TryParse(connectOptions.Host, out var ipAddress))
            {
                return new IPEndPoint(ipAddress, connectOptions.Port);
            }
#if NETSTANDARD1_3
                await Task.FromResult(0);
                throw new NotSupportedException("Resolving DNS end points is not supported for NETSTANDARD1_3. Please pass IP address instead.");
#else
            try
            {
                var hostIpAddresses = await Dns.GetHostAddressesAsync(connectOptions.Host).ConfigureAwait(false);
                if (hostIpAddresses.Length == 0)
                {
                    throw new CoapCommunicationException("Failed to resolve DNS end point", null);
                }

                // We only use the first address for now.
                return new IPEndPoint(hostIpAddresses[0], _connectOptions.Port);
            }
            catch (Exception exception)
            {
                throw new CoapCommunicationException("Error while resolving DNS name.", exception);
            }
#endif
        }
    }
}