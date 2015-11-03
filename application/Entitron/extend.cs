using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System
{
    public static partial class ExtendMethods
    {
        public static string Random(this string str, int length, string chars = "abcdefghijklmnopqrstuvwxyz")
        {
            var random = new Random();
            str = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return str;
        }
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            int i = 0;
            foreach(TSource item in source)
            {
                if (predicate(item))
                    return i;

                i++;
            }

            return -1;
        }
    }
}
