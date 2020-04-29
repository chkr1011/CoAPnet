namespace CoAPnet.Protocol
{
    public class CoapMessageOptionUintValue : CoapMessageOptionValue
    {
        public CoapMessageOptionUintValue(uint value)
        {
            Value = value;
        }

        public uint Value { get; }

        public override bool Equals(object obj)
        {
            if (obj is CoapMessageOptionUintValue other)
            {
                return Value.Equals(other.Value);
            }

            return false;
        }
    }
}