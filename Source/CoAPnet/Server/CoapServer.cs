using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Server
{
    public sealed class CoapServer : IDisposable
    {
        public ICoapRequestHandler RequestHandler { get; set; }

        public List<ICoapServerEndpoint> Endpoints { get; } = new List<ICoapServerEndpoint>();

        CancellationTokenSource _cancellationTokenSource;

        public Task StartAsync()
        {
            return Task.FromResult(0);
        }

        public Task StopAsync()
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
