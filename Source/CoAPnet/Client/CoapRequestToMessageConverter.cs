using CoAPnet.Protocol;
using System;
using System.Collections.Generic;

namespace CoAPnet.Client
{
    public class CoapRequestToMessageConverter
    {
        readonly CoapMessageOptionFactory _optionFactory = new CoapMessageOptionFactory();

        public CoapMessage Convert(CoapRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var message = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = GetMessageCode(request.Method),
                Options = new List<CoapMessageOption>()
            };

            message.Options.Add(_optionFactory.CreateUriPath(request.Uri));
            message.Options.Add(_optionFactory.CreateUriPort(5648));

            return message;
        }

        static CoapMessageCode GetMessageCode(CoapRequestMethod method)
        {
            switch (method)
            {
                case CoapRequestMethod.Get: return CoapMessageCodes.Get;
                case CoapRequestMethod.Post: return CoapMessageCodes.Post;
                case CoapRequestMethod.Delete: return CoapMessageCodes.Delete;
                case CoapRequestMethod.Put: return CoapMessageCodes.Put;
                default: throw new NotSupportedException();
            }
        }
    }
}
