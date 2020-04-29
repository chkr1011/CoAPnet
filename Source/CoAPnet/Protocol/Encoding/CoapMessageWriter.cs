using System;
using System.IO;

namespace CoAPnet.Protocol.Encoding
{
    public sealed class CoapMessageWriter : IDisposable
    {
        readonly MemoryStream _memoryStream = new MemoryStream(128);

        int _bitOffset = 7;
        byte _byteCache = 0x0;

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

        public void WriteBytes(byte[] bytes)
        {
            foreach (var @byte in bytes)
            {
                _memoryStream.WriteByte(@byte);
            }
        }

        public ArraySegment<byte> ToArray()
        {
            if (_bitOffset != 7)
            {
                CommitByteCache();
            }

#if NETSTANDARD2_0
            return new ArraySegment<byte>(_memoryStream.GetBuffer(), 0, (int)_memoryStream.Length);
#else
            return new ArraySegment<byte>(_memoryStream.ToArray(), 0, (int)_memoryStream.Length);
#endif
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
        }

        void CommitByteCache()
        {
            _memoryStream.WriteByte(_byteCache);
            _bitOffset = 7;
            _byteCache = 0x0;
        }
    }
}