using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lambda
{
    public static class StringExtensions
    {
        public static string Reverse(this string text)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

            StringBuilder sb = new StringBuilder(text.Length);
            for (int n = text.Length - 1; n >= 0; n--)
            {
                sb.Append(text[n]);
            }
            return sb.ToString();
        }

        public static IEnumerable<char> ReverseEnumerable(this string text)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

            for (int n = text.Length - 1; n >= 0; n--)
            {
                yield return text[n];
            }
        }
    }
}
