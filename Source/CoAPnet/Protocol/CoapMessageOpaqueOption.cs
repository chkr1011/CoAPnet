using System;

namespace CoapTest
{
    public sealed class CoapMessageOpaqueOption : CoapMessageOption
    {
        public CoapMessageOpaqueOption(byte number, byte[] value) : base(number)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public byte[] Value { get; private set; }
    }
}