using CoAPnet.Internal;
using CoAPnet.Logging;
using CoAPnet.LowLevelClient;
using CoAPnet.MessageDispatcher;
using CoAPnet.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public sealed class CoapClient : ICoapClient
    {
        readonly CoapRequestToMessageConverter _requestToMessageConverter = new CoapRequestToMessageConverter();
        readonly CoapMessageToResponseConverter _messageToResponseConverter = new CoapMessageToResponseConverter();
        readonly CoapMessageDispatcher _messageDispatcher = new CoapMessageDispatcher();
        readonly CoapMessageIdProvider _messageIdProvider = new CoapMessageIdProvider();
        readonly CoapNetLogger _logger;

        LowLevelCoapClient _lowLevelClient;
        CoapClientConnectOptions _connectOptions;
        CancellationTokenSource _cancellationToken;

        public CoapClient(CoapNetLogger logger)
        {
            _logger = logger;
        }

        public async Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _connectOptions = options;

            _lowLevelClient?.Dispose();

            _lowLevelClient = new LowLevelCoapClient(_logger);

            await _lowLevelClient.ConnectAsync(options, cancellationToken).ConfigureAwait(false);
            _cancellationToken = new CancellationTokenSource();

            ParallelTask.Run(() => ReceiveMessages(_cancellationToken.Token), _cancellationToken.Token);
        }

        public async Task<CoapResponse> RequestAsync(CoapRequest request, CancellationToken cancellationToken)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var requestMessage = _requestToMessageConverter.Convert(request);

            var responseMessage = await RequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            var payload = responseMessage.Payload;
            if (CoapClientBlockTransferReceiver.IsBlockTransfer(responseMessage))
            {
                payload = await new CoapClientBlockTransferReceiver(requestMessage, responseMessage, this, _logger).ReceiveFullPayload(cancellationToken).ConfigureAwait(false);
            }

            return _messageToResponseConverter.Convert(responseMessage, payload);
        }

        internal async Task<CoapMessage> RequestAsync(CoapMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage is null) throw new ArgumentNullException(nameof(requestMessage));

            requestMessage.Id = _messageIdProvider.Next();

            var responseAwaiter = _messageDispatcher.AddAwaiter(requestMessage.Id);
            try
            {
                await _lowLevelClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

                return await responseAwaiter.WaitOneAsync(_connectOptions.CommunicationTimeout).ConfigureAwait(false);
            }
            finally
            {
                _messageDispatcher.RemoveAwaiter(requestMessage.Id);
            }
        }

        public void Dispose()
        {
            _lowLevelClient?.Dispose();
        }

        async Task ReceiveMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var message = await _lowLevelClient.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                    _messageDispatcher.Dispatch(message);
                }
                catch (Exception)
                {
                    // TODO: Add logging
                }
            }
        }
    }
}

