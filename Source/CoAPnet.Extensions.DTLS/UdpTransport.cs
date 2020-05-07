using CoAPnet.Transport;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Net;
using System.Net.Sockets;

namespace CoAPnet.Extensions.DTLS
{
    public sealed class UdpTransport : DatagramTransport, IDisposable
    {
        readonly CoapTransportLayerConnectOptions _connectOptions;
        readonly Socket _socket;

        public UdpTransport(CoapTransportLayerConnectOptions connectOptions)
        {
            _connectOptions = connectOptions ?? throw new ArgumentNullException(nameof(connectOptions));

            _socket = new Socket(connectOptions.EndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
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
            if (buf is null)
            {
                throw new ArgumentNullException(nameof(buf));
            }

            try
            {
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var datagramLength = _socket.ReceiveFrom(buf, off, len, SocketFlags.None, ref remoteEndPoint);

                return datagramLength;
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
            if (buf is null)
            {
                throw new ArgumentNullException(nameof(buf));
            }

            _socket.SendTo(buf, off, len, SocketFlags.None, _connectOptions.EndPoint);
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        public void Close()
        {
        }
    }
}
