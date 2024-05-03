using System.Threading.Tasks;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;

namespace Monaverse.Api.MonaHttpClient.Handlers
{
    internal interface IWebRequestHandler
    {
        Task<IMonaHttpResponse> SendWebRequest(IMonaHttpRequest request);
    }
}