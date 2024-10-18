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


        public async Task<ApiResult> SignUp(SignUpRequest request)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Auth.SignUp),
                    method: RequestMethod.Post)
                .WithBody(request);
            
            var response = await _monaApiClient.Send(monaHttpRequest);
            return response.ToApiResult();
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
                _monaApiClient.Session.SaveSession(result.Data.Access, result.Data.Refresh);
            
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
                _monaApiClient.Session.SaveSession(result.Data.Access, result.Data.Refresh);
            else
                _monaApiClient.Session.ClearSession();

            return result;
        }
    }
}