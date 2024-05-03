using System;
using Monaverse.Api.Options;

namespace Monaverse.Api.Extensions
{
    public static class ApiOptionsExtensions
    {
        public static string GetUrlWithPath(this IMonaApiOptions monaApiOptions, string path)
            => new Uri(new Uri(monaApiOptions.Environment.ResolveHost()), path).AbsoluteUri;
    }
}