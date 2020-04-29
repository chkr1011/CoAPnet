using CoAPnet.Protocol;
using System;

namespace CoAPnet.Client
{
    public class CoapMessageToResponseConverter
    {
        public CoapResponse Convert(CoapMessage message)
        {
            return new CoapResponse
            {
                StatusCode = GetStatusCode(message),
                Payload = message.Payload
            };
        }

        CoapResponseStatusCode GetStatusCode(CoapMessage message)
        {
            if (message.Code.Equals(CoapMessageCodes.Created))
            {
                return CoapResponseStatusCode.Created;
            }

            if (message.Code.Equals(CoapMessageCodes.Deleted))
            {
                return CoapResponseStatusCode.Deleted;
            }

            if (message.Code.Equals(CoapMessageCodes.Valid))
            {
                return CoapResponseStatusCode.Valid;
            }

            if (message.Code.Equals(CoapMessageCodes.Changed))
            {
                return CoapResponseStatusCode.Changed;
            }

            if (message.Code.Equals(CoapMessageCodes.Content))
            {
                return CoapResponseStatusCode.Content;
            }

            if (message.Code.Equals(CoapMessageCodes.BadRequest))
            {
                return CoapResponseStatusCode.BadRequest;
            }

            if (message.Code.Equals(CoapMessageCodes.Unauthorized))
            {
                return CoapResponseStatusCode.Unauthorized;
            }

            if (message.Code.Equals(CoapMessageCodes.BadOption))
            {
                return CoapResponseStatusCode.BadOption;
            }

            if (message.Code.Equals(CoapMessageCodes.Forbidden))
            {
                return CoapResponseStatusCode.Forbidden;
            }

            if (message.Code.Equals(CoapMessageCodes.NotFound))
            {
                return CoapResponseStatusCode.NotFound;
            }

            if (message.Code.Equals(CoapMessageCodes.MethodNotAllowed))
            {
                return CoapResponseStatusCode.MethodNotAllowed;
            }

            if (message.Code.Equals(CoapMessageCodes.NotAcceptable))
            {
                return CoapResponseStatusCode.NotAcceptable;
            }

            if (message.Code.Equals(CoapMessageCodes.PreconditionFailed))
            {
                return CoapResponseStatusCode.PreconditionFailed;
            }

            if (message.Code.Equals(CoapMessageCodes.RequestEntityTooLarge))
            {
                return CoapResponseStatusCode.RequestEntityTooLarge;
            }

            if (message.Code.Equals(CoapMessageCodes.UnsupportedContentFormat))
            {
                return CoapResponseStatusCode.UnsupportedContentFormat;
            }

            if (message.Code.Equals(CoapMessageCodes.InternalServerError))
            {
                return CoapResponseStatusCode.InternalServerError;
            }

            if (message.Code.Equals(CoapMessageCodes.NotImplemented))
            {
                return CoapResponseStatusCode.NotImplemented;
            }

            if (message.Code.Equals(CoapMessageCodes.BadBateway))
            {
                return CoapResponseStatusCode.BadBateway;
            }

            if (message.Code.Equals(CoapMessageCodes.ServiceUnavailable))
            {
                return CoapResponseStatusCode.ServiceUnavailable;
            }

            if (message.Code.Equals(CoapMessageCodes.GatewayTimeout))
            {
                return CoapResponseStatusCode.GatewayTimeout;
            }

            if (message.Code.Equals(CoapMessageCodes.ProxyingNotSupported))
            {
                return CoapResponseStatusCode.ProxyingNotSupported;
            }

            throw new NotSupportedException();
        }
    }
}
