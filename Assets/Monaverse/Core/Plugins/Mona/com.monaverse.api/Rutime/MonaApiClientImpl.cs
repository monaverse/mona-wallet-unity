using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Auth;
using Monaverse.Api.Modules.Collectibles;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.Options;

namespace Monaverse.Api
{
    internal sealed class MonaApiClientImpl : IMonaApiClient
    {
        private readonly IMonaApiLogger _monaApiLogger;
        private readonly IMonaHttpClient _monaHttpClient;
        public IAuthApiModule Auth { get; private set; }
        public ICollectiblesApiModule Collectibles { get; private set; }

        public MonaApiClientImpl(IMonaApiOptions monaApiOptions,
            IMonaApiLogger monaApiLogger,
            IMonaHttpClient monaHttpClient)
        {
            _monaApiLogger = monaApiLogger;
            _monaHttpClient = monaHttpClient;

            //Configure API modules
            Auth = new AuthApiModule(monaApiOptions, monaApiLogger, _monaHttpClient);
            Collectibles = new CollectiblesApiModule(monaApiOptions, monaApiLogger, _monaHttpClient);
        }

        public bool IsAuthorized()
        {
            if(_monaHttpClient == null)
                return false;
            
            return !string.IsNullOrEmpty(_monaHttpClient.AccessToken);
        }

        public void ClearSession()
            => _monaHttpClient.ClearSession();
    }
}