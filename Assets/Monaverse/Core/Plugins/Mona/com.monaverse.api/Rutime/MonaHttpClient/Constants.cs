namespace Monaverse.Api.MonaHttpClient
{
    internal static class Constants
    {
        public const string ContentTypeHeader = "Content-Type" ;
        public const string DefaultCharSet = "charset=utf-8" ;
        public const string DefaultContentType = "application/octet-stream" ;
        public const string JsonContentType = "application/json" ;
        public const string AnyImageContentType = "image/*" ;
        
        public const string AcceptHeader = "Accept" ;
        public const string DefaultAcceptType = JsonContentType;

        public const string AuthorizationHeader = "Authorization" ;
        public const string BearerToken = "Bearer" ;
    }
}