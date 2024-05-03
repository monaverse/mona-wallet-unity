using System.Collections.Generic;
using System.Text;
using Monaverse.Api.MonaHttpClient.Request;

namespace Monaverse.Api.MonaHttpClient.Response
{
    public sealed class MonaHttpResponse : IMonaHttpResponse
    {
        private readonly IDictionary<string, string> _headers;

        public IMonaHttpRequest HttpRequest { get; }
        public string Url { get; }
        public int ResponseCode { get; }
        public byte[] Response { get; }
        public string Error { get; }

        public IEnumerable<KeyValuePair<string, string>> Headers => _headers;
        public bool IsSuccess => ResponseCode is >= 200 and < 400;

        public MonaHttpResponse(string url,
            int responseCode,
            byte[] response,
            IDictionary<string, string> headers,
            IMonaHttpRequest httpRequest = null,
            string error = null)
        {
            Url = url;
            ResponseCode = responseCode;
            Response = response;
            _headers = headers ?? new Dictionary<string, string>();
            HttpRequest = httpRequest;
            Error = error;
        }

        public bool GetHeader(string header, out string value)
            => _headers.TryGetValue(header, out value);

        public string GetResponseString(Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(Response);
        }
    }
}