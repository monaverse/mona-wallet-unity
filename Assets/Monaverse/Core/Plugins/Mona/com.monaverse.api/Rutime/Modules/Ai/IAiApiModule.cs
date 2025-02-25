using System.Threading.Tasks;
using Monaverse.Api.Modules.Ai.Enums;
using Monaverse.Api.Modules.Ai.Requests;
using Monaverse.Api.Modules.Ai.Responses;
using Monaverse.Api.Modules.Common;

namespace Monaverse.Api.Modules.Ai
{
    public interface IAiApiModule
    {
        Task<ApiResult<GetGenerationRequestResponse>> GetGenerationRequest(string requestId);
        Task<ApiResult<GetAssetByIdResponse>> GetAssetById(string assetId);
        Task<ApiResult<GetRequestsByUserResponse>> GetRequestsByUser(
            StatusFilter? status = null,
            StepTypeFilter? stepType = null,
            AssetTypeFilter? desiredOutputType = null,
            int limit = 100,
            int offset = 0
        );
        Task<ApiResult<GetAssetsByUserResponse>> GetAssetsByUser(
            AssetTypeFilter? assetType = null,
            int limit = 100,
            int offset = 0
        );
        Task<ApiResult<CreateTextToImageRequestResponse>> CreateTextToImageRequest(CreateTextToImageRequestRequest request);
        Task<ApiResult<CreateImageTo3dRequestResponse>> CreateImageTo3dRequest(CreateImageTo3dRequestRequest request);
        Task<ApiResult<GetUserQuotaResponse>> GetUserQuota();
    }
}