using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Auth.Responses;
using Monaverse.Api.Modules.Common;

namespace Monaverse.Api.Modules.Auth
{
    public interface IAuthApiModule
    {
        Task<ApiResult> SignUp(SignUpRequest request);
        Task<ApiResult> GenerateOtp(GenerateOtpRequest request);
        Task<ApiResult<VerifyOtpResponse>> VerifyOtp(VerifyOtpRequest request);
        Task<ApiResult<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest request);
    }
}