using Monaverse.Api.Configuration;
using Monaverse.Api.Logging;

namespace Monaverse.Api.Options
{
    public class DefaultApiOptions : IMonaApiOptions
    {
        public ApiLogLevel LogLevel { get; set; } = ApiLogLevel.Info;
        public ApiEnvironment Environment { get; set; } = ApiEnvironment.Staging;
        public string ApiKey { get; set; }
        public string ApplicationId { get; set; }
    }
}