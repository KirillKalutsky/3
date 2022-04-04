using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3
{
    public static class Extensions
    {
        public static IEnumerable<string> ToHexEnumerable(this string str)
        {
            return str.Select(ch => Convert.ToInt32(ch).ToString(format: "X"));
        }

        public static string FromHexEnumerableToString(this IEnumerable<string> hex)
        {
            return string.Concat(hex.Select(x=> (char)int.Parse(x, System.Globalization.NumberStyles.HexNumber)));
        }
    }
}
