namespace CoapTest
{
    public static class CoapMessageCodes
    {
        public static CoapMessageCode Empty { get; } = new CoapMessageCode(0, 0);
        public static CoapMessageCode Get { get; } = new CoapMessageCode(0, 1);
        public static CoapMessageCode Post { get; } = new CoapMessageCode(0, 2);
        public static CoapMessageCode Put { get; } = new CoapMessageCode(0, 3);
        public static CoapMessageCode Delete { get; } = new CoapMessageCode(0, 4);

        public static CoapMessageCode Created { get; } = new CoapMessageCode(2, 1);
        public static CoapMessageCode Deleted { get; } = new CoapMessageCode(2, 2);
        public static CoapMessageCode Valid { get; } = new CoapMessageCode(2, 3);
        public static CoapMessageCode Changed { get; } = new CoapMessageCode(2, 4);
        public static CoapMessageCode Content { get; } = new CoapMessageCode(2, 5);

        public static CoapMessageCode BadRequest { get; } = new CoapMessageCode(4, 0);
        public static CoapMessageCode Unauthorized { get; } = new CoapMessageCode(4, 1);
        public static CoapMessageCode BadOption { get; } = new CoapMessageCode(4, 2);
        public static CoapMessageCode Forbidden { get; } = new CoapMessageCode(4, 3);
        public static CoapMessageCode NotFound { get; } = new CoapMessageCode(4, 4);
        public static CoapMessageCode MethodNotAllowed { get; } = new CoapMessageCode(4, 5);
        public static CoapMessageCode NotAcceptable { get; } = new CoapMessageCode(4, 6);
        public static CoapMessageCode PreconditionFailed { get; } = new CoapMessageCode(4, 12);
        public static CoapMessageCode RequestEntityTooLarge { get; } = new CoapMessageCode(4, 13);
        public static CoapMessageCode UnsupportedContentFormat { get; } = new CoapMessageCode(4, 15);

        public static CoapMessageCode InternalServerError { get; } = new CoapMessageCode(5, 0);
        public static CoapMessageCode NotImplemented { get; } = new CoapMessageCode(5, 1);
        public static CoapMessageCode BadBateway { get; } = new CoapMessageCode(5, 2);
        public static CoapMessageCode ServiceUnavailable { get; } = new CoapMessageCode(5, 3);
        public static CoapMessageCode GatewayTimeout { get; } = new CoapMessageCode(5, 4);
        public static CoapMessageCode ProxyingNotSupported { get; } = new CoapMessageCode(5, 5);
    }
}