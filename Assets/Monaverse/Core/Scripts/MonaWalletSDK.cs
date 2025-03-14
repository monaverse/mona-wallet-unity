using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Monaverse.Api;
using Monaverse.Api.Configuration;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Ai.Enums;
using Monaverse.Api.Modules.Ai.Requests;
using Monaverse.Api.Modules.Ai.Responses;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Leaderboard;
using Monaverse.Api.Modules.Leaderboard.Enums;
using Monaverse.Api.Modules.Leaderboard.Responses;
using Monaverse.Api.Modules.Leaderboard.Responses.Common;
using Monaverse.Api.Modules.Token.Responses;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Api.Modules.User.Responses;
using Monaverse.Api.Options;
using Monaverse.Core.Scripts.Utils;
using Monaverse.Core.Utils;

namespace Monaverse.Core
{
    /// <summary>
    /// The entry point for the Monaverse Wallet SDK
    /// </summary>
    public sealed class MonaWalletSDK
    {
        public struct SDKOptions
        {
            /// <summary>
            /// Monaverse Application ID.
            /// </summary>
            public string applicationId;

            /// <summary>
            /// The Monaverse API environment to use.
            /// </summary>
            public ApiEnvironment apiEnvironment;

            /// <summary>
            /// Whether to show the sdk debug logs
            /// </summary>
            public bool showDebugLogs;

            /// <summary>
            /// 
            /// </summary>
            public SupportedChainId defaultChain;
        }

        public enum SupportedChainId
        {
            Ethereum = 1,
            Polygon = 137,
            Arbitrum = 42161,
            Optimism = 10,
            Base = 8453
        }

        public SDKOptions Options { get; private set; }
        public IMonaApiClient ApiClient { get; internal set; }
        public MonaverseSession Session { get; private set; }

        public static SynchronizationContext UnitySyncContext { get; private set; }

        /// <summary>
        /// Event raised when the user is logged out
        /// </summary>
        public event EventHandler LoggedOut;

        /// <summary>
        /// Event raised when the user is authenticated with the Monaverse API
        /// </summary>
        public event EventHandler Authenticated;

        /// <summary>
        /// Event raised when the user's wallet is not authorized
        /// An authorization result is passed as a parameter
        /// </summary>
        public event EventHandler<string> AuthenticationFailed;

        private string _secret;

        public MonaWalletSDK(SDKOptions options)
        {
            if (string.IsNullOrEmpty(options.applicationId))
                MonaDebug.LogError("You must provide a Mona Application Id. Please visit https://studio.monaverse.com to get one.");

            Options = options;

            ApiClient = MonaApi.Init(new DefaultApiOptions
            {
                ApplicationId = options.applicationId,
                Environment = options.apiEnvironment,
                LogLevel = options.showDebugLogs ? ApiLogLevel.Info : ApiLogLevel.Off
            });

            Session = new MonaverseSession(ApiClient.Session.AccessToken, ApiClient.Session.RefreshToken);
            Session.Load();

            Session.SaveDefaultChainId((int)options.defaultChain);

            ApiClient.Session.OnClearSession += () =>
            {
                Session.Clear();
                OnLoggedOut();
            };

            var currentSyncContext = SynchronizationContext.Current;
            if (currentSyncContext.GetType().FullName != "UnityEngine.UnitySynchronizationContext")
                throw new Exception(
                    $"[Monaverse] SynchronizationContext is not of type UnityEngine.UnitySynchronizationContext. Current type is <i>{currentSyncContext.GetType().FullName}</i>. Make sure to initialize the Monaverse SDK from the main thread.");
            UnitySyncContext = currentSyncContext;
        }

