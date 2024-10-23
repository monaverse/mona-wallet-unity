using System;
using UnityEngine;

namespace Monaverse.Modal.UI.Extensions
{
    public static class StringExtensions
    {
        public static string ToInitials(this string fullName)
        {
            // Handle null or empty strings
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty;
            }

            // Split the full name by spaces
            var nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // If only one word is entered, return the first letter as uppercase
            if (nameParts.Length == 1)
            {
                return nameParts[0].Length > 0 ? char.ToUpper(nameParts[0][0]).ToString() : string.Empty;
            }
            
            // Get the first character of each part and concatenate them
            var initials = "";
            foreach (var part in nameParts)
                initials += char.ToUpper(part[0]);

            return initials;
        }
    }
}