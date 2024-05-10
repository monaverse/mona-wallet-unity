using System;
using Monaverse.Api.Configuration;

namespace Monaverse.Api.Extensions
{
    public static class ApiEnvironmentExtensions
    {
        public static string ResolveHost(this ApiEnvironment environment)
            => environment switch
            {
                ApiEnvironment.Production => Constants.BaseUrlProduction,
                ApiEnvironment.Development => Constants.BaseUrlDevelopment,
                ApiEnvironment.Staging => Constants.BaseUrlStaging,
                ApiEnvironment.Local => Constants.BaseUrlLocal,
                _ => throw new InvalidOperationException($"invalid api environment {environment}")
            };
    }
}