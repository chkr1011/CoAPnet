namespace CoapTest
{
    public sealed class CoapMessageOptionFactory
    {
        public CoapMessageOption CreateIfMatch(byte[] payload)
        {
            return new CoapMessageOpaqueOption(1, payload);
        }

        public CoapMessageOption CreateUriHost(byte[] payload)
        {
            return new CoapMessageOpaqueOption(3, payload);
        }

        public CoapMessageOption CreateUriHost(string host)
        {
            return new CoapMessageStringOption(3, host);
        }

        public CoapMessageOption CreateETagHost(byte[] payload)
        {
            return new CoapMessageOpaqueOption(4, payload);
        }

        public CoapMessageOption CreateIfNoneMatch(byte[] payload)
        {
            return new CoapMessageOpaqueOption(5, payload);
        }

        public CoapMessageOption CreateUriPort(uint port)
        {
            return new CoapMessageUintOption(7, port);
        }

        public CoapMessageOption CreateLocationPath(byte[] payload)
        {
            return new CoapMessageOpaqueOption(8, payload);
        }

        public CoapMessageOption CreateUriPath(string payload)
        {
            return new CoapMessageStringOption(11, payload);
        }

        public CoapMessageOption CreateContentFormat(uint value)
        {
            return new CoapMessageUintOption(12, value);
        }

        public CoapMessageOption CreateMaxAge(uint value)
        {
            return new CoapMessageUintOption(14, value);
        }

        public CoapMessageOption CreateUriQuery(byte[] payload)
        {
            return new CoapMessageOpaqueOption(15, payload);
        }

        public CoapMessageOption CreateAccept(byte[] payload)
        {
            return new CoapMessageOpaqueOption(17, payload);
        }

        public CoapMessageOption CreateLocationQuery(byte[] payload)
        {
            return new CoapMessageOpaqueOption(20, payload);
        }

        public CoapMessageOption CreateProxyUri(byte[] payload)
        {
            return new CoapMessageOpaqueOption(35, payload);
        }

        public CoapMessageOption CreateProxyScheme(byte[] payload)
        {
            return new CoapMessageOpaqueOption(39, payload);
        }

        public CoapMessageOption CreateSize1(byte[] payload)
        {
            return new CoapMessageOpaqueOption(60, payload);
        }
    }
}