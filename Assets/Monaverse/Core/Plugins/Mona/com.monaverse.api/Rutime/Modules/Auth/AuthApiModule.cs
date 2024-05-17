using System;
using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Auth.Responses;
using Monaverse.Api.Modules.Common;
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

        public async Task<ApiResult<PostNonceResponse>> PostNonce(string walletAddress)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.PostNonce),
                    method: RequestMethod.Post)
                .WithBody(new PostNonceRequest { WalletAddress = walletAddress });

            var response = await _monaHttpClient.SendAsync(monaHttpRequest);
            return response.ConvertTo<PostNonceResponse>();
        }

        public async Task<ApiResult<ValidateWalletResponse>> ValidateWallet(string walletAddress)
        {
            try
            {
                var postNonceResponse = await PostNonce(walletAddress);
                if (!postNonceResponse.IsSuccess)
                    return ApiResult<ValidateWalletResponse>.Failed(message: postNonceResponse.Message,
                        data: new ValidateWalletResponse(ValidateWalletResult.FailedGeneratingNonce, ErrorMessage: postNonceResponse.Message));

                if (!postNonceResponse.Data.IsExistingUser)
                    return ApiResult<ValidateWalletResponse>.Failed(message: "Wallet is not registered", 
                        data: new ValidateWalletResponse(ValidateWalletResult.WalletIsNotRegistered, ErrorMessage: postNonceResponse.Message));

                var siweMessage = SiweMessageBuilder.BuildMessage(domain: Constants.MonaDomain,
                    address: walletAddress,
                    nonce: postNonceResponse.Data.Nonce);

                return ApiResult<ValidateWalletResponse>.Success(new ValidateWalletResponse(ValidateWalletResult.WalletIsValid, siweMessage));
            }
            catch (Exception exception)
            {
                _monaApiLogger.LogError(exception.Message);
                return ApiResult<ValidateWalletResponse>.Failed(message: exception.Message,
                    data:new ValidateWalletResponse(ValidateWalletResult.Error, ErrorMessage: exception.Message));
            }
        }

        public async Task<ApiResult> Authorize(string signature, string siweMessage)
        {
            try
            {
                var monaHttpRequest = new MonaHttpRequest(
                        url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.PostAuthorize),
                        method: RequestMethod.Post)
                    .WithBody(new AuthorizeRequest
                    {
                        Signature = signature,
                        Message = siweMessage
                    });

                var response = await _monaHttpClient.SendAsync(monaHttpRequest);
                var result = response.ConvertTo<AuthorizeResponse>();

                if (result.Data != null && !string.IsNullOrEmpty(result.Data.AccessToken))
                    _monaHttpClient.SaveSession(result.Data.AccessToken);

                return result;
            }
            catch (Exception exception)
            {
                _monaApiLogger.LogError(exception.Message);
                return ApiResult.Failed(exception.Message);
            }
        }
    }
}