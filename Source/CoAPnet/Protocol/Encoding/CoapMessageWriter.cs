using CoAPnet.Internal;
using System;
using System.Runtime.CompilerServices;

namespace CoAPnet.Protocol.Encoding
{
    public sealed class CoapMessageWriter : IDisposable
    {
        readonly MemoryBuffer _memoryBuffer = new MemoryBuffer(128);

        int _bitOffset = 7;
        byte _byteCache;

        public void WriteBits(int data, int count)
        {
            // Write each bit in backward order as per RFC.
            for (var i = count - 1; i >= 0; i--)
            {
                var bit = (data >> i & 1) != 0;
                if (bit)
                {
                    _byteCache |= (byte)(1 << _bitOffset);
                }

                _bitOffset--;

                if (_bitOffset < 0)
                {
                    CommitByteCache();
                }
            }
        }

        public void WriteByte(byte @byte)
        {
            _memoryBuffer.Write(@byte);
        }

        public void WriteBytes(byte[] bytes)
        {
            _memoryBuffer.Write(bytes);
        }

        public void WriteBytes(ArraySegment<byte> bytes)
        {
            _memoryBuffer.Write(bytes);
        }

        public ArraySegment<byte> ToArray()
        {
            if (_bitOffset != 7)
            {
                CommitByteCache();
            }

            return _memoryBuffer.GetBuffer();
        }

        public void Dispose()
        {
            _memoryBuffer.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CommitByteCache()
        {
            _memoryBuffer.Write(_byteCache);
            _bitOffset = 7;
            _byteCache = 0x0;
        }
    }
}