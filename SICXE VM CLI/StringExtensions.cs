using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICXE_VM_CLI
{
    static class StringExtensions
    {
        public static bool EqualsAnyIgnoreCase(this string str, params string[] possibilities)
        {
            foreach (var p in possibilities)
                if (str.Equals(p, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>
        /// Tries to parse the given string as an integer.
        /// If the last character is 'd', the rest of the string is interpreted as a decimal number.
        /// If the last character is 'h', or if the string begins with "0x", the rest of the string is interpreted as a hexadecimal number.
        /// Otherwise, a decimal interpetation will be used.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="result">The result of parsing, if successful.</param>
        /// <returns>Whether the parse was successful.</returns>
        public static bool TryParseAsSuffixedInt(this string str, out int result)
        {
            char last = str.Last();
            if (last == 'h' || last == 'H')
            {
                // Marked by hex as suffix.
                return int.TryParse(str.Substring(0, str.Length - 1), System.Globalization.NumberStyles.HexNumber, null, out result);
            }
            if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                // Marked as hex by prefix.
                return int.TryParse(str.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out result);
            }
            if (last == 'd' || last == 'D')
            {
                // Marked as decimal by suffix.
                return int.TryParse(str.Substring(0, str.Length - 1), out result);
            }
            
            // No markers, assume decimal.
            return int.TryParse(str, out result);
        }
    }
}
