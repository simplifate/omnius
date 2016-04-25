using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static partial class ExtendMethods
    {
        public static char ToUpper(this char input)
        {
            int i = input;
            if (i >= 97 && i <= 122)
                i -= 32;

            return (char)i;
        }
        public static char ToLower(this char input)
        {
            int i = input;
            if (i >= 65 && i <= 90)
                i += 32;

            return (char)i;
        }

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
            input = input.Normalize(NormalizationForm.FormD);

            // remove diacriticts
            input = new string(input
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            // remove special symbols
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            var splitted = rgx.Split(input)
                .Where(s => s.Length != 0) // remove empty
                .Select(s => s[0].ToUpper() + s.Substring(1).ToLower()) // good case
                .ToArray();

            // merge
            input = string.Join("", splitted);

            return input;
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
