using System.Threading.Tasks;
using Monaverse.Api.MonaHttpClient.Handlers;
using Monaverse.Api.MonaHttpClient.Logger;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;

namespace Monaverse.Api.MonaHttpClient
{
    public sealed class DefaultHttpClient : IMonaHttpClient
    {
        private readonly IWebRequestHandler _webRequestHandler;
        public string AccessToken { get; set; }

        public DefaultHttpClient(IHttpLogger httpLogger)
        {
            _webRequestHandler = new AsyncWebRequestHandler(httpLogger);
        }
        
        public Task<IMonaHttpResponse> SendAsync(IMonaHttpRequest request)
            => _webRequestHandler.SendWebRequest(request);
    }
}