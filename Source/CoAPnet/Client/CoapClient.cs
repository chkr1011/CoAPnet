using CoAPnet.LowLevelClient;
using CoAPnet.MessageDispatcher;
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

        LowLevelCoapClient _lowLevelClient;
        CoapClientConnectOptions _connectOptions;
        CancellationTokenSource _cancellationToken;

        public async Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _connectOptions = options;

            _lowLevelClient?.Dispose();

            _lowLevelClient = new LowLevelCoapClient();

            await _lowLevelClient.ConnectAsync(options, cancellationToken).ConfigureAwait(false);
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() => ReceiveMessages(_cancellationToken.Token), _cancellationToken.Token);
        }

        public async Task<CoapResponse> RequestAsync(CoapRequest request, CancellationToken cancellationToken)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var requestMessage = _requestToMessageConverter.Convert(request);
            requestMessage.Id = _messageIdProvider.Next();

            var responseAwaiter = _messageDispatcher.AddAwaiter(requestMessage.Id);
            await _lowLevelClient.SendAsync(requestMessage, cancellationToken);

            var responseMessage = await responseAwaiter.WaitOneAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

            return _messageToResponseConverter.Convert(responseMessage);
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

