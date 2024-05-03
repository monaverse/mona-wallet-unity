namespace Monaverse.Api.Configuration
{
    public sealed class StagingConfiguration : IApiConfiguration
    {
        public ApiEnvironment Environment => ApiEnvironment.Staging;
        public string Host => Constants.BaseUrlStaging;
    }
}