using Monaverse.MonaHttpClient.Response;

namespace Monaverse.MonaHttpClient.Logger
{
    public interface IHttpLogger
    {
        void LogResponse(IMonaHttpResponse response);
    }
}