        /// <summary>
        /// Creates a new user with the provided email, username and name with the Monaverse platform
        /// </summary>
        /// <param name="email"> The email of the user. Must be unique </param>
        /// <param name="username"> The username of the user. Must be unique</param>
        /// <param name="name"> The name of the user. </param>
        /// <returns> If successful, an account is created and a one-time password is sent to the provided email
        /// Returns a 409 error if the username already exists
        /// Returns a 409 error if the email already exists
        /// </returns>
        public async Task<ApiResult> SignUp(string email, string username, string name)
        {
            try
            {
                var result = await ApiClient.Auth
                    .SignUp(new SignUpRequest
                    {
                        Email = email,
                        Username = username,
                        Name = name
                    });

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Generates a one time password and sends it to the provided email
        /// If the user is not registered, an email asking for registration will be sent
        /// </summary>
        /// <param name="email"> The email to send the one time password to </param>
        /// <returns> true if the operation was successful </returns>
        public async Task<bool> GenerateOneTimePassword(string email)
        {
            try
            {
                var result = await ApiClient.Auth
                    .GenerateOtp(new GenerateOtpRequest
                    {
                        Email = email
                    });

                return result.IsSuccess;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return false;
            }
        }

        /// <summary>
        /// Verifies a one time password
        /// if successful, the user will be logged in and the session will be saved
        /// Once the session is saved, the Authenticated event will be raised
        /// </summary>
        /// <param name="email"> The registered email </param>
        /// <param name="otp"> The one time password to verify </param>
        /// <returns> true if the operation was successful </returns>
        public async Task<bool> VerifyOneTimePassword(string email, string otp)
        {
            try
            {
                var result = await ApiClient.Auth
                    .VerifyOtp(new VerifyOtpRequest
                    {
                        Email = email,
                        Otp = otp
                    });

                if (!result.IsSuccess)
                {
                    OnAuthenticationFailed(result.Message);
                    return false;
                }

                Session.SaveSession(accessToken: result.Data.Access,
                    refreshToken: result.Data.Refresh,
                    emailAddress: email);

                OnAuthenticated();

                return true;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                OnAuthenticationFailed(exception.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the currently logged-in user
        /// If the user is not authenticated, an error will be raised
        /// </summary>
        /// <returns> The user response object </returns>
        public async Task<ApiResult<GetUserResponse>> GetUser()
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetUserResponse>.Failed("Not authenticated");

                var result = await ApiClient.User
                    .GetUser();

                if (!result.IsSuccess) return result;

                Session.Wallets = result.Data.Wallets.ToHashSet();
                Session.SaveSessionEmail(result.Data.Email);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetUserResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Gets the user's tokens from the Monaverse API for a specific chain and wallet address
        /// The wallet address supplied must be owned by the user
        /// You can link your wallet address to your account in https://monaverse.com/
        /// </summary>
        /// <param name="chainId"> The id of the chain to get the tokens for</param>
        /// <param name="address"> The wallet address to get the tokens for</param>
        /// <returns> The user's tokens for the specified chain and wallet address </returns>
        public async Task<ApiResult<GetUserTokensResponse>> GetUserTokens(int chainId, string address, string continuation = null)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetUserTokensResponse>.Failed("Not authenticated");

                var result = await ApiClient.User
                    .GetUserTokens(chainId: chainId,
                        address: address,
                        continuation: continuation);

                //TODO: Do some caching here

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetUserTokensResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Gets the animation file for the specified token
        /// </summary>
        /// <param name="token"> The token to get the animation for. You can get this from the GetUserTokensResponse </param>
        /// <returns> The animation file information </returns>
        public async Task<ApiResult<GetTokenAnimationResponse>> GetTokenAnimation(TokenDto token)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetTokenAnimationResponse>.Failed("Not authenticated");

                if (token == null)
                    return ApiResult<GetTokenAnimationResponse>.Failed("Token cannot be null");

                var result = await ApiClient.Token
                    .GetTokenAnimation(chainId: token.ChainId,
                        contract: token.Contract,
                        tokenId: token.TokenId);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetTokenAnimationResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Gets the community tokens available for the specified chain
        /// </summary>
        /// <param name="chainId"> The id of the chain to get the tokens for</param>
        /// <param name="continuation"> The continuation token to get the next set of tokens</param>
        /// <returns></returns>
        public async Task<ApiResult<GetCommunityTokensResponse>> GetCommunityTokens(int chainId, string continuation = null)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetCommunityTokensResponse>.Failed("Not authenticated");

                var result = await ApiClient.Token
                    .GetCommunityTokens(chainId: chainId,
                        continuation: continuation);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetCommunityTokensResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Posts a score to the application leaderboard
        /// </summary>
        /// <param name="score">The score to post</param>
        /// <param name="topic">The topic to post the score to (optional). i.e. "Level 1", "Halloween Special", etc.
        /// Scores will be grouped by topic if provided. If not provided, the score will be posted with no topic </param>
        /// <param name="sdkSecret"> (Optional) The SDK secret to use. Alternatively, use SetSecret()</param>
        /// <returns> The result of the request </returns>
        public async Task<ApiResult> PostScore(float score, string topic = null, string sdkSecret = null)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetTokenAnimationResponse>.Failed("Not authenticated");

                if (!string.IsNullOrEmpty(sdkSecret))
                    SetSecret(sdkSecret);

                if (string.IsNullOrEmpty(_secret))
                    return ApiResult<GetTokenAnimationResponse>.Failed("SDK Secret not set. Use SetSecret() to set it.");

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var formattedScore = score.ToString("F3", CultureInfo.InvariantCulture); // Fixed to 3 decimal places
                var message = $"{formattedScore}:{timestamp}:{topic}";
                var signature = message.GenerateHmac(_secret);

                var result = await ApiClient.Leaderboard
                    .PostScore(new PostScoreRequest
                    {
                        Score = score,
                        Topic = topic,
                        Timestamp = timestamp,
                        Signature = signature
                    });

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Gets the top scores from the leaderboard
        /// </summary>
        /// <param name="limit"> The maximum number of scores to return per page </param>
        /// <param name="offset"> The offset of the first score to return </param>
        /// <param name="featured"> If true, only returns featured topic scores and topic parameter is ignored </param>
        /// <param name="sortingOrder"> The sorting order to use i.e. "highest", "lowest" </param>
        /// <param name="topic"> The topic to get scores for i.e. "Level 1", "Halloween Special", etc.
        /// If featured is true, this parameter is ignored </param>
        /// <param name="period"> The period to get scores for i.e. "all_time", "daily", "weekly", "monthly" </param>
        /// <param name="startTime"> Custom start time for scores. If provided, end time must also be provided </param>
        /// <param name="endTime"> Custom end time for scores. If provided, start time must also be provided </param>
        /// <param name="includeAllUserScores"> If true, includes all user scores for rank calculation </param>
        /// <returns></returns>
        public async Task<ApiResult<GetTopScoresResponse>> GetTopScores(
            int limit = 20,
            int offset = 0,
            bool featured = false,
            string topic = null,
            LeaderboardSortingOrder sortingOrder = LeaderboardSortingOrder.Highest,
            LeaderboardPeriod period = LeaderboardPeriod.AllTime,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool includeAllUserScores = false)
        {
            try
            {
                if(featured && !string.IsNullOrEmpty(topic))
                    MonaDebug.LogWarning($"Topic {topic} parameter is ignored when featured is true");
                
                if(startTime != null && endTime == null)
                    return ApiResult<GetTopScoresResponse>.Failed("End time must be provided if start time is provided");
                
                if(endTime != null && startTime == null)
                    return ApiResult<GetTopScoresResponse>.Failed("Start time must be provided if end time is provided");
                
                //Check that start and end times are valid
                if(startTime > endTime)
                    return ApiResult<GetTopScoresResponse>.Failed("Start time cannot be after end time");
            
                var response = await ApiClient.Leaderboard
                    .GetTopScores(
                        limit: limit,
                        offset: offset,
                        featured: featured,
                        topic: topic,
                        sortingOrder: sortingOrder,
                        period: period,
                        startTime: startTime,
                        endTime: endTime,
                        includeAllUserScores: includeAllUserScores);
                
                return response;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetTopScoresResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Gets the user's rank in the leaderboard
        /// </summary>
        /// <param name="featured"> If true, only returns ranking for featured leaderboard and topic parameter is ignored </param>
        /// <param name="sortingOrder"> The sorting order to use i.e. "highest", "lowest" </param>
        /// <param name="topic"> The topic to get ranking for i.e. "Level 1", "Halloween Special", etc.
        /// If featured is true, this parameter is ignored </param>
        /// <param name="period"> The period to get ranking for i.e. "all_time", "daily", "weekly", "monthly" </param>
        /// <param name="startTime"> Custom start time for ranking. If provided, end time must also be provided </param>
        /// <param name="endTime"> Custom end time for ranking. If provided, start time must also be provided </param>
        /// <param name="includeAllUserScores"> If true, includes all user scores for rank calculation </param>
        /// <returns></returns>
        public async Task<ApiResult<GetUserRankResponse>> GetUserRank(
            bool featured = false,
            string topic = null,
            LeaderboardSortingOrder sortingOrder = LeaderboardSortingOrder.Highest,
            LeaderboardPeriod period = LeaderboardPeriod.AllTime,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool includeAllUserScores = false)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetUserRankResponse>.Failed("Not authenticated");

                if(featured && !string.IsNullOrEmpty(topic))
                    MonaDebug.LogWarning($"Topic {topic} parameter is ignored when featured is true");
                
                if(startTime != null && endTime == null)
                    return ApiResult<GetUserRankResponse>.Failed("End time must be provided if start time is provided");
                
                if(endTime != null && startTime == null)
                    return ApiResult<GetUserRankResponse>.Failed("Start time must be provided if end time is provided");
                
                //Check that start and end times are valid
                if(startTime > endTime)
                    return ApiResult<GetUserRankResponse>.Failed("Start time cannot be after end time");

                var response = await ApiClient.Leaderboard
                    .GetUserRank(
                        featured: featured,
                        topic: topic,
                        sortingOrder: sortingOrder,
                        period: period,
                        startTime: startTime,
                        endTime: endTime,
                        includeAllUserScores: includeAllUserScores);
                
                return response;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetUserRankResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Get scores around the user's rank (Above and below the user) 
        /// </summary>
        /// <param name="featured"> If true, only returns scores for featured leaderboard and topic parameter is ignored </param>
        /// <param name="sortingOrder"> The sorting order to use i.e. "highest", "lowest" </param>
        /// <param name="topic"> The topic to get scores for i.e. "Level 1", "Halloween Special", etc.
        /// If featured is true, this parameter is ignored </param>
        /// <param name="period"> The period to get scores for i.e. "all_time", "daily", "weekly", "monthly" </param>
        /// <param name="startTime"> Custom start time for scores. If provided, end time must also be provided </param>
        /// <param name="endTime"> Custom end time for scores. If provided, start time must also be provided </param>
        /// <param name="includeAllUserScores"> If true, includes all user scores for rank calculation </param>
        /// <param name="limit"> The number of rankings to return. If there are ties, there may be more than limit results </param>
        /// <returns></returns>
        public async Task<ApiResult<List<TopScore>>> GetAroundMeScores(
            bool featured = false,
            string topic = null,
            LeaderboardSortingOrder sortingOrder = LeaderboardSortingOrder.Highest,
            LeaderboardPeriod period = LeaderboardPeriod.AllTime,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool includeAllUserScores = false,
            int limit = 10)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<List<TopScore>>.Failed("Not authenticated");

                if(featured && !string.IsNullOrEmpty(topic))
                    MonaDebug.LogWarning($"Topic {topic} parameter is ignored when featured is true");
                
                if(startTime != null && endTime == null)
                    return ApiResult<List<TopScore>>.Failed("End time must be provided if start time is provided");
                
                if(endTime != null && startTime == null)
                    return ApiResult<List<TopScore>>.Failed("Start time must be provided if end time is provided");
                
                //Check that start and end times are valid
                if(startTime > endTime)
                    return ApiResult<List<TopScore>>.Failed("Start time cannot be after end time");

                var response = await ApiClient.Leaderboard
                    .GetAroundMeScores(
                        featured: featured,
                        topic: topic,
                        sortingOrder: sortingOrder,
                        period: period,
                        startTime: startTime,
                        endTime: endTime,
                        includeAllUserScores: includeAllUserScores,
                        limit: limit);
                
                return response;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<List<TopScore>>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Gets the status and details of a specific generation request
        /// </summary>
        /// <param name="requestId">The unique identifier of the generation request to retrieve</param>
        /// <returns>
        /// A result containing the generation request details including:
        /// - Input and output assets
        /// - Generation status
        /// - Creation and completion timestamps
        /// - Any error messages if the generation failed
        /// Returns an error if not authenticated or if the request fails
        /// </returns>
        public async Task<ApiResult<GetGenerationRequestResponse>> GetGenerationRequest(string requestId)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetGenerationRequestResponse>.Failed("Not authenticated");

                var result = await ApiClient.Ai.GetGenerationRequest(requestId);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetGenerationRequestResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific asset by its ID
        /// </summary>
        /// <param name="assetId">The unique identifier of the asset to retrieve</param>
        /// <returns>
        /// A result containing the asset details including:
        /// - Creator information
        /// - Source generation details
        /// - Asset metadata (type, URL, creation time)
        /// Returns an error if not authenticated or if the request fails
        /// </returns>
        public async Task<ApiResult<GetAssetResponse>> GetAsset(string assetId)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetAssetResponse>.Failed("Not authenticated");

                var result = await ApiClient.Ai.GetAsset(assetId);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetAssetResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of generation requests made by the authenticated user with optional filtering
        /// </summary>
        /// <param name="status">Filter requests by their status (e.g., Pending, Completed, Failed)</param>
        /// <param name="stepType">Filter requests by the type of generation step</param>
        /// <param name="desiredOutputType">Filter requests by the desired output asset type</param>
        /// <param name="limit">Maximum number of requests to return (default: 100)</param>
        /// <param name="offset">Number of requests to skip for pagination (default: 0)</param>
        /// <returns>
        /// A paginated result containing generation requests matching the specified filters
        /// Returns an error if not authenticated or if the request fails
        /// </returns>
        public async Task<ApiResult<GetGenerationRequestsResponse>> GetGenerationRequests(
            StatusFilter? status = null,
            StepTypeFilter? stepType = null,
            AssetTypeFilter? desiredOutputType = null,
            int limit = 100,
            int offset = 0
        )
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetGenerationRequestsResponse>.Failed("Not authenticated");

                var result = await ApiClient.Ai.GetGenerationRequests(status, stepType, desiredOutputType, limit, offset);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetGenerationRequestsResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of assets owned by the authenticated user with optional filtering
        /// </summary>
        /// <param name="assetType">Filter assets by their type</param>
        /// <param name="limit">Maximum number of assets to return (default: 100)</param>
        /// <param name="offset">Number of assets to skip for pagination (default: 0)</param>
        /// <returns>
        /// A paginated result containing assets matching the specified filters
        /// Returns an error if not authenticated or if the request fails
        /// </returns>
        public async Task<ApiResult<GetAssetsResponse>> GetAssets(
            AssetTypeFilter? assetType = null,
            int limit = 100,
            int offset = 0    
        )
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetAssetsResponse>.Failed("Not authenticated");

                var result = await ApiClient.Ai.GetAssets(assetType, limit, offset);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetAssetsResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Creates a new text-to-image generation request
        /// </summary>
        /// <param name="prompt">The text description of the image to generate</param>
        /// <returns>
        /// A result containing the created generation request details
        /// The generation will be processed asynchronously and can be monitored using GetGenerationRequest
        /// Returns an error if not authenticated or if the request fails
        /// </returns>
        public async Task<ApiResult<CreateTextToImageRequestResponse>> CreateTextToImageRequest(string prompt)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<CreateTextToImageRequestResponse>.Failed("Not authenticated");

                var result = await ApiClient.Ai.CreateTextToImageRequest(new CreateTextToImageRequestRequest
                {
                    Prompt = prompt
                });

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<CreateTextToImageRequestResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Creates a new image-to-3D generation request
        /// </summary>
        /// <param name="imageId">The ID of the source image to convert to 3D</param>
        /// <returns>
        /// A result containing the created generation request details
        /// The generation will be processed asynchronously and can be monitored using GetGenerationRequest
        /// Returns an error if not authenticated or if the request fails
        /// </returns>
        public async Task<ApiResult<CreateImageTo3dRequestResponse>> CreateImageTo3dRequest(string imageId)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<CreateImageTo3dRequestResponse>.Failed("Not authenticated");

                var result = await ApiClient.Ai.CreateImageTo3dRequest(new CreateImageTo3dRequestRequest
                {
                    ImageId = imageId
                });

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<CreateImageTo3dRequestResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Retrieves the current user's quota information for various generation types
        /// </summary>
        /// <returns>
        /// A result containing quota information for each generation type including:
        /// - Generation type and period
        /// - Usage limits
        /// - Current usage and remaining quota
        /// - Custom quota status
        /// Returns an error if not authenticated or if the request fails
        /// </returns>
        public async Task<ApiResult<GetQuotaResponse>> GetQuota()
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetQuotaResponse>.Failed("Not authenticated");

                var result = await ApiClient.Ai.GetQuota();

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetQuotaResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Sets the SDK secret for the Monaverse API.
        /// You can find your SDK Secret in the Monaverse Studio Dashboard
        /// For security reasons, do not serialize this secret in your code (Component serialized fields) 
        /// </summary>
        /// <param name="sdkSecret"></param>
        public void SetSecret(string sdkSecret)
            => _secret = sdkSecret;

        /// <summary>
        /// Logs the user out of the Monaverse API
        /// It clears any stored credentials
        /// </summary>
        public void Logout()
        {
            //Clear session
            ApiClient.Session.ClearSession();
        }

        /// <summary>
        /// Returns true if the there is an active session with the Monaverse API.
        /// This must be true before you can call any authenticated API endpoints
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            return Session.IsAuthenticated;
        }

        #region Helpers

        /// <summary>
        /// Returns a list of supported chain ids by the Monaverse API
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetSupportedChainIds()
        {
            return ChainHelper.SupportedChains();
        }

        /// <summary>
        /// Returns the name of the chain with the specified chain id
        /// </summary>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public string GetChainName(int chainId)
        {
            return ChainHelper.GetChainName(chainId);
        }

        #endregion

        #region Events Handlers

        private void OnAuthenticated()
        {
            UnitySyncContext.Post(_ => { Authenticated?.Invoke(this, EventArgs.Empty); }, null);
        }

        private void OnLoggedOut()
        {
            UnitySyncContext.Post(_ => { LoggedOut?.Invoke(this, EventArgs.Empty); }, null);
        }

        private void OnAuthenticationFailed(string errorMessage)
        {
            UnitySyncContext.Post(_ => { AuthenticationFailed?.Invoke(this, errorMessage); }, null);
        }

        #endregion
    }
}