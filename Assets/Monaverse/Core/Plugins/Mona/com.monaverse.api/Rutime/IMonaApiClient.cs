using Monaverse.Api.Modules.Auth;
using Monaverse.Api.Modules.Collectibles;

namespace Monaverse.Api
{
    public interface IMonaApiClient
    {
        IAuthApiModule Auth { get; }
        ICollectiblesApiModule Collectibles { get; }
        bool IsAuthorized();
        void ClearSession();
    }
}