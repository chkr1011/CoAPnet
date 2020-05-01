using CoAPnet.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.LowLevelClient
{
    public interface ILowLevelCoapClient : IDisposable
    {
        Task SendAsync(CoapMessage message, CancellationToken cancellationToken);

        Task<CoapMessage> ReceiveAsync(CancellationToken cancellationToken);
    }
}
