using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public sealed class CoapObserveResponse
    {
        readonly ICoapClient _client;

        public CoapObserveResponse(CoapResponse response, ICoapClient client)
        {
            Response = response ?? throw new System.ArgumentNullException(nameof(response));
            _client = client ?? throw new System.ArgumentNullException(nameof(client));
        }

        public CoapResponse Response { get; }

        public Task StopObservationAsync(CancellationToken cancellationToken)
        {
            return _client.StopObservationAsync(this, cancellationToken);
        }

        internal CoapMessageToken Token { get; set; }

        internal CoapRequest Request { get; set; }
    }
}

