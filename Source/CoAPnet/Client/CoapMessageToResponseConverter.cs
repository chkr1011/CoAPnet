using CoAPnet.Protocol;
using CoAPnet.Protocol.Options;
using System;
using System.Linq;

namespace CoAPnet.Client
{
    public sealed class CoapMessageToResponseConverter
    {
        public CoapResponse Convert(CoapMessage message, ArraySegment<byte> payload)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            return new CoapResponse
            {
                StatusCode = GetStatusCode(message),
                Options = GetOptions(message),
                Payload = payload.ToArray(),
            };
        }

        CoapResponseOptions GetOptions(CoapMessage message)
        {
            var options = new CoapResponseOptions();

            var contentFormatOption = message.Options.FirstOrDefault(o => o.Number == CoapMessageOptionNumber.ContentFormat);
            if (contentFormatOption != null)
            {
                options.ContentFormat = (CoapMessageContentFormat)((CoapMessageOptionUintValue)contentFormatOption.Value).Value;
            }

            var maxAgeOption = message.Options.FirstOrDefault(o => o.Number == CoapMessageOptionNumber.MaxAge);
            if (maxAgeOption != null)
            {
                options.MaxAge = (int)((CoapMessageOptionUintValue)maxAgeOption.Value).Value;
            }
            else
            {
                // From RFC:  A default value of 60 seconds is assumed in the absence of the option in a response.
                options.MaxAge = 60;
            }

            var eTagOption = message.Options.FirstOrDefault(o => o.Number == CoapMessageOptionNumber.ETag);
            if (eTagOption != null)
            {
                options.ETag = ((CoapMessageOptionOpaqueValue)eTagOption.Value).Value;
            }

            return options;
        }

        CoapResponseStatusCode GetStatusCode(CoapMessage message)
        {
            if (message.Code.Equals(CoapMessageCodes.Empty))
            {
                return CoapResponseStatusCode.Empty;
            }

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
