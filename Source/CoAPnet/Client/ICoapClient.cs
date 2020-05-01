using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public interface ICoapClient : IDisposable
    {
        Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken);

        Task<CoapResponse> RequestAsync(CoapRequest request, CancellationToken cancellationToken);
    }
}
