using System.Threading.Tasks;
using Monaverse.MonaHttpClient.Request;
using Monaverse.MonaHttpClient.Response;

namespace Monaverse.MonaHttpClient.Handlers
{
    internal interface IWebRequestHandler
    {
        Task<IMonaHttpResponse> SendWebRequest(IMonaHttpRequest request);
    }
}