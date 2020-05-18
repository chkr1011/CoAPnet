using System;
using System.IO;

namespace CoAPnet.Internal
{
    public sealed class MemoryBuffer : IDisposable
    {
        readonly MemoryStream _memoryStream;
        
        public MemoryBuffer(int size)
        {
            _memoryStream = new MemoryStream(size);
        }

        public void Write(byte buffer)
        {
            _memoryStream.WriteByte(buffer);
        }

        public void Write(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            _memoryStream.Write(buffer, 0, buffer.Length);
        }

        public void Write(ArraySegment<byte> buffer)
        {
            if (buffer.Array is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            _memoryStream.Write(buffer.Array, buffer.Offset, buffer.Count);
        }

        public ArraySegment<byte> GetBuffer()
        {
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
    }
}
