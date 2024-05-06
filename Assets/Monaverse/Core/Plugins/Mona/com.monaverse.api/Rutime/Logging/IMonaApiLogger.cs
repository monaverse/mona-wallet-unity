namespace Monaverse.Api.Logging
{
    public interface IMonaApiLogger
    {
        void Log(object message);
        void LogWarning(object message);
        void LogError(object message);
    }
}