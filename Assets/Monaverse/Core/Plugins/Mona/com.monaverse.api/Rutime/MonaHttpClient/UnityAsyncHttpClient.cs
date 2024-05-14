using System.Threading.Tasks;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Logger;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;
using UnityEngine.Networking;

namespace Monaverse.Api.MonaHttpClient
{
    public class UnityAsyncHttpClient : IMonaHttpClient
    {
        private readonly IHttpLogger _httpLogger;
        public string AccessToken { get; protected set; }
      
        public UnityAsyncHttpClient(IHttpLogger httpLogger)
        {
            _httpLogger = httpLogger;
        }

        public virtual async Task<IMonaHttpResponse> SendAsync(IMonaHttpRequest request)
        {
            var uwr = new UnityWebRequest
            {
                method = request.Method.ToString(),
                url = request.GetFullUrl()
            };

            foreach (var kvp in request.Headers)
                uwr.SetRequestHeader(kvp.Key, kvp.Value);

            if (request.Body != null)
            {
                uwr.uploadHandler = new UploadHandlerRaw(request.Body)
                {
                    contentType = request.GetContentType(),
                };
            }

            uwr.timeout = request.Timeout;
            uwr.downloadHandler = new DownloadHandlerBuffer();

            await uwr.SendWebRequestAsync();

            var response = uwr.ToMonaHttpResponse(request);
            _httpLogger.LogResponse(response);

            return response;
        }
        
        public virtual void ClearSession()
        {
            AccessToken = null;
        }

        public virtual void SaveSession(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}