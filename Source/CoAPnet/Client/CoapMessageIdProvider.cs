using System;

namespace CoAPnet.Client
{
    public sealed class CoapMessageIdProvider
    {
        readonly object _syncRoot = new object();

        ushort _value;

        public CoapMessageIdProvider()
        {
            // From RFC: It is strongly recommended that the initial
            // value of the variable(e.g., on startup) be randomized, in order
            // to make successful off - path attacks on the protocol less likely.
            var buffer = new byte[2];
            new Random().NextBytes(buffer);

            _value = BitConverter.ToUInt16(buffer, 0);
        }

        public ushort Next()
        {
            lock (_syncRoot)
            {
                var result = _value;
                _value++;
                return _value;
            }
        }
    }
}
