using System;

namespace CoAPnet.Protocol.Options
{
    public sealed class CoapMessageOption
    {
        public CoapMessageOption(CoapMessageOptionNumber number, CoapMessageOptionValue value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            Number = number;
            Value = value;
        }

        public CoapMessageOptionNumber Number
        {
            get;
        }

        public CoapMessageOptionValue Value
        {
            get; set;
        }

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