using CoAPnet.Client;
using CoAPnet.Exceptions;
using CoAPnet.Transport;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class UdpWithDtlsCoapTransportLayer : ICoapTransportLayer
    {
        static bool _waherTypesInitialized;

        readonly BlockingCollection<byte[]> _inflightQueue = new BlockingCollection<byte[]>();

        UdpClient _udpClient;
        Waher.Security.DTLS.DtlsOverUdp _dtlsClient;
        CoapClientConnectOptions _connectOptions;
        IPEndPoint _ipEndPoint;
        Waher.Security.DTLS.IDtlsCredentials _credentials;

        public UdpWithDtlsCoapTransportLayer()
        {
            if (!_waherTypesInitialized)
            {
                Waher.Runtime.Inventory.Types.Initialize(typeof(Waher.Security.DTLS.Ciphers.PskCipher).Assembly);
                _waherTypesInitialized = true;
            }
        }

        public IDtlsCredentials Credentials { get; set; }

        public async Task ConnectAsync(CoapClientConnectOptions options, CancellationToken cancellationToken)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _connectOptions = options;

            Dispose();

            await ResolveIPEndpointAsync().ConfigureAwait(false);
            ConvertCredentials();

            // ! Match the local address family with the address family of the host!
            _udpClient = new UdpClient(0, _ipEndPoint.AddressFamily);
            _dtlsClient = new Waher.Security.DTLS.DtlsOverUdp(_udpClient, Waher.Security.DTLS.DtlsMode.Client, null, null);
            _dtlsClient.OnDatagramReceived += OnDatagramReceived;
        }

        public Task<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            // TODO: Replace with proper async implementation.
            // The Tas.Run is required to let the current thread
            // wait for messages in the UDP client.
            return Task.Run(() =>
            {
                var datagram = _inflightQueue.Take(cancellationToken);
                Array.Copy(datagram, 0, buffer.Array, buffer.Offset, datagram.Length);
                return Task.FromResult(datagram.Length);
            });
        }

        public async Task SendAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            ThrowIfNotConnected();

            var promise = new TaskCompletionSource<bool>();

            _dtlsClient.Send(buffer.ToArray(), _ipEndPoint, _credentials, (s, e) =>
            {
                promise.TrySetResult(e.Successful);
            }, null);

            var result = await promise.Task.ConfigureAwait(false);

            if (!result)
            {
                throw new CoapCommunicationException("Sending CoAP message over DTLS failed.", null);
            }
        }

        public void Dispose()
        {
            if (_dtlsClient != null)
            {
                _dtlsClient.OnDatagramReceived -= OnDatagramReceived;
                _dtlsClient.Dispose();
            }

            _udpClient?.Dispose();
        }

        async Task ResolveIPEndpointAsync()
        {
            if (IPAddress.TryParse(_connectOptions.Host, out var hostIPAddress))
            {
                _ipEndPoint = new IPEndPoint(hostIPAddress, _connectOptions.Port);
                return;
            }

            var hostIPAddresses = await Dns.GetHostAddressesAsync(_connectOptions.Host).ConfigureAwait(false);

            // We only use the first address for now.
            _ipEndPoint = new IPEndPoint(hostIPAddresses.First(), _connectOptions.Port);
        }

        void ConvertCredentials()
        {
            if (Credentials == null)
            {
                return;
            }

            if (Credentials is PreSharedKey psk)
            {
                _credentials = new Waher.Security.DTLS.PresharedKey(psk.Identity, psk.Key);
                return;
            }

            throw new NotSupportedException();
        }

        void OnDatagramReceived(object sender, Waher.Security.DTLS.Events.UdpDatagramEventArgs e)
        {
            _inflightQueue.Add(e.Datagram);
        }

        void ThrowIfNotConnected()
        {
            if (_dtlsClient == null)
            {
                throw new InvalidOperationException("The CoAP transport layer is not connected.");
            }
        }
    }
}
