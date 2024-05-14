using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Monaverse.Api
{
    internal class PlayerPrefsObfuscator
    {
        private const string TimestampKey = "_timestamp";
        public static void Save(string key, string value, int expiryInSeconds = 86400)
        {
            var obfuscatedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expiryInSeconds;
            PlayerPrefs.SetString(key, obfuscatedValue);
            PlayerPrefs.SetString(key + TimestampKey, timestamp.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
        }

        public static string Load(string key)
        {
            var storedValue = PlayerPrefs.GetString(key, string.Empty);
            var timestampStr = PlayerPrefs.GetString(key + TimestampKey, string.Empty);

            if (string.IsNullOrEmpty(storedValue) || string.IsNullOrEmpty(timestampStr)) return string.Empty;
            
            try
            {
                var storedTimestamp = long.Parse(timestampStr,CultureInfo.InvariantCulture);
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (now > storedTimestamp)
                {
                    PlayerPrefs.DeleteKey(key);
                    PlayerPrefs.DeleteKey(key + TimestampKey);
                    
                    return string.Empty;
                }
                
                var bytes = Convert.FromBase64String(storedValue);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                // Handle the case where the string is not in a valid base64 format
                Debug.LogError("[MonaverseApi] Decoding failed for key: " + key);
                return string.Empty;
            }
        }

        public static void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}