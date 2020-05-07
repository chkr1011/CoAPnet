using System;

namespace CoAPnet.Client
{
    public class CoapMessageTokenProvider
    {
        ulong _value;

        public CoapMessageToken Next()
        {
            _value++;
            return new CoapMessageToken(BitConverter.GetBytes(_value));
        }
    }
}

