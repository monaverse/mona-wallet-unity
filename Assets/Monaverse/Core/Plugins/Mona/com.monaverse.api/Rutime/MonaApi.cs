using Monaverse.Api.Options;

namespace Monaverse.Api
{
    public static class MonaApi
    {
        public static IMonaApiClient ApiClient { get; private set; }

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
    }
}