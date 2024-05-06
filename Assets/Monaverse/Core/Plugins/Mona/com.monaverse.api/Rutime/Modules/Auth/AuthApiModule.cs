using System;
using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Logging;
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
        private readonly IMonaApiLogger _monaApiLogger;
        private readonly IMonaHttpClient _monaHttpClient;

        public AuthApiModule(IMonaApiOptions monaApiOptions,
            IMonaApiLogger monaApiLogger,
            IMonaHttpClient monaHttpClient)
        {
            _monaApiOptions = monaApiOptions;
            _monaApiLogger = monaApiLogger;
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
        
        public async Task<ValidateWalletAddressResponse> ValidateWalletAddress(string walletAddress)
        {
            try
            {
                var postNonceResponse = await PostNonce(walletAddress);
                if(postNonceResponse == null)
                    return new ValidateWalletAddressResponse(ValidateWalletResult.FailedGeneratingNonce);
            
                if (!postNonceResponse.IsExistingUser)
                    return new ValidateWalletAddressResponse(ValidateWalletResult.WalletIsNotRegistered);

                var siweMessage = SiweMessageBuilder.BuildMessage(domain: Constants.MonaDomain,
                    address: walletAddress,
                    nonce: postNonceResponse.Nonce);
            
                return new ValidateWalletAddressResponse(ValidateWalletResult.WalletIsValid, siweMessage);
            }
            catch (Exception exception)
            {
                return new ValidateWalletAddressResponse(
                    Result: ValidateWalletResult.Error,
                    SiweMessage: null,
                    ErrorMessage: exception.Message);
            }
        }
        
        public async Task<bool> Authorize(string signature, string siweMessage)
        {
            try
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
                
                return true;
            }
            catch (Exception exception)
            {
                _monaApiLogger.LogError(exception.Message);
                
                return false;
            }
           
        }
    }
}