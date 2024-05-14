using System.Threading.Tasks;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;

namespace Monaverse.Api.MonaHttpClient
{
    public interface IMonaHttpClient
    {
        Task<IMonaHttpResponse> SendAsync(IMonaHttpRequest request);
        string AccessToken { get; }
        void ClearSession();
        void SaveSession(string accessToken);
    }
}