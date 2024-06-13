using System;
using System.Threading;
using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Auth;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Collectibles;
using Monaverse.Api.Modules.User;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;
using Monaverse.Api.Options;

namespace Monaverse.Api
{
    internal sealed class MonaApiClientImpl : IMonaApiClient
    {
        private class TokenModel
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string LegacyAccessToken { get; set; }
        }

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private readonly IMonaApiOptions _monaApiOptions;
        private readonly IMonaApiLogger _monaApiLogger;
        private readonly IMonaHttpClient _monaHttpClient;
        private readonly TokenModel _tokenModel = new();

        public IAuthApiModule Auth { get; private set; }
        public ICollectiblesApiModule Collectibles { get; private set; }
        public IUserApiModule User { get; private set; }

        public MonaApiClientImpl(IMonaApiOptions monaApiOptions,
            IMonaApiLogger monaApiLogger,
            IMonaHttpClient monaHttpClient)
        {
            _monaApiOptions = monaApiOptions;
            _monaApiLogger = monaApiLogger;
            _monaHttpClient = monaHttpClient;

            //Load legacy session
            var legacyAccessToken = PlayerPrefsObfuscator.Load(Constants.LegacyAccessTokenStorageKey);
            if (!string.IsNullOrEmpty(legacyAccessToken))
                _tokenModel.LegacyAccessToken = legacyAccessToken;

            //Load access and refresh tokens
            var accessToken = PlayerPrefsObfuscator.Load(Constants.AccessKey);
            if (!string.IsNullOrEmpty(accessToken))
                _tokenModel.AccessToken = accessToken;

            var refreshToken = PlayerPrefsObfuscator.Load(Constants.RefreshKey);
            if (!string.IsNullOrEmpty(refreshToken))
                _tokenModel.RefreshToken = refreshToken;

            //Configure API modules
            Auth = new AuthApiModule(this, monaApiLogger);
            Collectibles = new CollectiblesApiModule(this);
            User = new UserApiModule(this);
        }

        public bool IsAuthorized()
        {
            if (_monaHttpClient == null)
                return false;

            return !string.IsNullOrEmpty(_tokenModel.LegacyAccessToken)
                   || !string.IsNullOrEmpty(_tokenModel.AccessToken);
        }

        public void ClearSession()
        {
            PlayerPrefsObfuscator.Delete(Constants.LegacyAccessTokenStorageKey);
            PlayerPrefsObfuscator.Delete(Constants.AccessKey);
            PlayerPrefsObfuscator.Delete(Constants.RefreshKey);

            _tokenModel.AccessToken = null;
            _tokenModel.RefreshToken = null;
            _tokenModel.LegacyAccessToken = null;
        }

        public string GetLegacyAccessToken()
            => _tokenModel.LegacyAccessToken;

        public void SaveLegacySession(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                _monaApiLogger.LogError("Cannot save session because accessToken is null or empty");
                return;
            }

            _tokenModel.LegacyAccessToken = accessToken;
            PlayerPrefsObfuscator.Save(Constants.LegacyAccessTokenStorageKey, accessToken);
        }

        public void SaveSession(string accessToken, string refreshToken)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                _monaApiLogger.LogError("Cannot save session because accessToken or refreshToken is null or empty");
                return;
            }

            _tokenModel.AccessToken = accessToken;
            _tokenModel.RefreshToken = refreshToken;
            PlayerPrefsObfuscator.Save(Constants.AccessKey, accessToken);
            PlayerPrefsObfuscator.Save(Constants.RefreshKey, refreshToken);
        }

        public string GetUrlWithPath(string path)
            => _monaApiOptions.GetUrlWithPath(path);

        public string GetUrlWithPathLegacy(string path)
            => _monaApiOptions.GetUrlWithPathLegacy(path);

        public async Task<IMonaHttpResponse> Send(IMonaHttpRequest request)
        {
            // Add the application id to every request
            request.WithHeader(Constants.ApplicationIdHeader, _monaApiOptions.ApplicationId);
            return await _monaHttpClient.SendAsync(request);
        }

        public async Task<IMonaHttpResponse> SendAuthenticated(IMonaHttpRequest request)
        {
            // Add the access token if there is one
            if (string.IsNullOrEmpty(_tokenModel.AccessToken))
                throw new Exception("No access token found");

            request.WithBearerToken(_tokenModel.AccessToken);

            var response = await Send(request);

            if (response.ResponseCode != 401) return response;

            //handle refresh token if unauthorized in a thread-safe way
            await _semaphoreSlim.WaitAsync();
            try
            {

                // Check again to avoid race condition
                if (string.IsNullOrEmpty(_tokenModel.AccessToken)
                    || string.IsNullOrEmpty(_tokenModel.RefreshToken))
                    throw new InvalidOperationException("Session expired");

                var refreshTokenRequest = new RefreshTokenRequest { Refresh = _tokenModel.RefreshToken };
                var refreshResponse = await Auth.RefreshToken(refreshTokenRequest);
                if (!refreshResponse.IsSuccess)
                {
                    throw new InvalidOperationException("Failed to refresh token");
                }
                
                request.WithBearerToken(_tokenModel.AccessToken);
                response = await Send(request);
            }
            finally
            { 
                _semaphoreSlim.Release();
            }

            return response;
        }

        public async Task<IMonaHttpResponse> SendLegacy(IMonaHttpRequest request)
        {
            // Add the application id to every request
            request.WithHeader(Constants.ApplicationIdHeader, _monaApiOptions.ApplicationId);

            // Add the access token if there is one
            if (!string.IsNullOrEmpty(_tokenModel.LegacyAccessToken))
                request.WithBearerToken(_tokenModel.LegacyAccessToken);

            return await _monaHttpClient.SendAsync(request);
        }
    }
}