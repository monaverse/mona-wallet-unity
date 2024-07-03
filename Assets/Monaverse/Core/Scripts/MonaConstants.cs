namespace Monaverse.Core
{
    public static class MonaConstants
    {
        public class Media
        {
            public const string MonaCloudinaryBaseURL = "https://res.cloudinary.com/mona-gallery/image/fetch";
            public const string MonaIpfsGateway = "https://ipfs.mona.gallery/ipfs";
            public const string MonaRemoteLogo = "https://res.cloudinary.com/mona-gallery/image/upload/q_auto,f_auto,w_400/zodiac/logos/logo-white-svg";
        }
        
        public class Session
        {
            public const string SessionVersion = "v2";
            public const string SessionWalletAddressKey = SessionVersion + "mona-session-wallet-address" ;
            public const string SessionEmailKey = SessionVersion + "mona-session-email-address" ;
            public const string SessionChainIdKey = SessionVersion + "mona-session-chain-id" ;
        }

        public class MonaversePages
        {
            public const string Marketplace = "https://marketplace.monaverse.com";
        }
    }
}