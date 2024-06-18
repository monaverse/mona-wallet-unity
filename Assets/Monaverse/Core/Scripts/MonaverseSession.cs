using Monaverse.Core.Utils;
using UnityEngine;

namespace Monaverse.Core
{
    public class MonaverseSession
    {
        public string AccessToken { get; private set; }
        public string WalletAddress { get; private set; }
        
        public bool IsWalletConnected => !string.IsNullOrEmpty(WalletAddress);
        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

        internal MonaverseSession(string accessToken = null)
        {
            AccessToken = accessToken;
        }

        internal void Load()
        {
            WalletAddress = PlayerPrefs.GetString(MonaConstants.Session.SessionWalletAddressKey);
        }

        internal string SaveAccessToken(string accessToken)
            => AccessToken = accessToken;
        
        internal string SaveWalletAddress(string walletAddress)
            => WalletAddress = walletAddress.UpdatePlayerPrefs(MonaConstants.Session.SessionWalletAddressKey);

        internal void Clear()
        {
            WalletAddress = SaveWalletAddress(null);
            AccessToken = null;
        }
    }
}