using System;
using System.Threading.Tasks;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Auth.Responses;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Request;

namespace Monaverse.Api.Modules.Auth
{
    internal sealed class AuthApiModule : IAuthApiModule
    {
        private readonly IMonaApiClient _monaApiClient;
        private readonly IMonaApiLogger _monaApiLogger;

        public AuthApiModule(IMonaApiClient monaApiClient,
            IMonaApiLogger monaApiLogger)
        {
            _monaApiClient = monaApiClient;
            _monaApiLogger = monaApiLogger;
        }

        public async Task<ApiResult> GenerateOtp(GenerateOtpRequest request)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Auth.GenerateOtp),
                    method: RequestMethod.Post)
                .WithBody(request);
            
            var response = await _monaApiClient.Send(monaHttpRequest);
            return response.ToApiResult();
        }
        
        public async Task<ApiResult<VerifyOtpResponse>> VerifyOtp(VerifyOtpRequest request)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Auth.VerifyOtp),
                    method: RequestMethod.Post)
                .WithBody(request);
            
            var response = await _monaApiClient.Send(monaHttpRequest);
            var result = response.ConvertTo<VerifyOtpResponse>();
            
            if(result.IsSuccess)
                _monaApiClient.SaveSession(result.Data.Access, result.Data.Refresh);
            
            return result;
        }
        
        public async Task<ApiResult<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest request)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Auth.RefreshToken),
                    method: RequestMethod.Post)
                .WithBody(request);
            
            var response = await _monaApiClient.Send(monaHttpRequest);
            var result = response.ConvertTo<RefreshTokenResponse>();
            
            if(result.IsSuccess)
                _monaApiClient.SaveSession(result.Data.Access, result.Data.Refresh);

            return result;
        }


        #region Legacy Endpoints
        
        public async Task<ApiResult<PostNonceResponse>> PostNonce(string walletAddress)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPathLegacy(Constants.Endpoints.PostNonce),
                    method: RequestMethod.Post)
                .WithBody(new PostNonceRequest { WalletAddress = walletAddress });

            var response = await _monaApiClient.SendLegacy(monaHttpRequest);
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
                        url: _monaApiClient.GetUrlWithPathLegacy(Constants.Endpoints.PostAuthorize),
                        method: RequestMethod.Post)
                    .WithBody(new AuthorizeRequest
                    {
                        Signature = signature,
                        Message = siweMessage
                    });

                var response = await _monaApiClient.SendLegacy(monaHttpRequest);
                var result = response.ConvertTo<AuthorizeResponse>();

                if (result.Data != null && !string.IsNullOrEmpty(result.Data.AccessToken))
                    _monaApiClient.SaveLegacySession(result.Data.AccessToken);

                return result;
            }
            catch (Exception exception)
            {
                _monaApiLogger.LogError(exception.Message);
                return ApiResult.Failed(exception.Message);
            }
        }

        #endregion
    }
}