using CoAPnet.Logging;
using CoAPnet.LowLevelClient;
using CoAPnet.Protocol;
using CoAPnet.Protocol.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public sealed class CoapClientObservationManager
    {
        readonly LowLevelCoapClient _client;
        readonly CoapNetLogger _logger;
        readonly CoapMessageToResponseConverter _messageToResponseConverter;
        readonly ConcurrentDictionary<CoapMessageToken, ICoapResponseHandler> _observedResponseHandlers = new ConcurrentDictionary<CoapMessageToken, ICoapResponseHandler>();

        public CoapClientObservationManager(CoapMessageToResponseConverter messageToResponseConverter, LowLevelCoapClient client, CoapNetLogger logger)
        {
            _messageToResponseConverter = messageToResponseConverter ?? throw new ArgumentNullException(nameof(messageToResponseConverter));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Deregister(CoapMessageToken token)
        {
            _observedResponseHandlers.TryRemove(token, out _);
        }

        public void Register(CoapMessageToken token, ICoapResponseHandler responseHandler)
        {
            _observedResponseHandlers[token] = responseHandler;
        }

        public async Task<bool> TryHandleReceivedMessage(CoapMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            try
            {
                var observeOption = message.Options.FirstOrDefault(o => o.Number == CoapMessageOptionNumber.Observe);
                if (observeOption == null)
                {
                    return false;
                }

                if (!_observedResponseHandlers.TryGetValue(new CoapMessageToken(message.Token), out var responseHandler))
                {
                    await DeregisterObservation(message).ConfigureAwait(false);
                    return true;
                }

                if (message.Type == CoapMessageType.Confirmable)
                {
                    var ackMessage = new CoapMessage
                    {
                        Type = CoapMessageType.Acknowledgement,
                        Code = CoapMessageCodes.Empty,
                        Id = message.Id
                    };

                    await _client.SendAsync(ackMessage, CancellationToken.None).ConfigureAwait(false);
                }

                var payload = message.Payload;

                // TODO: Check if supported etc.
                //if (CoapClientBlockTransferReceiver.IsBlockTransfer(coapMessage))
                //{
                //    payload = await new CoapClientBlockTransferReceiver(requestMessage, coapMessage, this, _logger).ReceiveFullPayload(CancellationToken.None).ConfigureAwait(false);
                //}

                var sequenceNumber = ((CoapMessageOptionUintValue)observeOption.Value).Value;
                var response = _messageToResponseConverter.Convert(message, payload);

                await responseHandler.HandleResponseAsync(new HandleResponseContext
                {
                    SequenceNumber = sequenceNumber,
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

        Task DeregisterObservation(CoapMessage message)
        {
            var emptyResponse = new CoapMessage
            {
                Type = CoapMessageType.Reset,
                Code = CoapMessageCodes.Empty,
                Id = message.Id
            };

            _logger.Information(nameof(CoapClient), "Received unobserved message. Sending empty response to deregister.");
            return _client.SendAsync(emptyResponse, CancellationToken.None);
        }
    }
}