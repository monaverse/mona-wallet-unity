using System;

namespace Monaverse.Core.Scripts.Utils
{
    public static class ValidationExtensions
    {
        public static bool IsEmailValid(this string email)
        {
            return !string.IsNullOrEmpty(email) 
                   && email.IndexOf("@", StringComparison.InvariantCulture) > 0;
        }
        
        public static bool IsOtpValid(this string otp)
        {
            return !string.IsNullOrEmpty(otp) 
                   && otp.Length == 6
                   && int.TryParse(otp, out _);
        }
    }
}