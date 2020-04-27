using System;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public sealed class CoapClient : IDisposable
    {
        public async Task ConnectAsync(CoapClientConnectOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));


        }

        public void Dispose()
        {

        }
    }
}
