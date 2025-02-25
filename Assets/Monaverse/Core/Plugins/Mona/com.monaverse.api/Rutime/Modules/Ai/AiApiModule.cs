using System.Threading.Tasks;
using Monaverse.Api.Modules.Ai.Enums;
using Monaverse.Api.Modules.Ai.Requests;
using Monaverse.Api.Modules.Ai.Responses;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Request;

namespace Monaverse.Api.Modules.Ai
{
    internal class AiApiModule : IAiApiModule
    {
        private readonly IMonaApiClient _monaApiClient;

        public AiApiModule(IMonaApiClient monaApiClient)
        {
            _monaApiClient = monaApiClient;
        }

        public async Task<ApiResult<GetGenerationRequestResponse>> GetGenerationRequest(string requestId)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Ai.GetGenerationRequest(requestId)),
                method: RequestMethod.Get
            );

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetGenerationRequestResponse>();
        }

        public async Task<ApiResult<GetAssetByIdResponse>> GetAssetById(string assetId)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Ai.GetAssetById(assetId)),
                method: RequestMethod.Get
            );

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetAssetByIdResponse>();
        }

        public async Task<ApiResult<GetRequestsByUserResponse>> GetRequestsByUser(
            StatusFilter? status = null,
            StepTypeFilter? stepType = null,
            AssetTypeFilter? desiredOutputType = null,
            int limit = 100,
            int offset = 0
        )
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Ai.GetRequestsByUser),
                method: RequestMethod.Get);

            if (status.HasValue)
            {
                monaHttpRequest.WithQueryParam("status", status.Value.ConvertToString());
            }

            if (stepType.HasValue)
            {
                monaHttpRequest.WithQueryParam("step_type", stepType.Value.ConvertToString());
            }

            if (desiredOutputType.HasValue)
            {
                monaHttpRequest.WithQueryParam("desired_output_type", desiredOutputType.Value.ConvertToString());
            }

            monaHttpRequest
                .WithQueryParam("limit", limit)
                .WithQueryParam("offset", offset);
            
            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetRequestsByUserResponse>();
        }

        public async Task<ApiResult<GetAssetsByUserResponse>> GetAssetsByUser(
            AssetTypeFilter? assetType = null,
            int limit = 100,
            int offset = 0
        )
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Ai.GetAssetsByUser),
                method: RequestMethod.Get);
                
            if (assetType.HasValue)
            {
                monaHttpRequest.WithQueryParam("asset_type", assetType.Value.ConvertToString());
            }
            
            monaHttpRequest
                .WithQueryParam("limit", limit)
                .WithQueryParam("offset", offset);

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetAssetsByUserResponse>();
        }

        public async Task<ApiResult<CreateTextToImageRequestResponse>> CreateTextToImageRequest(CreateTextToImageRequestRequest request)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Ai.CreateTextToImageRequest),
                    method: RequestMethod.Post)
                .WithBody(request);

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<CreateTextToImageRequestResponse>();
        }

        public async Task<ApiResult<CreateImageTo3dRequestResponse>> CreateImageTo3dRequest(CreateImageTo3dRequestRequest request)
        {
            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Ai.CreateImageTo3dRequest),
                    method: RequestMethod.Post)
                .WithBody(request);

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<CreateImageTo3dRequestResponse>();
        }

        public async Task<ApiResult<GetUserQuotaResponse>> GetUserQuota()
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Ai.GetUserQuota),
                method: RequestMethod.Get
            );

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetUserQuotaResponse>();
        }
    }
}