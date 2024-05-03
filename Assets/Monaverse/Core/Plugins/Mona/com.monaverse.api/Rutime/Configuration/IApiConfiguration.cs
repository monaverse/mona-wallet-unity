namespace Monaverse.Api.Configuration
{
    public interface IApiConfiguration
    {
        ApiEnvironment Environment { get; }
        string Host { get;}
    }
}