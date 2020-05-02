namespace CoAPnet.Protocol.Options
{
    public enum CoapMessageOptionNumber
    {
        // RFC 7252
        IfMatch = 1,
        UriHost = 3,
        ETag = 4,
        IfNoneMatch = 5,
        UriPort = 7,
        LocationPath = 8,
        UriPath = 11,
        ContentFormat = 12,
        MaxAge = 14,
        UriQuery = 15,
        Accept = 17,
        LocationQuery = 20,
        ProxyUri = 35,
        ProxyScheme = 39,
        Size1 = 60,

        // RFC 7959
        Block1 = 27,
        Block2 = 23
    }
}
