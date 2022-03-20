using System;
using System.Threading;
using System.Threading.Tasks;
using CoAPnet.Protocol;

namespace CoAPnet.Server
{
    public interface ICoapServerClient : IDisposable
    {
        Task<CoapMessage> ReceiveAsync(CancellationToken cancellationToken);

        Task SendAsync(CancellationToken cancellationToken);
    }
}