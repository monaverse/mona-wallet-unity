using System;

namespace Monaverse.Api.Session
{
    public interface IMonaApiSession
    {
        void SaveLegacySession(string accessToken);
        void SaveSession(string accessToken, string refreshToken);
        void ClearSession();
        string LegacyAccessToken { get; }
        string AccessToken { get; }
        string RefreshToken { get; }
        Action OnClearSession { get; set; }
    }
}