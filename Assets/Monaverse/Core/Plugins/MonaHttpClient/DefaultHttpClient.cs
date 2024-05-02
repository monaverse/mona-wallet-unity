using System.Threading.Tasks;
using Monaverse.MonaHttpClient.Handlers;
using Monaverse.MonaHttpClient.Logger;
using Monaverse.MonaHttpClient.Request;
using Monaverse.MonaHttpClient.Response;

namespace Monaverse.MonaHttpClient
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