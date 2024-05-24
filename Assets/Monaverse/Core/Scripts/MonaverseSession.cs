using UnityEngine;

namespace Monaverse.Core
{
    public class MonaverseSession
    {
        public bool IsActive => !string.IsNullOrEmpty(WalletAddress);
        public string WalletAddress { get; set; }
        
        public void Load()
        {
            WalletAddress = PlayerPrefs.GetString(MonaConstants.Session.SessionWalletAddressKey);
        }
        
        public void Save()
        {
            if (!string.IsNullOrEmpty(WalletAddress))
                PlayerPrefs.SetString(MonaConstants.Session.SessionWalletAddressKey, WalletAddress);
            else
                PlayerPrefs.DeleteKey(MonaConstants.Session.SessionWalletAddressKey);
            
            PlayerPrefs.Save();
        }
        
        public void Clear()
        {
            WalletAddress = null;
            Save();
        }
    }
}