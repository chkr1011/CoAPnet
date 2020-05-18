using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace CoAPnet.Protocol.Encoding
{
    public sealed class CoapMessageReader : IDisposable
    {
        readonly MemoryStream _memoryStream;
        readonly ArraySegment<byte> _buffer;

        int _bitOffset = -1;
        byte _byteCache;

        public CoapMessageReader(ArraySegment<byte> buffer)
        {
            _memoryStream = new MemoryStream(buffer.Array, 0, buffer.Count, false);
            _buffer = buffer;
        }

        public bool EndOfStream => _memoryStream.Position == _memoryStream.Length;

        public int ReadBits(int count)
        {
            var result = 0;

            // Read each bit in backward order as per RFC.
            var j = count - 1;
            for (var i = 0; i < count; i++)
            {
                if (_bitOffset < 0)
                {
                    FillByteCache();
                }

                if ((_byteCache & (0x1 << _bitOffset)) > 0)
                {
                    result |= 0x1 << j;
                }

                j--;
                _bitOffset--;
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
            return new ArraySegment<byte>(_buffer.Array, (int)_memoryStream.Position, (int)(_memoryStream.Length - _memoryStream.Position));
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void FillByteCache()
        {
            _byteCache = (byte)_memoryStream.ReadByte();
            _bitOffset = 7;
        }
    }
}