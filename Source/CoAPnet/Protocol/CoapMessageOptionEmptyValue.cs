﻿namespace CoAPnet.Protocol
{
    public class CoapMessageOptionEmptyValue : CoapMessageOptionValue
    {
        public CoapMessageOptionEmptyValue()
        {
        }

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