using System.Linq;

namespace CoAPnet.Protocol
{
    public class CoapMessageOptionOpaqueValue : CoapMessageOptionValue
    {
        public CoapMessageOptionOpaqueValue(byte[] value)
        {
            Value = value;
        }

        public byte[] Value { get; }

        public override bool Equals(object obj)
        {
            if (obj is CoapMessageOptionOpaqueValue other)
            {
                return Value.SequenceEqual(other.Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Value == null)
            {
                return 0;
            }

            return Value.GetHashCode();
        }
    }
}