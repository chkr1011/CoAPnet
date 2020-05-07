using System;
using System.Collections.Generic;

namespace CoAPnet.Client
{
    public class CoapObserveOptionsBuilder
    {
        readonly CoapObserveOptions _options = new CoapObserveOptions
        {
            Request = new CoapObserveRequest()
        };

        public CoapObserveOptionsBuilder WithPath(string value)
        {
            _options.Request.Options.UriPath = value;
            return this;
        }

        public CoapObserveOptionsBuilder WithQuery(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            if (_options.Request.Options.UriQuery == null)
            {
                _options.Request.Options.UriQuery = new List<string>();
            }

            _options.Request.Options.UriQuery.Add(value);
            return this;
        }

        public CoapObserveOptionsBuilder WithQuery(params string[] value)
        {
            _options.Request.Options.UriQuery = value;
            return this;
        }

        public CoapObserveOptionsBuilder WithQuery(ICollection<string> value)
        {
            _options.Request.Options.UriQuery = value;
            return this;
        }

        public CoapObserveOptionsBuilder WithResponseHandler(ICoapResponseHandler value)
        {
            _options.ResponseHandler = value;
            return this;
        }

        public CoapObserveOptions Build()
        {
            if (_options.ResponseHandler == null)
            {
                throw new InvalidOperationException("No handler is set.");
            }

            return _options;
        }
    }
}
