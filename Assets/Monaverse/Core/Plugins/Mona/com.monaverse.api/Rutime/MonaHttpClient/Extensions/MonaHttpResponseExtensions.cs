using System;
using System.Text;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.MonaHttpClient.Response;
using Newtonsoft.Json;

namespace Monaverse.Api.MonaHttpClient.Extensions
{
    public static class MonaHttpResponseExtensions
    {
        public static ApiResult<T> ConvertTo<T>(this IMonaHttpResponse response)
        {
            if (!response.IsSuccess) return ApiResult<T>.Failed(response.Error);

            if (typeof(T) == typeof(IMonaHttpResponse))
                return ApiResult<T>.Success((T)Convert.ChangeType(response, typeof(T)));

            if (typeof(T) == typeof(MonaHttpResponse))
                return ApiResult<T>.Success((T)Convert.ChangeType(response, typeof(T)));

            if (typeof(T) == typeof(string))
                return ApiResult<T>.Success((T)Convert.ChangeType(response.GetResponseString(), typeof(T)));

            var json = response.GetResponseString();

            var data = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return ApiResult<T>.Success(data, json);
        }

        public static ApiResult ToApiResult(this IMonaHttpResponse response)
        {
            return !response.IsSuccess ? ApiResult.Failed(response.Error) : ApiResult.Success();
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

                if (!string.IsNullOrEmpty(value) &&
                    (value.StartsWith(Constants.JsonContentType) 
                     || value.StartsWith(Constants.TextContentType)
                     || value.StartsWith(Constants.HtmlContentType)))
                    result.Append(httpResponse.GetResponseString());

                result.Append("\n\r");
            }

            return result.ToString();
        }
    }
}