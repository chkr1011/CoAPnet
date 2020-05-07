using CoAPnet.Transport;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class UdpTransport : DatagramTransport, IDisposable
    {
        readonly CoapTransportLayerConnectOptions _connectOptions;

        readonly UdpClient _udpClient;

        public UdpTransport(CoapTransportLayerConnectOptions connectOptions)
        {
            _connectOptions = connectOptions ?? throw new ArgumentNullException(nameof(connectOptions));

            _udpClient = new UdpClient(0, connectOptions.EndPoint.AddressFamily);
        }

        public int GetReceiveLimit()
        {
            return 65000;
        }

        public int GetSendLimit()
        {
            return 65000;
        }

        public int Receive(byte[] buf, int off, int len, int waitMillis)
        {
            try
            {
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var datagram = _udpClient.Receive(ref remoteEndPoint);

                Array.Copy(datagram, 0, buf, off, datagram.Length);

                return datagram.Length;
            }
            catch (SocketException)
            {
                return 0;
            }
            catch (ObjectDisposedException)
            {
                return 0;
            }
        }

        public void Send(byte[] buf, int off, int len)
        {
            if (off == 0)
            {
                _udpClient.Send(buf, len, _connectOptions.EndPoint);
            }
            else
            {
                var segment = new ArraySegment<byte>(buf, off, len);
                var buffer2 = segment.ToArray();
                _udpClient.Send(buffer2, buffer2.Length, _connectOptions.EndPoint);
            }
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
        }

        public void Close()
        {
        }
    }
}
