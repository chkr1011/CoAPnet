using System.Linq;

namespace CoAPnet.Protocol.Options
{
    public sealed class CoapMessageOptionOpaqueValue : CoapMessageOptionValue
    {
        public CoapMessageOptionOpaqueValue(byte[] value)
        {
            Value = value;
        }

        public byte[] Value
        {
            get;
        }

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
            return Value == null ? 0 : Value.GetHashCode();
        }
    }
}