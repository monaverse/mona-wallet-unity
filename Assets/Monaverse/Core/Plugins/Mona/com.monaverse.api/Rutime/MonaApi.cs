using Monaverse.Api.Logging;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.Options;
using Monaverse.Api.Session;
using UnityEngine;

namespace Monaverse.Api
{
    public static class MonaApi
    {
        private static IMonaApiClient _apiClient;

        public static IMonaApiClient ApiClient
        {
            get
            {
                if (_apiClient != null) return _apiClient;
                Debug.LogError("You must call Init before using the API");
                return null;
            }
        }

        public static IMonaApiClient Init(string applicationId)
        {
            return Init(new DefaultApiOptions
            {
                ApplicationId = applicationId
            });
        }

        public static IMonaApiClient Init(IMonaApiOptions monaApiOptions)
        {
            var monaApiLogger = new UnityMonaApiLogger(monaApiOptions.LogLevel);
            var monaHttpClient = new UnityAsyncHttpClient(monaApiLogger);
            var monaApiSession = new TokenSession();
            _apiClient = new MonaApiClientImpl(monaApiOptions, 
                monaApiLogger, 
                monaHttpClient, 
                monaApiSession);
            
            return _apiClient;
        }
        
        public static IMonaApiClient Init(IMonaApiOptions monaApiOptions,
            IMonaApiLogger monaApiLogger,
            IMonaHttpClient monaHttpClient)
        {
            _apiClient = new MonaApiClientImpl(monaApiOptions, monaApiLogger, monaHttpClient, new TokenSession());
            return _apiClient;
        }
    }
}