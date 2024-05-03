namespace Monaverse.Api.Configuration
{
    public sealed class LocalConfiguration : IApiConfiguration
    {
        public ApiEnvironment Environment => ApiEnvironment.Local;
        public string Host => Constants.BaseUrlLocal;
    }
}