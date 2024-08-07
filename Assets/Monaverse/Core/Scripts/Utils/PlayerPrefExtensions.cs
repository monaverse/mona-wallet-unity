using UnityEngine;

namespace Monaverse.Core.Utils
{
    public static class PlayerPrefExtensions
    {
        public static string UpdatePlayerPrefs(this string value, string key)
        {
            if (string.IsNullOrEmpty(value))
                PlayerPrefs.DeleteKey(key);
            else
                PlayerPrefs.SetString(key, value);

            return value;
        }

        public static int UpdatePlayerPrefs(this int value, string key)
        {
            if(value <= 0)
                PlayerPrefs.DeleteKey(key);
            else
                PlayerPrefs.SetInt(key, value);
            
            return value;
        }
    }
}