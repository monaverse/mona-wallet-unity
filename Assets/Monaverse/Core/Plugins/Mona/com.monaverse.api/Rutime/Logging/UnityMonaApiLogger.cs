using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Logger;
using Monaverse.Api.MonaHttpClient.Response;
using UnityEngine;

namespace Monaverse.Api.Logging
{
    public sealed class UnityMonaApiLogger : IHttpLogger, IMonaApiLogger
    {
        private readonly ApiLogLevel _logLevel;
        private const string LogPrefix = "[MonaverseApi] ";

        public UnityMonaApiLogger(ApiLogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public void Log(object message)
        {
            if(_logLevel < ApiLogLevel.Info)
                return;
            
            Debug.Log(LogPrefix + message);
        }

        public void LogWarning(object message)
        {
            if(_logLevel < ApiLogLevel.Warn)
                return;
            
            Debug.LogWarning(LogPrefix + message);
        }

        public void LogError(object message)
        {
            if(_logLevel < ApiLogLevel.Error)
                return;
            
            Debug.LogError(LogPrefix + message);
        }

        public void LogResponse(IMonaHttpResponse response)
            => Log(response.ToLog());
    }
}