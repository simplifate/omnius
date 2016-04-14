using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        public static string RemoveDiacritics(this string input)
        {
            string decomposed = input.Normalize(NormalizationForm.FormD);
            char[] filtered = decomposed
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            string newString = new string(filtered);
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            newString = rgx.Replace(newString, "");

            return newString;
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> source, IEnumerable<KeyValuePair<TKey, TValue>> range)
        {
            foreach (var pair in range)
            {
                if (source.ContainsKey(pair.Key))
                    source[pair.Key] = pair.Value;
                else
                    source.Add(pair.Key, pair.Value);
            }
        }
        public static void AddOrUpdateRange<TKey, TValue>(this Dictionary<TKey, TValue> source, IEnumerable<KeyValuePair<TKey, TValue>> range)
        {
            if (range == null)
                return;

            foreach (var pair in range)
            {
                source[pair.Key] = pair.Value;
            }
        }
        public static void ChangeKey<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey oldKey, TKey newKey)
        {
            TValue value = source[oldKey];
            source.Remove(oldKey);
            source.Add(newKey, value);
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

        public static void AddRange<TKey>(this HashSet<TKey> source, IEnumerable<TKey> items)
        {
            foreach (TKey item in items)
            {
                source.Add(item);
            }
        }
    }
}
