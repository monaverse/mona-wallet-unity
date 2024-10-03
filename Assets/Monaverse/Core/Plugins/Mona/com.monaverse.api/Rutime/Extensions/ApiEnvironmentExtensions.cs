using System;
using Monaverse.Api.Configuration;

namespace Monaverse.Api.Extensions
{
    public static class ApiEnvironmentExtensions
    {
        public static string ResolveHost(this ApiEnvironment environment)
            => environment switch
            {
                ApiEnvironment.Production => Constants.ApiDomainProduction,
                ApiEnvironment.Staging => Constants.ApiDomainStaging,
                ApiEnvironment.Development => Constants.ApiDomainLocal,
                ApiEnvironment.Local => Constants.ApiDomainLocal,
                _ => throw new InvalidOperationException($"invalid api environment {environment}")
            };
    }
}