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
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> source, IEnumerable<KeyValuePair<TKey, TValue>> range)
        {
            foreach (var pair in range)
            {
                source.Add(pair.Key, pair.Value);
            }
        }
        public static void AddOrUpdateRange<TKey, TValue>(this Dictionary<TKey, TValue> source, IEnumerable<KeyValuePair<TKey, TValue>> range)
        {
            foreach (var pair in range)
            {
                source[pair.Key] = pair.Value;
            }
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
