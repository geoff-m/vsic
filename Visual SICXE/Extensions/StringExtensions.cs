using System.Linq;

namespace Visual_SICXE.Extensions
{
    public static class StringExtensions
    {
        public static string Reverse(this string s)
        {
            return new string(s.Reverse<char>().ToArray());
        }
    }
}
