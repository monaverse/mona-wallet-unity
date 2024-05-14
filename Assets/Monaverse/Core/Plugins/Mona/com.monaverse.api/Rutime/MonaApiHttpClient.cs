using System.Threading.Tasks;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.MonaHttpClient.Logger;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;

namespace Monaverse.Api
{
    public sealed class MonaApiHttpClient : UnityAsyncHttpClient
    {
        private readonly string _applicationId;

        public MonaApiHttpClient(IHttpLogger httpLogger, string applicationId) : base(httpLogger)
        {
            _applicationId = applicationId;
            var accessToken = PlayerPrefsObfuscator.Load(Constants.AccessTokenStorageKey);
            if (!string.IsNullOrEmpty(accessToken))
                AccessToken = accessToken;
        }

        public override async Task<IMonaHttpResponse> SendAsync(IMonaHttpRequest request)
        {
            // Add the application id to every request
            request.WithHeader(Constants.ApplicationIdHeader, _applicationId);

            // Add the access token if there is one
            if (!string.IsNullOrEmpty(AccessToken))
                request.WithBearerToken(AccessToken);

            return await base.SendAsync(request);
        }
        
        public override void ClearSession()
        {
            base.ClearSession();
            PlayerPrefsObfuscator.Delete(Constants.AccessTokenStorageKey);
        }

        public override void SaveSession(string accessToken)
        {
            base.SaveSession(accessToken);
            
            if(!string.IsNullOrEmpty(AccessToken))
                PlayerPrefsObfuscator.Save(Constants.AccessTokenStorageKey, AccessToken);
            else
                ClearSession();
        }
    }
}