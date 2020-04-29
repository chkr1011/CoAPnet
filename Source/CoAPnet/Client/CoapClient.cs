using CoAPnet.Protocol.Encoding;
using CoAPnet.Transport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    // TODO: Add ICoapClient interface.
    public sealed class CoapClient : IDisposable
    {
        readonly CoapMessageIdProvider _messageIdProvider = new CoapMessageIdProvider();
        readonly CoapMessageEncoder _messageEncoder = new CoapMessageEncoder();
        readonly CoapMessageDecoder _messageDecoder = new CoapMessageDecoder();
        readonly CoapRequestToMessageConverter _requestToMessageConverter = new CoapRequestToMessageConverter();
        readonly CoapMessageToResponseConverter _messageToResponseConverter = new CoapMessageToResponseConverter();

        readonly ArraySegment<byte> _receiveBuffer = new ArraySegment<byte>(new byte[64000]);

        ICoapTransportLayer _transportLayer;
        CoapClientConnectOptions _connectOptions;

        public async Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _connectOptions = options;

            _transportLayer = options.TransportLayer;

            await _transportLayer.ConnectAsync(options, cancellationToken).ConfigureAwait(false);
        }

        public async Task<CoapResponse> Request(CoapRequest request, CancellationToken cancellationToken)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var requestMessage = _requestToMessageConverter.Convert(request);
            requestMessage.Id = _messageIdProvider.Next();

            var requestMessageBuffer = _messageEncoder.Encode(requestMessage);

            await _transportLayer.SendAsync(requestMessageBuffer, cancellationToken).ConfigureAwait(false);

            // TODO: Add proper reqest-response matching like in MQTTnet.

            await _transportLayer.ReceiveAsync(_receiveBuffer, cancellationToken).ConfigureAwait(false);

            var responseMessage = _messageDecoder.Decode(_receiveBuffer);

            return _messageToResponseConverter.Convert(responseMessage);
        }

        public void Dispose()
        {
            _transportLayer?.Dispose();
        }
    }
}

