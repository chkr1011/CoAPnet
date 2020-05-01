using System;

namespace CoAPnet.Protocol
{
    public sealed class CoapMessageOption
    {
        public CoapMessageOption(byte number, CoapMessageOptionValue value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            Number = number;
            Value = value;
        }

        public byte Number { get; }

        public CoapMessageOptionValue Value { get; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is CoapMessageOption other)
            {
                return Number.Equals(other.Number) && Value.Equals(other.Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }
}