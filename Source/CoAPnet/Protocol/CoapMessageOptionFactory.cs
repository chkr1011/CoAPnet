namespace CoAPnet.Protocol
{
    public sealed class CoapMessageOptionFactory
    {
        public CoapMessageOption CreateIfMatch(byte[] value)
        {
            return new CoapMessageOption(1, new CoapMessageOptionOpaqueValue(value));
        }

        public CoapMessageOption CreateUriHost(string value)
        {
            return new CoapMessageOption(3, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateETag(byte[] value)
        {
            return new CoapMessageOption(4, new CoapMessageOptionOpaqueValue(value));
        }

        public CoapMessageOption CreateIfNoneMatch()
        {
            return new CoapMessageOption(5, new CoapMessageOptionEmptyValue());
        }

        public CoapMessageOption CreateUriPort(uint value)
        {
            return new CoapMessageOption(7, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateLocationPath(string value)
        {
            return new CoapMessageOption(8, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateUriPath(string value)
        {
            return new CoapMessageOption(11, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateContentFormat(CoapMessageContentFormat value)
        {
            return new CoapMessageOption(12, new CoapMessageOptionUintValue((uint)value));
        }

        public CoapMessageOption CreateMaxAge(uint value)
        {
            return new CoapMessageOption(14, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateUriQuery(string value)
        {
            return new CoapMessageOption(15, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateAccept(uint value)
        {
            return new CoapMessageOption(17, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateLocationQuery(string value)
        {
            return new CoapMessageOption(20, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateProxyUri(string value)
        {
            return new CoapMessageOption(35, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateProxyScheme(string value)
        {
            return new CoapMessageOption(39, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateSize1(uint value)
        {
            return new CoapMessageOption(60, new CoapMessageOptionUintValue(value));
        }
    }
}