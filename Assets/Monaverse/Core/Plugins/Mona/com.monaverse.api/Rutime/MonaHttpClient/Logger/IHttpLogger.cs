using Monaverse.Api.MonaHttpClient.Response;

namespace Monaverse.Api.MonaHttpClient.Logger
{
    public interface IHttpLogger
    {
        void Log(object message);
        void LogWarning(object message);
        void LogError(object message);
        void LogResponse(IMonaHttpResponse response);
    }
}