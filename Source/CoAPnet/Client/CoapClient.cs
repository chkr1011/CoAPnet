using CoAPnet.Internal;
using CoAPnet.Logging;
using CoAPnet.LowLevelClient;
using CoAPnet.MessageDispatcher;
using CoAPnet.Protocol;
using CoAPnet.Protocol.Observe;
using CoAPnet.Protocol.Options;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public sealed class CoapClient : ICoapClient
    {
        readonly ConcurrentDictionary<CoapMessageToken, ICoapResponseHandler> _observedResponseHandlers = new ConcurrentDictionary<CoapMessageToken, ICoapResponseHandler>();
        readonly CoapRequestToMessageConverter _requestToMessageConverter = new CoapRequestToMessageConverter();
        readonly CoapMessageToResponseConverter _messageToResponseConverter = new CoapMessageToResponseConverter();
        readonly CoapMessageDispatcher _messageDispatcher = new CoapMessageDispatcher();
        readonly CoapMessageIdProvider _messageIdProvider = new CoapMessageIdProvider();
        readonly CoapMessageTokenProvider _messageTokenProvider = new CoapMessageTokenProvider();
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
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _connectOptions = options;

            _lowLevelClient?.Dispose();

            _lowLevelClient = new LowLevelCoapClient(_logger);

            await _lowLevelClient.ConnectAsync(options, cancellationToken).ConfigureAwait(false);
            _cancellationToken = new CancellationTokenSource();

            ParallelTask.Run(() => ReceiveMessages(_cancellationToken.Token), _cancellationToken.Token);
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

            _observedResponseHandlers[token] = options.ResponseHandler;

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

            requestMessage.Options.RemoveAll(o => o.Number == (byte)CoapMessageOptionNumber.Observe);
            requestMessage.Options.Add(new CoapMessageOptionFactory().CreateObserve(CoapObserveOptionValue.Deregister));

            var responseMessage = await RequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            _observedResponseHandlers.TryRemove(observeResponse.Token, out var _);
        }

        internal async Task<CoapMessage> RequestAsync(CoapMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage is null) throw new ArgumentNullException(nameof(requestMessage));

            requestMessage.Id = _messageIdProvider.Next();

            var responseAwaiter = _messageDispatcher.AddAwaiter(requestMessage.Id);
            try
            {
                await _lowLevelClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

                var responseMessage = await responseAwaiter.WaitOneAsync(_connectOptions.CommunicationTimeout).ConfigureAwait(false);

                if (responseMessage.Code == CoapMessageCodes.Empty)
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

                    if (!_messageDispatcher.TryDispatch(message))
                    {
                        if (!await TryHandleObservedMessage(message).ConfigureAwait(false))
                        {
                            await DeregisterObservation(message).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(nameof(CoapClient), exception, "Error while receiving message.");
                }
            }
        }

        async Task DeregisterObservation(CoapMessage message)
        {
            var emptyResponse = new CoapMessage
            {
                Type = CoapMessageType.Reset,
                Code = CoapMessageCodes.Empty,
                Id = message.Id
            };

            await _lowLevelClient.SendAsync(emptyResponse, CancellationToken.None).ConfigureAwait(false);
            _logger.Information(nameof(CoapClient), "Received unobserved message. Sending empty response to deregister.");
        }

        async Task<bool> TryHandleObservedMessage(CoapMessage coapMessage)
        {
            try
            {
                if (coapMessage.Token?.Length != 8)
                {
                    return false;
                }

                var token = BitConverter.ToUInt64(coapMessage.Token, 0);
                if (!_observedResponseHandlers.TryGetValue(new CoapMessageToken(coapMessage.Token), out var responseHandler))
                {
                    return false;
                }

                var payload = coapMessage.Payload;

                // TODO: Check if supported etc.
                //if (CoapClientBlockTransferReceiver.IsBlockTransfer(coapMessage))
                //{
                //    payload = await new CoapClientBlockTransferReceiver(requestMessage, coapMessage, this, _logger).ReceiveFullPayload(CancellationToken.None).ConfigureAwait(false);
                //}

                var response = _messageToResponseConverter.Convert(coapMessage, payload);

                await responseHandler.HandleResponseAsync(new HandleResponseContext
                {
                    Response = response
                }).ConfigureAwait(false);

                return true;
            }
            catch (Exception exception)
            {
                _logger.Error(nameof(CoapClient), exception, "Error while handling observed message.");

                return false;
            }
        }
    }
}

