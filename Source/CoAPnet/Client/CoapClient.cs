using CoAPnet.Internal;
using CoAPnet.Logging;
using CoAPnet.LowLevelClient;
using CoAPnet.MessageDispatcher;
using CoAPnet.Protocol;
using CoAPnet.Protocol.Observe;
using CoAPnet.Protocol.Options;
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
        readonly CoapMessageTokenProvider _messageTokenProvider = new CoapMessageTokenProvider();

        readonly CoapNetLogger _logger;
        readonly CoapClientObservationManager _observationManager;
        readonly LowLevelCoapClient _lowLevelClient;

        CoapClientConnectOptions _connectOptions;
        CancellationTokenSource _cancellationToken;

        public CoapClient(CoapNetLogger logger)
        {
            _logger = logger;

            _lowLevelClient = new LowLevelCoapClient(_logger);
            _observationManager = new CoapClientObservationManager(_messageToResponseConverter, _lowLevelClient, _logger);
        }

        public async Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken)
        {
            _connectOptions = options ?? throw new ArgumentNullException(nameof(options));

            await _lowLevelClient.ConnectAsync(options, cancellationToken).ConfigureAwait(false);
            
            _cancellationToken = new CancellationTokenSource();
            ParallelTask.StartLongRunning(() => ReceiveMessages(_cancellationToken.Token), _cancellationToken.Token);
        }

        public async Task<CoapResponse> RequestAsync(CoapRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestMessage = _requestToMessageConverter.Convert(request);

            var responseMessage = await RequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            var payload = responseMessage.Payload;
            if (CoapClientBlockTransferReceiver.IsBlockTransfer(responseMessage))
            {
                payload = await new CoapClientBlockTransferReceiver(requestMessage, responseMessage, this, _logger).ReceiveFullPayload(cancellationToken).ConfigureAwait(false);
            }

            return _messageToResponseConverter.Convert(responseMessage, payload);
        }

        public async Task<CoapObserveResponse> ObserveAsync(CoapObserveOptions options, CancellationToken cancellationToken)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var request = new CoapRequest
            {
                Method = CoapRequestMethod.Get,
                Options = options.Request.Options,
            };

            var token = _messageTokenProvider.Next();

            var requestMessage = _requestToMessageConverter.Convert(request);
            requestMessage.Token = token.Value;
            requestMessage.Options.Add(new CoapMessageOptionFactory().CreateObserve(CoapObserveOptionValue.Register));

            var responseMessage = await RequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            var payload = responseMessage.Payload;
            if (CoapClientBlockTransferReceiver.IsBlockTransfer(responseMessage))
            {
                payload = await new CoapClientBlockTransferReceiver(requestMessage, responseMessage, this, _logger).ReceiveFullPayload(cancellationToken).ConfigureAwait(false);
            }

            _observationManager.Register(token, options.ResponseHandler);

            var response = _messageToResponseConverter.Convert(responseMessage, payload);
            return new CoapObserveResponse(response, this)
            {
                Token = token,
                Request = request
            };
        }

        public async Task StopObservationAsync(CoapObserveResponse observeResponse, CancellationToken cancellationToken)
        {
            if (observeResponse is null)
            {
                throw new ArgumentNullException(nameof(observeResponse));
            }

            var requestMessage = _requestToMessageConverter.Convert(observeResponse.Request);
            requestMessage.Token = observeResponse.Token.Value;

            requestMessage.Options.RemoveAll(o => o.Number == CoapMessageOptionNumber.Observe);
            requestMessage.Options.Add(new CoapMessageOptionFactory().CreateObserve(CoapObserveOptionValue.Deregister));

            var responseMessage = await RequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            _observationManager.Deregister(observeResponse.Token);
        }

        internal async Task<CoapMessage> RequestAsync(CoapMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage is null)
            {
                throw new ArgumentNullException(nameof(requestMessage));
            }

            requestMessage.Id = _messageIdProvider.Next();

            var responseAwaiter = _messageDispatcher.AddAwaiter(requestMessage.Id);
            try
            {
                await _lowLevelClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

                var responseMessage = await responseAwaiter.WaitOneAsync(_connectOptions.CommunicationTimeout).ConfigureAwait(false);

                if (responseMessage.Code.Equals(CoapMessageCodes.Empty))
                {
                    // TODO: Support message which are sent later (no piggybacking).
                }

                return responseMessage;
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
                    if (message == null)
                    {
                        continue;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if (!_messageDispatcher.TryHandleReceivedMessage(message))
                    {
                        if (!await _observationManager.TryHandleReceivedMessage(message).ConfigureAwait(false))
                        {
                            _logger.Trace(nameof(CoapClient), "Received an unexpected message ({0}).", message.Id);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    _logger.Error(nameof(CoapClient), exception, "Error while receiving message.");
                }
            }
        }
    }
}

