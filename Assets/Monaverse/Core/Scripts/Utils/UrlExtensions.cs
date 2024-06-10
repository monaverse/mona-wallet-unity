namespace Monaverse.Core.Utils
{
    public static class UrlExtensions
    {
        public static string ToIpfsGatewayUrl(this string cid) 
            => $"{MonaConstants.Media.MonaIpfsGateway}/{cid}";
        
        public static string ToCloudinaryImageUrl(this string imageUrl, int width = 400) 
            => $"{MonaConstants.Media.MonaCloudinaryBaseURL}/q_auto,f_auto,w_{width}/{imageUrl}";
    }
}