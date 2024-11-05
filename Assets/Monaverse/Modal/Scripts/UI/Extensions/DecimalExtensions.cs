using System;
using System.Globalization;

namespace Monaverse.Modal.UI.Extensions
{
    public static class DecimalExtensions
    {
        public static string GetFormattedNativePrice(this decimal price, int precision = 6)
        {
            // Check if the number is an integer
            // Non-integer value: display all significant decimal digits, removing trailing zeros
            return price.ToString(price == Math.Truncate(price) ?
                // Integer value: display two decimal places
                "#,##0.00" : "#,##0.############################", CultureInfo.InvariantCulture);
        }
        
    }
}