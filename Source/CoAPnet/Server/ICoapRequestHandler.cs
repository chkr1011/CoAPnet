using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Server
{
    public interface ICoapRequestHandler
    {
        Task HandleCoapRequest(CoapRequestContext context, CancellationToken cancellationToken);
    }
}