using System;
using System.Collections.Generic;
using System.Text;

namespace Monaverse.Api.MonaHttpClient.Request
{
    public sealed class MonaHttpRequest : IMonaHttpRequest
    {
        private readonly IDictionary<string, string> _headers;
        private readonly IDictionary<string, string> _queryParams;

        public string Url { get; }
        public RequestMethod Method { get; }
        public byte[] Body { get; private set; }
        public int Timeout { get; set; } = 30;
        
        public IEnumerable<KeyValuePair<string, string>> Headers => _headers;
        public IEnumerable<KeyValuePair<string, string>> QueryParams => _queryParams;

        public MonaHttpRequest(string url, RequestMethod method)
        {
            Url = url;
            Method = method;

            _headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            _queryParams = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // fix a bug that appears in some older versions of Unity
            WithHeader("X-HTTP-Method-Override", method.ToString().ToUpperInvariant());
        }

        public IMonaHttpRequest WithHeader(string header, string value)
        {
            if (string.IsNullOrEmpty(value))
                _headers.Remove(header);
            else
                _headers[header] = value;

            return this;
        }

        public IMonaHttpRequest WithQueryParam(string key, object value)
        {
            string valueStr = null;

            if (value != null)
                valueStr = value.ToString();

            if (string.IsNullOrEmpty(valueStr))
                _queryParams.Remove(key);
            else
                _queryParams[key] = valueStr;

            return this;
        }

        public IMonaHttpRequest WithAcceptType(string contentType)
        {
            _headers.TryGetValue(Constants.AcceptHeader, out var acceptValues);

            if (string.IsNullOrEmpty(acceptValues))
                _headers[Constants.AcceptHeader] = contentType;
            else
                _headers[Constants.AcceptHeader] = $"{acceptValues},{contentType}";

            return this;
        }

        public IMonaHttpRequest WithBearerToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                _headers.Remove(Constants.AuthorizationHeader);
            else
                WithHeader(Constants.AuthorizationHeader, $"{Constants.BearerToken} {token}");

            return this;
        }

        public IMonaHttpRequest WithJsonBody(string jsonBody)
        {
            if (Method == RequestMethod.Delete)
                throw new InvalidOperationException($"cannot have a body for method {Method}");

            Body = Encoding.UTF8.GetBytes(jsonBody);

            // note: we set the charset for security reasons even though utf-8 
            // is the default/implied charset for application/json
            WithHeader(Constants.ContentTypeHeader, $"{Constants.JsonContentType}; {Constants.DefaultCharSet}");

            return this;
        }

        public string GetFullUrl()
        {
            var result = Url;

            if (_queryParams.Count <= 0) return result;

            result += "?";
            var sb = new StringBuilder();

            foreach (var kvp in _queryParams)
            {
                if (sb.Length > 0)
                    sb.Append("&");

                sb.AppendFormat($"{kvp.Key}={kvp.Value}");
            }

            result += sb.ToString();
            result = Uri.EscapeDataString(result);

            return result;
        }

        public string GetContentType()
        {
            _headers.TryGetValue(Constants.ContentTypeHeader,out var contentType) ;
            return contentType ?? Constants.DefaultContentType ;
        }
    }
}