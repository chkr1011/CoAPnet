using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Server
{
    public interface ICoapServerEndpoint : IDisposable
    {
        Task BindAsync(CancellationToken cancellationToken);

        Task<ICoapServerClient> AcceptAsync(CancellationToken cancellationToken);
    }
}