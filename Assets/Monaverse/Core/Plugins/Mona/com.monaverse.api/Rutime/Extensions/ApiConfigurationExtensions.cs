using System;
using Monaverse.Api.Configuration;

namespace Monaverse.Api.Extensions
{
    public static class ApiConfigurationExtensions
    {
        public static string GetUrlWithPath(this IApiConfiguration apiConfiguration, string path)
            => new Uri(new Uri(apiConfiguration.Host), path).AbsoluteUri;
    }
}