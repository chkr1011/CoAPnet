namespace CoAPnet.Protocol
{
    public sealed class CoapMessageCode
    {
        public CoapMessageCode(byte @class, byte detail)
        {
            Class = @class;
            Detail = detail;
        }

        public byte Class { get; }

        public byte Detail { get; }

        public override string ToString()
        {
            return $"{Class}.{Detail.ToString().PadLeft(2, '0')}";
        }

        public override int GetHashCode()
        {
            return Class.GetHashCode() ^ Detail.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var otherCode = other as CoapMessageCode;
            if (otherCode == null)
            {
                return false;
            }

            return Class.Equals(otherCode.Class) && Detail.Equals(otherCode.Detail);
        }
    }
}