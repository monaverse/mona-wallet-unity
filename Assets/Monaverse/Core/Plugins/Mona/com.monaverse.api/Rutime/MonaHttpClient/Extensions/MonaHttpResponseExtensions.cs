using System;
using System.Net.Http;
using System.Text;
using Monaverse.Api.MonaHttpClient.Response;
using Newtonsoft.Json;

namespace Monaverse.Api.MonaHttpClient.Extensions
{
    public static class MonaHttpResponseExtensions
    {
        public static T ConvertTo<T>(this IMonaHttpResponse response)
        {
            if (response.IsSuccess)
            {
                if (typeof(T) == typeof(IMonaHttpResponse))
                    return (T)Convert.ChangeType(response, typeof(T));

                if (typeof(T) == typeof(MonaHttpResponse))
                    return (T)Convert.ChangeType(response, typeof(T));

                if (typeof(T) == typeof(string))
                    return (T)Convert.ChangeType(response.GetResponseString(), typeof(T));
                
                return JsonConvert.DeserializeObject<T>(response.GetResponseString());
            }

            Exception ex = new HttpRequestException($"error processing HTTP request for {response.HttpRequest.Method} {response.Url}: {response.ResponseCode}");
            throw ex;
        }


        public static string ToLog(this IMonaHttpResponse httpResponse)
        {
            var result = new StringBuilder();
            var uri = new Uri(httpResponse.Url);

            var httpRequest = httpResponse.HttpRequest;

            result.Append($"{httpResponse.ResponseCode} {httpResponse.ResponseCode} {httpRequest.Method} {uri.AbsolutePath} HTTP \n\r");
            result.Append($"Host: {uri.Host}\n\r");
            
            if (httpResponse.Error is not null)
            {
                result.Append($"Error: {httpResponse.Error}\n\r");
            }

            if (httpResponse.Response is not null)
            {
                result.Append($"Content-Length: {httpResponse.Response.Length}\n\r");
            }

            foreach (var header in httpResponse.Headers)
            {
                result.Append($"{header.Key}: {header.Value}\n\r");
            }

            if (httpResponse.Response is not null)
            {
                result.Append("\n\r-------- BODY --------\n\r");

                httpResponse.GetHeader(Constants.ContentTypeHeader, out string value);

                if (!string.IsNullOrEmpty(value) && value.StartsWith(Constants.JsonContentType))
                    result.Append(httpResponse.GetResponseString());

                result.Append("\n\r");
            }

            return result.ToString();
        }
    }
}