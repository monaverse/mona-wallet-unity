using System.Collections.Generic;

namespace Monaverse.Api.MonaHttpClient.Request
{
    public interface IMonaHttpRequest
    {
        string Url { get; }
        RequestMethod Method { get; }
        int Timeout { get; }
        byte[] Body { get; }
        IEnumerable<KeyValuePair<string, string>> Headers { get; }
        IEnumerable<KeyValuePair<string, string>> QueryParams { get; }
        string GetFullUrl();
        string GetContentType();
        IMonaHttpRequest WithHeader(string header, string value);
        IMonaHttpRequest WithQueryParam(string key, object value);
        IMonaHttpRequest WithAcceptType(string contentType);
        IMonaHttpRequest WithBearerToken(string token);
        IMonaHttpRequest WithJsonBody(string jsonBody);
    }
}