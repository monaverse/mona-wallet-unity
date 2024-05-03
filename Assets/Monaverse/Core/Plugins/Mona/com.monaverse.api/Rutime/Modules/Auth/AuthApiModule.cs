using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Modules.Auth.Responses;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.Options;

namespace Monaverse.Api.Modules.Auth
{
    internal sealed class AuthApiModule : IAuthApiModule
    {
        private readonly IMonaApiOptions _monaApiOptions;
        private readonly IMonaHttpClient _monaHttpClient;

        public AuthApiModule(IMonaApiOptions monaApiOptions, 
            IMonaHttpClient monaHttpClient)
        {
            _monaApiOptions = monaApiOptions;
            _monaHttpClient = monaHttpClient;
        }
        
        public async Task<PostNonceResponse> PostNonce(string walletAddress)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.PostNonce),
                    method: RequestMethod.Post)
                .WithBody(new { walletAddress = walletAddress });

            var response = await _monaHttpClient.SendAsync(monaHttpRequest);
            return response.ConvertTo<PostNonceResponse>();
        }
        
        public async Task<AuthorizeResponse> Authorize(string signature, string siweMessage)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.PostAuthorize),
                    method: RequestMethod.Post)
                .WithBody(new
                {
                    signature = signature,
                    message = siweMessage
                });

            var response = await _monaHttpClient.SendAsync(monaHttpRequest);
            var result = response.ConvertTo<AuthorizeResponse>();
            _monaHttpClient.AccessToken = result.AccessToken;

            return result;
        }
    }
}