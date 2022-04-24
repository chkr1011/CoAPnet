namespace CoAPnet.Protocol.Options
{
    public sealed class CoapMessageOptionEmptyValue : CoapMessageOptionValue
    {
        public override bool Equals(object obj)
        {
            return obj is CoapMessageOptionEmptyValue;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}