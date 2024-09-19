using System;
using System.Security.Cryptography;
using System.Text;

namespace Monaverse.Core.Scripts.Utils
{
    public static class SecurityExtensions
    {
        public static string GenerateHmac(this string message, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToBase64String(hash);
        }
    }
}