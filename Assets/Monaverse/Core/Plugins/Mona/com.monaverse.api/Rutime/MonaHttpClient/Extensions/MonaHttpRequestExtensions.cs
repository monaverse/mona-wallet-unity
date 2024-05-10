using System.Collections.Generic;
using Monaverse.Api.MonaHttpClient.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Monaverse.Api.MonaHttpClient.Extensions
{
    public static class MonaHttpRequestExtensions
    {
        public static IMonaHttpRequest WithHeaders(this IMonaHttpRequest request,
            IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var kvp in headers)
                request.WithHeader(kvp.Key, kvp.Value);

            return request;
        }

        public static IMonaHttpRequest WithQueryParams(this IMonaHttpRequest request,
            IEnumerable<KeyValuePair<string, object>> queryParams)
        {
            foreach (var kvp in queryParams)
                request.WithQueryParam(kvp.Key, kvp.Value);

            return request;
        }

        public static IMonaHttpRequest WithBody(this IMonaHttpRequest request,
            object body)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            
            var jsonBody = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
            });
            
            request.WithJsonBody(jsonBody);

            return request;
        }
    }
}