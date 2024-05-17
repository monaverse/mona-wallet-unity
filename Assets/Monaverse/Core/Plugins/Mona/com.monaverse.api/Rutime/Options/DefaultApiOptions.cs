using Monaverse.Api.Configuration;
using Monaverse.Api.Logging;

namespace Monaverse.Api.Options
{
    public class DefaultApiOptions : IMonaApiOptions
    {
        public ApiLogLevel LogLevel { get; set; } = ApiLogLevel.Info;
        public ApiEnvironment Environment { get; set; } = ApiEnvironment.Production;
        public string ApplicationId { get; set; }
    }
}