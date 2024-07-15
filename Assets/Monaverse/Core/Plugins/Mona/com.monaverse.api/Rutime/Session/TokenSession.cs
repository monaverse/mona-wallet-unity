using System;

namespace Monaverse.Api.Session
{
    internal class TokenSession : IMonaApiSession
    {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string LegacyAccessToken { get; private set; }
        public Action OnClearSession { get; set; }
        
        private const int RefreshTokenTtl = 2592000;

        public TokenSession()
        {
            var legacyAccessToken = PlayerPrefsObfuscator.Load(Constants.LegacyAccessTokenStorageKey);
            if (!string.IsNullOrEmpty(legacyAccessToken))
                LegacyAccessToken = legacyAccessToken;

            var accessToken = PlayerPrefsObfuscator.Load(Constants.AccessKey);
            if (!string.IsNullOrEmpty(accessToken))
                AccessToken = accessToken;

            var refreshToken = PlayerPrefsObfuscator.Load(Constants.RefreshKey);
            if (!string.IsNullOrEmpty(refreshToken))
                RefreshToken = refreshToken;
        }

        public void SaveLegacySession(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return;

            LegacyAccessToken = accessToken;
            PlayerPrefsObfuscator.Save(Constants.LegacyAccessTokenStorageKey, accessToken);
        }

        public void SaveSession(string accessToken, string refreshToken)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                return;

            AccessToken = accessToken;
            RefreshToken = refreshToken;
            
            PlayerPrefsObfuscator.Save(Constants.AccessKey, accessToken);
            PlayerPrefsObfuscator.Save(Constants.RefreshKey, refreshToken, RefreshTokenTtl);
        }
        
        public void ClearSession()
        {
            PlayerPrefsObfuscator.Delete(Constants.LegacyAccessTokenStorageKey);
            PlayerPrefsObfuscator.Delete(Constants.AccessKey);
            PlayerPrefsObfuscator.Delete(Constants.RefreshKey);

            AccessToken = null;
            RefreshToken = null;
            LegacyAccessToken = null;
            
            OnClearSession?.Invoke();
        }

    }
}