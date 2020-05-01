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

        public CoapRequestBuilder WithPath(string value)
        {
            _request.UriPath = value;
            return this;
        }

        public CoapRequestBuilder WithQuery(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            if (_request.UriQuery == null)
            {
                _request.UriQuery = new List<string>();
            }

            _request.UriQuery.Add(value);
            return this;
        }

        public CoapRequestBuilder WithQuery(params string[] value)
        {
            _request.UriQuery = value;
            return this;
        }

        public CoapRequestBuilder WithQuery(ICollection<string> value)
        {
            _request.UriQuery = value;
            return this;
        }

        public CoapRequestBuilder WithPayload(ArraySegment<byte> value)
        {
            _request.Payload = value;
            return this;
        }

        public CoapRequestBuilder WithPayload(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            _request.Payload = new ArraySegment<byte>(Encoding.UTF8.GetBytes(value));
            return this;
        }

        public CoapRequest Build()
        {
            return _request;
        }
    }
}
