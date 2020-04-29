using System;
using System.IO;

namespace CoAPnet.Protocol.Encoding
{
    public sealed class CoapMessageReader : IDisposable
    {
        readonly MemoryStream _memoryStream;

        int _bitOffset = -1;
        byte _byteCache = 0x0;

        public CoapMessageReader(ArraySegment<byte> buffer)
        {
            _memoryStream = new MemoryStream(buffer.Array, 0, buffer.Count, false);
        }

        public bool EndOfStream
        {
            get
            {
                return _memoryStream.Position == _memoryStream.Length;
            }
        }

        public int ReadBits(int count)
        {
            // TODO: This needs optimization!

            // Read each bit in backward order as per RFC.
            var bits = new bool[count];
            for (var i = 0; i < count; i++)
            {
                if (_bitOffset < 0)
                {
                    FillByteCache();
                }

                if ((_byteCache & (0x1 << _bitOffset)) > 0)
                {
                    bits[i] = true;
                }

                _bitOffset--;
            }

            var result = 0;
            var j = 0;
            for (var i = count - 1; i >= 0; i--)
            {
                if (bits[i])
                {
                    result |= 0x1 << j;
                }

                j++;
            }

            return result;
        }

        public byte ReadByte()
        {
            return (byte)_memoryStream.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            var buffer = new byte[count];
            _memoryStream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public ArraySegment<byte> ReadToEnd()
        {
            return new ArraySegment<byte>(_memoryStream.ToArray(), (int)_memoryStream.Position, (int)(_memoryStream.Length - _memoryStream.Position));
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
        }

        void FillByteCache()
        {
            _byteCache = (byte)_memoryStream.ReadByte();
            _bitOffset = 7;
        }
    }
}