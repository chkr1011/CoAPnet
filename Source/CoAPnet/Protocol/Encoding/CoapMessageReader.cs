using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CoAPnet.Protocol.Encoding
{
    public sealed class CoapMessageReader : IDisposable
    {
        readonly byte[] _buffer;
        readonly int _length;

        int _bitOffset = -1;
        byte _byteCache;
        int _position;

        public CoapMessageReader(ArraySegment<byte> buffer)
        {
            Debug.Assert(buffer.Array != null);

            _buffer = buffer.Array;
            _position = buffer.Offset;
            _length = buffer.Count;
        }

        public bool EndOfStream => _position == _length;

        public void Dispose()
        {
        }

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
            var @byte = _buffer[_position];
            _position++;

            return @byte;
        }

        public byte[] ReadBytes(int count)
        {
            var buffer = new byte[count];
            Array.Copy(_buffer, _position, buffer, 0, count);
            _position += count;
            return buffer;
        }

        public byte[] ReadToEnd()
        {
            // We have to copy the payload because the internal buffer is used for other calls!
            return ReadBytes(_length - _position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void FillByteCache()
        {
            _byteCache = _buffer[_position];
            _position++;
            _bitOffset = 7;
        }
    }
}