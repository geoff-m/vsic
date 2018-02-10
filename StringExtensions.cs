using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sicsim
{
    public static class StringExtensions
    {
        public static string Reverse(this string s)
        {
            return new string(s.Reverse<char>().ToArray());
        }
    }
}
