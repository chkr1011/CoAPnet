using System;

namespace CoapTest
{
    public sealed class CoapMessageStringOption : CoapMessageOption
    {
        public CoapMessageStringOption(byte number, string value) : base(number)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public string Value { get; private set; }
    }
}