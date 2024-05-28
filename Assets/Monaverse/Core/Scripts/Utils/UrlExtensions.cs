namespace Monaverse.Core.Utils
{
    public static class UrlExtensions
    {
        public static string ResolveTokenUrl(this string tokenImage, int width = 400)
        {
            if (string.IsNullOrEmpty(tokenImage))
            {
                MonaDebug.LogError("Token image is null or empty"); 
                return null;
            }
            
            //if token does not have a schema we assume is an Ipfs CID 
            if (!tokenImage.Contains("://"))
                tokenImage = tokenImage.ToIpfsGatewayUrl();
            
            return tokenImage.ToCloudinaryImageUrl(width);
        }
        
        private static string ToIpfsGatewayUrl(this string cid) 
            => $"{MonaConstants.Media.MonaIpfsGateway}/{cid}";
        
        public static string ToCloudinaryImageUrl(this string imageUrl, int width = 400) 
            => $"{MonaConstants.Media.MonaCloudinaryBaseURL}/q_auto,f_auto,w_{width}/{imageUrl}";
    }
}