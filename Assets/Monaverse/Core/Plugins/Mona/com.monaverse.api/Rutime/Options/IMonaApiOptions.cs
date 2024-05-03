using Monaverse.Api.Configuration;
using Monaverse.Api.Logging;

namespace Monaverse.Api.Options
{
    public interface IMonaApiOptions
    {
        ApiLogLevel LogLevel { get; }
        ApiEnvironment Environment { get; }
        string ApplicationId { get; }
    }
}