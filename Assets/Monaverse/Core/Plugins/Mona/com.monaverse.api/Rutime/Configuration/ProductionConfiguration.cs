namespace Monaverse.Api.Configuration
{
    public sealed class ProductionConfiguration : IApiConfiguration
    {
        public ApiEnvironment Environment => ApiEnvironment.Production;
        public string Host => Constants.BaseUrlProduction;
    }
}