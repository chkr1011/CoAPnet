namespace CoAPnet.Protocol.Options
{
    public sealed class CoapMessageOptionFactory
    {
        public CoapMessageOption CreateIfMatch(byte[] value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.IfMatch, new CoapMessageOptionOpaqueValue(value));
        }

        public CoapMessageOption CreateUriHost(string value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.UriHost, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateETag(byte[] value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.ETag, new CoapMessageOptionOpaqueValue(value));
        }

        public CoapMessageOption CreateIfNoneMatch()
        {
            return new CoapMessageOption(CoapMessageOptionNumber.IfNoneMatch, new CoapMessageOptionEmptyValue());
        }

        public CoapMessageOption CreateUriPort(uint value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.UriPort, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateObserve(uint value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.Observe, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateLocationPath(string value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.LocationPath, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateUriPath(string value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.UriPath, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateContentFormat(CoapMessageContentFormat value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.ContentFormat, new CoapMessageOptionUintValue((uint)value));
        }

        public CoapMessageOption CreateMaxAge(uint value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.MaxAge, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateUriQuery(string value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.UriQuery, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateAccept(uint value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.Accept, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateLocationQuery(string value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.LocationQuery, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateProxyUri(string value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.ProxyUri, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateProxyScheme(string value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.ProxyScheme, new CoapMessageOptionStringValue(value));
        }

        public CoapMessageOption CreateSize1(uint value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.Size1, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateBlock1(uint value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.Block1, new CoapMessageOptionUintValue(value));
        }

        public CoapMessageOption CreateBlock2(uint value)
        {
            return new CoapMessageOption(CoapMessageOptionNumber.Block2, new CoapMessageOptionUintValue(value));
        }
    }
}