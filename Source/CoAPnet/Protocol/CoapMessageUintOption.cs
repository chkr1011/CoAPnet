namespace CoapTest
{
    public sealed class CoapMessageUintOption : CoapMessageOption
    {
        public CoapMessageUintOption(byte number, uint value) : base(number)
        {
            Value = value;
        }

        public uint Value { get; private set; }
    }
}