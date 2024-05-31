namespace Monaverse.Core
{
    public static class MonaConstants
    {
        public class Media
        {
            public const string MonaCloudinaryBaseURL = "https://res.cloudinary.com/mona-gallery/image/fetch";
            public const string MonaIpfsGateway = "https://ipfs.mona.gallery/ipfs";
        }
        
        public class Session
        {
            public const string SessionVersion = "v1";
            public const string SessionWalletAddressKey = SessionVersion + "mona-session-wallet-address" ;
        }

        public class MonaversePages
        {
            public const string Monaverse = "https://monaverse.com";
            public const string ArtifactDetailsBaseUrl = "https://monaverse.com/artifacts"; 
        }
    }
}