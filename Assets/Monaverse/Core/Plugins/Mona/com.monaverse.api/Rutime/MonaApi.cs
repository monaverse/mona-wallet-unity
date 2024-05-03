using Monaverse.Api.Options;
using UnityEngine;

namespace Monaverse.Api
{
    public static class MonaApi
    {
        private static IMonaApiClient ApiClient { get; set; }

        public static IMonaApiClient Init(string applicationId)
        {
            return Init(new DefaultApiOptions
            {
                ApplicationId = applicationId
            });
        }

        public static IMonaApiClient Init(IMonaApiOptions monaApiOptions)
        {
            ApiClient = new MonaApiClientImpl(monaApiOptions);
            return ApiClient;
        }
        
        public static IMonaApiClient GetApiClient()
        {
            if (ApiClient != null) return ApiClient;
            
            Debug.LogError("You must call Init before using the API");
            return null;
        }
    }
}