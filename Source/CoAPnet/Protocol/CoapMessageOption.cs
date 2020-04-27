namespace CoapTest
{
    public abstract class CoapMessageOption
    {
        protected CoapMessageOption(byte number)
        {
            Number = number;
        }

        public byte Number { get; }
    }
}