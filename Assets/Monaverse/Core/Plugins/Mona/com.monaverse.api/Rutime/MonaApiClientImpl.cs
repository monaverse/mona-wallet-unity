using System;
using System.Threading;
using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Ai;
using Monaverse.Api.Modules.Auth;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Leaderboard;
using Monaverse.Api.Modules.Token;
using Monaverse.Api.Modules.User;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;
using Monaverse.Api.Options;
using Monaverse.Api.Session;

namespace Monaverse.Api
{
    internal sealed class MonaApiClientImpl : IMonaApiClient
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private readonly IMonaApiOptions _monaApiOptions;
        private readonly IMonaHttpClient _monaHttpClient;

        public IAuthApiModule Auth { get; private set; }
        public IUserApiModule User { get; private set; }
        public ITokenApiModule Token { get; private set; }
        public ILeaderboardApiModule Leaderboard { get; private set; }
        public IAiApiModule Ai { get; private set; }
        
        public IMonaApiSession Session { get; }
        
        public string ApplicationId => _monaApiOptions.ApplicationId;

        public MonaApiClientImpl(IMonaApiOptions monaApiOptions,
            IMonaApiLogger monaApiLogger,
            IMonaHttpClient monaHttpClient,
            IMonaApiSession monaApiSession)
        {
            _monaApiOptions = monaApiOptions;
            _monaHttpClient = monaHttpClient;
            Session = monaApiSession;

            //Configure API modules
            Auth = new AuthApiModule(this, monaApiLogger);
            User = new UserApiModule(this);
            Token = new TokenApiModule(this);
            Leaderboard = new LeaderboardApiModule(this);
            Ai = new AiApiModule(this);
        }

        public bool IsAuthorized()
        {
            if (_monaHttpClient == null)
                return false;

            return !string.IsNullOrEmpty(Session.LegacyAccessToken)
                   || !string.IsNullOrEmpty(Session.AccessToken);
        }

        public void ClearSession()
        {
           Session.ClearSession();
        }

        public string GetLegacyAccessToken()
            => Session.LegacyAccessToken;

        public void SaveLegacySession(string accessToken)
        {
            Session.SaveLegacySession(accessToken);
        }

        public void SaveSession(string accessToken, string refreshToken)
        {
            Session.SaveSession(accessToken, refreshToken);
        }

        public string GetUrlWithPath(string path)
            => _monaApiOptions.GetUrlWithPath(path);

        public async Task<IMonaHttpResponse> Send(IMonaHttpRequest request)
        {
            // Add the application id to every request
            request.WithHeader(Constants.ApplicationIdHeader, _monaApiOptions.ApplicationId);
            return await _monaHttpClient.SendAsync(request);
        }

        public async Task<IMonaHttpResponse> SendAuthenticated(IMonaHttpRequest request)
        {
            // Add the access token if there is one
            if (string.IsNullOrEmpty(Session.AccessToken))
                throw new Exception("No access token found");

            request.WithBearerToken(Session.AccessToken);

            var response = await Send(request);

            if (response.ResponseCode != 401) return response;

            //handle refresh token if unauthorized in a thread-safe way
            await _semaphoreSlim.WaitAsync();
            try
            {

                // Check again to avoid race condition
                if (string.IsNullOrEmpty(Session.AccessToken)
                    || string.IsNullOrEmpty(Session.RefreshToken))
                    throw new InvalidOperationException("Session expired");

                var refreshTokenRequest = new RefreshTokenRequest { Refresh = Session.RefreshToken };
                var refreshResponse = await Auth.RefreshToken(refreshTokenRequest);
                if (!refreshResponse.IsSuccess)
                {
                    throw new InvalidOperationException("Failed to refresh token");
                }
                
                request.WithBearerToken(Session.AccessToken);
                response = await Send(request);
            }
            finally
            { 
                _semaphoreSlim.Release();
            }

            return response;
        }
    }
}