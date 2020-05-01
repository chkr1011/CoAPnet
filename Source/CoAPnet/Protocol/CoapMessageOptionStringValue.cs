namespace CoAPnet.Protocol
{
    public class CoapMessageOptionStringValue : CoapMessageOptionValue
    {
        public CoapMessageOptionStringValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            if (obj is CoapMessageOptionStringValue other)
            {
                return string.Equals(Value, other.Value, System.StringComparison.Ordinal);
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