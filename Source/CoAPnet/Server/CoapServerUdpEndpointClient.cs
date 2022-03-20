using System;
using System.Threading;
using System.Threading.Tasks;
using CoAPnet.Protocol;

namespace CoAPnet.Server
{
    public sealed class CoapServerUdpEndpointClient : ICoapServerClient
    {
        public CoapServerUdpEndpointClient()
        {
            
        }

        public Task<CoapMessage> ReceiveAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}