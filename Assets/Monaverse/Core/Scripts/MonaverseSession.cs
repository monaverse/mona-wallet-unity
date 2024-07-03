using System.Collections.Generic;
using Monaverse.Core.Utils;
using UnityEngine;

namespace Monaverse.Core
{
    public class MonaverseSession
    {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string EmailAddress { get; private set; }
        public string WalletAddress { get; private set; }
        
        public int DefaultChainId { get; set; }
        
        public HashSet<string> Wallets { get; set; }
        
        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

        internal MonaverseSession(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        internal void Load()
        {
            EmailAddress = PlayerPrefs.GetString(MonaConstants.Session.SessionEmailKey);
            WalletAddress = PlayerPrefs.GetString(MonaConstants.Session.SessionWalletAddressKey);
            DefaultChainId = PlayerPrefs.GetInt(MonaConstants.Session.SessionWalletAddressKey, 1);
        }

        internal void SaveSession(string accessToken, string refreshToken, string emailAddress)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            EmailAddress = SaveSessionEmail(emailAddress);
        }
        
        internal string SaveSessionEmail(string emailAddress)
            => EmailAddress = emailAddress.UpdatePlayerPrefs(MonaConstants.Session.SessionEmailKey);
        
        public string SaveWalletAddress(string walletAddress)
            => WalletAddress = walletAddress.UpdatePlayerPrefs(MonaConstants.Session.SessionWalletAddressKey);

        public int SaveDefaultChainId(int chainId)
            => DefaultChainId = chainId.UpdatePlayerPrefs(MonaConstants.Session.SessionWalletAddressKey);
        
        internal void Clear()
        {
            WalletAddress = SaveWalletAddress(null);
            EmailAddress = SaveSessionEmail(null);
            DefaultChainId = SaveDefaultChainId(1);
            AccessToken = null;
            RefreshToken = null;
        }
    }
}