using System.Threading.Tasks;
using Monaverse.MonaHttpClient.Request;
using Monaverse.MonaHttpClient.Response;

namespace Monaverse.MonaHttpClient
{
    public interface IMonaHttpClient
    {
        Task<IMonaHttpResponse> SendAsync(IMonaHttpRequest request);
        string AccessToken { get; set; }
    }
}