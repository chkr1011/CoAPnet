using System;
using System.Collections.Generic;
using System.Text;

namespace CoAPnet.Client
{
    public class CoapRequestBuilder
    {
        readonly CoapRequest _request = new CoapRequest();

        public CoapRequestBuilder WithMethod(CoapRequestMethod value)
        {
            _request.Method = value;
            return this;
        }

        public CoapRequestBuilder WithPath(params string[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _request.Options.UriPath = string.Join("/", value);
            return this;
        }

        public CoapRequestBuilder WithPath(string value)
        {
            _request.Options.UriPath = value;
            return this;
        }

        public CoapRequestBuilder WithQuery(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (_request.Options.UriQuery == null)
            {
                _request.Options.UriQuery = new List<string>();
            }

            _request.Options.UriQuery.Add(value);
            return this;
        }

        public CoapRequestBuilder WithQuery(params string[] value)
        {
            _request.Options.UriQuery = value;
            return this;
        }

        public CoapRequestBuilder WithQuery(ICollection<string> value)
        {
            _request.Options.UriQuery = value;
            return this;
        }

        public CoapRequestBuilder WithPayload(byte[] value)
        {
            _request.Payload = new ArraySegment<byte>(value);
            return this;
        }

        public CoapRequestBuilder WithPayload(ArraySegment<byte> value)
        {
            _request.Payload = value;
            return this;
        }

        public CoapRequestBuilder WithPayload(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return WithPayload(Encoding.UTF8.GetBytes(value));
        }

        public CoapRequest Build()
        {
            return _request;
        }
    }
}
