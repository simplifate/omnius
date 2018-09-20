using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static partial class ExtendMethods
    {
        private static Dictionary<string, string> LOCAL_CHAR_TO_URL = new Dictionary<string, string>()
        {
            {"À" , "A"},{ "Á" , "A"},{ "Â" , "A"},{ "Ã" , "A"},{ "Ä" , "A"},{ "Å" , "A"},{ "Æ" , "A"},{ "Ā" , "A"},{ "Ą" , "A"},{ "Ă" , "A"},{ "Ç" , "C"},{
            "Ć" , "C"},{ "Č" , "C"},{ "Ĉ" , "C"},{ "Ċ" , "C"},{ "Ď" , "D"},{ "Đ" , "D"},{ "È" , "E"},{ "É" , "E"},{ "Ê" , "E"},{ "Ë" , "E"},{
            "Ē" , "E"},{ "Ę" , "E"},{ "Ě" , "E"},{ "Ĕ" , "E"},{ "Ė" , "E"},{ "Ĝ" , "G"},{ "Ğ" , "G"},{ "Ġ" , "G"},{ "Ģ" , "G"},{ "Ĥ" , "H"},{
            "Ħ" , "H"},{ "Ì" , "I"},{ "Í" , "I"},{ "Î" , "I"},{ "Ï" , "I"},{ "Ī" , "I"},{ "Ĩ" , "I"},{ "Ĭ" , "I"},{ "Į" , "I"},{ "İ" , "I"},{
            "Ĳ" , "IJ"},{ "Ĵ" , "J"},{ "Ķ" , "K"},{ "Ľ" , "K"},{ "Ĺ" , "K"},{ "Ļ" , "K"},{ "Ŀ" , "K"},{ "Ł" , "L"},{ "Ñ" , "N"},{ "Ń" , "N"},{
            "Ň" , "N"},{ "Ņ" , "N"},{ "Ŋ" , "N"},{ "Ò" , "O"},{ "Ó" , "O"},{ "Ô" , "O"},{ "Õ" , "O"},{ "Ö" , "O"},{ "Ø" , "O"},{ "Ō" , "O"},{
            "Ő" , "O"},{ "Ŏ" , "O"},{ "Œ" , "OE"},{ "Ŕ" , "R"},{ "Ř" , "R"},{ "Ŗ" , "R"},{ "Ś" , "S"},{ "Ş" , "S"},{ "Ŝ" , "S"},{ "Ș" , "S"},{
            "Š" , "S"},{ "Ť" , "T"},{ "Ţ" , "T"},{ "Ŧ" , "T"},{ "Ț" , "T"},{ "Ù" , "U"},{ "Ú" , "U"},{ "Û" , "U"},{ "Ü" , "U"},{ "Ū" , "U"},{
            "Ů" , "U"},{ "Ű" , "U"},{ "Ŭ" , "U"},{ "Ũ" , "U"},{ "Ų" , "U"},{ "Ŵ" , "W"},{ "Ŷ" , "Y"},{ "Ÿ" , "Y"},{ "Ý" , "Y"},{ "Ź" , "Z"},{
            "Ż" , "Z"},{ "Ž" , "Z"},{ "à" , "a"},{ "á" , "a"},{ "â" , "a"},{ "ã" , "a"},{ "ä" , "a"},{ "ā" , "a"},{ "ą" , "a"},{ "ă" , "a"},{
            "å" , "a"},{ "æ" , "ae"},{ "ç" , "c"},{ "ć" , "c"},{ "č" , "c"},{ "ĉ" , "c"},{ "ċ" , "c"},{ "ď" , "d"},{ "đ" , "d"},{ "è" , "e"},{
            "é" , "e"},{ "ê" , "e"},{ "ë" , "e"},{ "ē" , "e"},{ "ę" , "e"},{ "ě" , "e"},{ "ĕ" , "e"},{ "ė" , "e"},{ "ƒ" , "f"},{ "ĝ" , "g"},{
            "ğ" , "g"},{ "ġ" , "g"},{ "ģ" , "g"},{ "ĥ" , "h"},{ "ħ" , "h"},{ "ì" , "i"},{ "í" , "i"},{ "î" , "i"},{ "ï" , "i"},{ "ī" , "i"},{
            "ĩ" , "i"},{ "ĭ" , "i"},{ "į" , "i"},{ "ı" , "i"},{ "ĳ" , "ij"},{ "ĵ" , "j"},{ "ķ" , "k"},{ "ĸ" , "k"},{ "ł" , "l"},{ "ľ" , "l"},{
            "ĺ" , "l"},{ "ļ" , "l"},{ "ŀ" , "l"},{ "ñ" , "n"},{ "ń" , "n"},{ "ň" , "n"},{ "ņ" , "n"},{ "ŉ" , "n"},{ "ŋ" , "n"},{ "ò" , "o"},{
            "ó" , "o"},{ "ô" , "o"},{ "õ" , "o"},{ "ö" , "o"},{ "ø" , "o"},{ "ō" , "o"},{ "ő" , "o"},{ "ŏ" , "o"},{ "œ" , "oe"},{ "ŕ" , "r"},{
            "ř" , "r"},{ "ŗ" , "r"},{ "ś" , "s"},{ "š" , "s"},{ "ş" , "s"},{ "ť" , "t"},{ "ţ" , "t"},{ "ù" , "u"},{ "ú" , "u"},{ "û" , "u"},{
            "ü" , "u"},{ "ū" , "u"},{ "ů" , "u"},{ "ű" , "u"},{ "ŭ" , "u"},{ "ũ" , "u"},{ "ų" , "u"},{ "ŵ" , "w"},{ "ÿ" , "y"},{ "ý" , "y"},{
            "ŷ" , "y"},{ "ż" , "z"},{ "ź" , "z"},{ "ž" , "z"},{ "ß" , "ss"},{ "ſ" , "ss"},{ "Α" , "A"},{ "Ά" , "A"},{ "Ἀ" , "A"},{ "Ἁ" , "A"},{
            "Ἂ" , "A"},{ "Ἃ" , "A"},{ "Ἄ" , "A"},{ "Ἅ" , "A"},{ "Ἆ" , "A"},{ "Ἇ" , "A"},{ "ᾈ" , "A"},{ "ᾉ" , "A"},{ "ᾊ" , "A"},{ "ᾋ" , "A"},{
            "ᾌ" , "A"},{ "ᾍ" , "A"},{ "ᾎ" , "A"},{ "ᾏ" , "A"},{ "Ᾰ" , "A"},{ "Ᾱ" , "A"},{ "Ὰ" , "A"},{ "Ά" , "A"},{ "ᾼ" , "A"},{ "Β" , "B"},{
            "Γ" , "G"},{ "Δ" , "D"},{ "Ε" , "E"},{ "Έ" , "E"},{ "Ἐ" , "E"},{ "Ἑ" , "E"},{ "Ἒ" , "E"},{ "Ἓ" , "E"},{ "Ἔ" , "E"},{ "Ἕ" , "E"},{
            "Έ" , "E"},{ "Ὲ" , "E"},{ "Ζ" , "Z"},{ "Η" , "I"},{ "Ή" , "I"},{ "Ἠ" , "I"},{ "Ἡ" , "I"},{ "Ἢ" , "I"},{ "Ἣ" , "I"},{ "Ἤ" , "I"},{
            "Ἥ" , "I"},{ "Ἦ" , "I"},{ "Ἧ" , "I"},{ "ᾘ" , "I"},{ "ᾙ" , "I"},{ "ᾚ" , "I"},{ "ᾛ" , "I"},{ "ᾜ" , "I"},{ "ᾝ" , "I"},{ "ᾞ" , "I"},{
            "ᾟ" , "I"},{ "Ὴ" , "I"},{ "Ή" , "I"},{ "ῌ" , "I"},{ "Θ" , "TH"},{ "Ι" , "I"},{ "Ί" , "I"},{ "Ϊ" , "I"},{ "Ἰ" , "I"},{ "Ἱ" , "I"},{
            "Ἲ" , "I"},{ "Ἳ" , "I"},{ "Ἴ" , "I"},{ "Ἵ" , "I"},{ "Ἶ" , "I"},{ "Ἷ" , "I"},{ "Ῐ" , "I"},{ "Ῑ" , "I"},{ "Ὶ" , "I"},{ "Ί" , "I"},{
            "Κ" , "K"},{ "Λ" , "L"},{ "Μ" , "M"},{ "Ν" , "N"},{ "Ξ" , "KS"},{ "Ο" , "O"},{ "Ό" , "O"},{ "Ὀ" , "O"},{ "Ὁ" , "O"},{ "Ὂ" , "O"},{
            "Ὃ" , "O"},{ "Ὄ" , "O"},{ "Ὅ" , "O"},{ "Ὸ" , "O"},{ "Ό" , "O"},{ "Π" , "P"},{ "Ρ" , "R"},{ "Ῥ" , "R"},{ "Σ" , "S"},{ "Τ" , "T"},{
            "Υ" , "Y"},{ "Ύ" , "Y"},{ "Ϋ" , "Y"},{ "Ὑ" , "Y"},{ "Ὓ" , "Y"},{ "Ὕ" , "Y"},{ "Ὗ" , "Y"},{ "Ῠ" , "Y"},{ "Ῡ" , "Y"},{ "Ὺ" , "Y"},{
            "Ύ" , "Y"},{ "Φ" , "F"},{ "Χ" , "X"},{ "Ψ" , "PS"},{ "Ω" , "O"},{ "Ώ" , "O"},{ "Ὠ" , "O"},{ "Ὡ" , "O"},{ "Ὢ" , "O"},{ "Ὣ" , "O"},{
            "Ὤ" , "O"},{ "Ὥ" , "O"},{ "Ὦ" , "O"},{ "Ὧ" , "O"},{ "ᾨ" , "O"},{ "ᾩ" , "O"},{ "ᾪ" , "O"},{ "ᾫ" , "O"},{ "ᾬ" , "O"},{ "ᾭ" , "O"},{
            "ᾮ" , "O"},{ "ᾯ" , "O"},{ "Ὼ" , "O"},{ "Ώ" , "O"},{ "ῼ" , "O"},{ "α" , "a"},{ "ά" , "a"},{ "ἀ" , "a"},{ "ἁ" , "a"},{ "ἂ" , "a"},{
            "ἃ" , "a"},{ "ἄ" , "a"},{ "ἅ" , "a"},{ "ἆ" , "a"},{ "ἇ" , "a"},{ "ᾀ" , "a"},{ "ᾁ" , "a"},{ "ᾂ" , "a"},{ "ᾃ" , "a"},{ "ᾄ" , "a"},{
            "ᾅ" , "a"},{ "ᾆ" , "a"},{ "ᾇ" , "a"},{ "ὰ" , "a"},{ "ά" , "a"},{ "ᾰ" , "a"},{ "ᾱ" , "a"},{ "ᾲ" , "a"},{ "ᾳ" , "a"},{ "ᾴ" , "a"},{
            "ᾶ" , "a"},{ "ᾷ" , "a"},{ "β" , "b"},{ "γ" , "g"},{ "δ" , "d"},{ "ε" , "e"},{ "έ" , "e"},{ "ἐ" , "e"},{ "ἑ" , "e"},{ "ἒ" , "e"},{
            "ἓ" , "e"},{ "ἔ" , "e"},{ "ἕ" , "e"},{ "ὲ" , "e"},{ "έ" , "e"},{ "ζ" , "z"},{ "η" , "i"},{ "ή" , "i"},{ "ἠ" , "i"},{ "ἡ" , "i"},{
            "ἢ" , "i"},{ "ἣ" , "i"},{ "ἤ" , "i"},{ "ἥ" , "i"},{ "ἦ" , "i"},{ "ἧ" , "i"},{ "ᾐ" , "i"},{ "ᾑ" , "i"},{ "ᾒ" , "i"},{ "ᾓ" , "i"},{
            "ᾔ" , "i"},{ "ᾕ" , "i"},{ "ᾖ" , "i"},{ "ᾗ" , "i"},{ "ὴ" , "i"},{ "ή" , "i"},{ "ῂ" , "i"},{ "ῃ" , "i"},{ "ῄ" , "i"},{ "ῆ" , "i"},{
            "ῇ" , "i"},{ "θ" , "th"},{ "ι" , "i"},{ "ί" , "i"},{ "ϊ" , "i"},{ "ΐ" , "i"},{ "ἰ" , "i"},{ "ἱ" , "i"},{ "ἲ" , "i"},{ "ἳ" , "i"},{
            "ἴ" , "i"},{ "ἵ" , "i"},{ "ἶ" , "i"},{ "ἷ" , "i"},{ "ὶ" , "i"},{ "ί" , "i"},{ "ῐ" , "i"},{ "ῑ" , "i"},{ "ῒ" , "i"},{ "ΐ" , "i"},{
            "ῖ" , "i"},{ "ῗ" , "i"},{ "κ" , "k"},{ "λ" , "l"},{ "μ" , "m"},{ "ν" , "n"},{ "ξ" , "ks"},{ "ο" , "o"},{ "ό" , "o"},{ "ὀ" , "o"},{
            "ὁ" , "o"},{ "ὂ" , "o"},{ "ὃ" , "o"},{ "ὄ" , "o"},{ "ὅ" , "o"},{ "ὸ" , "o"},{ "ό" , "o"},{ "π" , "p"},{ "ρ" , "r"},{ "ῤ" , "r"},{
            "ῥ" , "r"},{ "σ" , "s"},{ "ς" , "s"},{ "τ" , "t"},{ "υ" , "y"},{ "ύ" , "y"},{ "ϋ" , "y"},{ "ΰ" , "y"},{ "ὐ" , "y"},{ "ὑ" , "y"},{
            "ὒ" , "y"},{ "ὓ" , "y"},{ "ὔ" , "y"},{ "ὕ" , "y"},{ "ὖ" , "y"},{ "ὗ" , "y"},{ "ὺ" , "y"},{ "ύ" , "y"},{ "ῠ" , "y"},{ "ῡ" , "y"},{
            "ῢ" , "y"},{ "ΰ" , "y"},{ "ῦ" , "y"},{ "ῧ" , "y"},{ "φ" , "f"},{ "χ" , "x"},{ "ψ" , "ps"},{ "ω" , "o"},{ "ώ" , "o"},{ "ὠ" , "o"},{
            "ὡ" , "o"},{ "ὢ" , "o"},{ "ὣ" , "o"},{ "ὤ" , "o"},{ "ὥ" , "o"},{ "ὦ" , "o"},{ "ὧ" , "o"},{ "ᾠ" , "o"},{ "ᾡ" , "o"},{ "ᾢ" , "o"},{
            "ᾣ" , "o"},{ "ᾤ" , "o"},{ "ᾥ" , "o"},{ "ᾦ" , "o"},{ "ᾧ" , "o"},{ "ὼ" , "o"},{ "ώ" , "o"},{ "ῲ" , "o"},{ "ῳ" , "o"},{ "ῴ" , "o"},{
            "ῶ" , "o"},{ "ῷ" , "o"},{ "¨" , ""},{ "΅" , ""},{ "᾿" , ""},{ "῾" , ""},{ "῍" , ""},{ "῝" , ""},{ "῎" , ""},{ "῞" , ""},{
            "῏" , ""},{ "῟" , ""},{ "῀" , ""},{ "῁" , ""},{ "΄" , ""},{ "΅" , ""},{ "`" , ""},{ "῭" , ""},{ "ͺ" , ""},{ "᾽" , ""},{
            "А" , "A"},{ "Б" , "B"},{ "В" , "V"},{ "Г" , "G"},{ "Д" , "D"},{ "Е" , "E"},{ "Ё" , "E"},{ "Ж" , "ZH"},{ "З" , "Z"},{ "И" , "I"},{
            "Й" , "I"},{ "К" , "K"},{ "Л" , "L"},{ "М" , "M"},{ "Н" , "N"},{ "О" , "O"},{ "П" , "P"},{ "Р" , "R"},{ "С" , "S"},{ "Т" , "T"},{
            "У" , "U"},{ "Ф" , "F"},{ "Х" , "KH"},{ "Ц" , "TS"},{ "Ч" , "CH"},{ "Ш" , "SH"},{ "Щ" , "SHCH"},{ "Ы" , "Y"},{ "Э" , "E"},{ "Ю" , "YU"},{
            "Я" , "YA"},{ "а" , "A"},{ "б" , "B"},{ "в" , "V"},{ "г" , "G"},{ "д" , "D"},{ "е" , "E"},{ "ё" , "E"},{ "ж" , "ZH"},{ "з" , "Z"},{
            "и" , "I"},{ "й" , "I"},{ "к" , "K"},{ "л" , "L"},{ "м" , "M"},{ "н" , "N"},{ "о" , "O"},{ "п" , "P"},{ "р" , "R"},{ "с" , "S"},{
            "т" , "T"},{ "у" , "U"},{ "ф" , "F"},{ "х" , "KH"},{ "ц" , "TS"},{ "ч" , "CH"},{ "ш" , "SH"},{ "щ" , "SHCH"},{ "ы" , "Y"},{ "э" , "E"},{
            "ю" , "YU"},{ "я" , "YA"},{ "Ъ" , ""},{ "ъ" , ""},{ "Ь" , ""},{ "ь" , ""},{ "ð" , "d"},{ "Ð" , "D"},{ "þ" , "th"},{ "Þ" , "TH" }
        };

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
        
        public static string FirstLine(this string str)
        {
            if (str == null)
                return null;

            return str.Split(Environment.NewLine.ToArray()).First();
        }
        public static string Truncate(this string str, int length)
        {
            if (length >= str.Length)
                return str;

            return str.Substring(0, length);
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
                {
                    if (pair.Key as string == "DataSource")
                    {
                        continue;
                    }
                    source[pair.Key] = pair.Value;
                }
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

        public static string StrTr(string source, Dictionary<string, string> replace)
        {
            char[] chars = source.ToCharArray();
            string[] input = new string[chars.Length];
            bool[] replaced = new bool[chars.Length];

            for (int i = 0; i < chars.Length; i++) {
                input[i] = chars[i].ToString();
            }

            for (int j = 0; j < input.Length; j++)
                replaced[j] = false;

            foreach(KeyValuePair<string, string> r in replace) { 
                for (int j = 0; j < input.Length; j++)
                    if (replaced[j] == false && input[j] == r.Key) {
                        input[j] = r.Value;
                        replaced[j] = true;
                    }
            }
            return string.Join("", input);
        }

        public static string ToASCII(string input)
        {
            return StrTr(input, LOCAL_CHAR_TO_URL);
        }

        public static string URLSafeString(string input, bool toLower = false, bool allowUnderscore = true)
        {
            Regex rxBadChars = new Regex(allowUnderscore ? "[^a-z0-9-_]+" : "[^a-z0-9-]+", RegexOptions.IgnoreCase);
            Regex rxStartEnd = new Regex("^-|-$");
            Regex rxDash = new Regex("-{2,}");

            string safe = ToASCII(input);
            safe = toLower ? safe.ToLower() : safe;
            safe = rxBadChars.Replace(safe, "-");
            safe = rxStartEnd.Replace(safe, "");
            safe = rxDash.Replace(safe, "-");

            return safe;
        }

        public static string EscapeVerbatim(this string input)
        {
            return input.Replace("\"", "\"\"");
        }

        /// <summary>
        /// Returns a string with backslashes before characters that need to be quoted
        /// </summary>
        /// <param name="InputTxt">Text string need to be escape with slashes</param>
        public static string AddSlashes(this string input)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            return Regex.Replace(input, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
        }

        /// <summary>
        /// Un-quotes a quoted string
        /// </summary>
        /// <param name="InputTxt">Text string need to be escape with slashes</param>
        public static string StripSlashes(this string input)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            return System.Text.RegularExpressions.Regex.Replace(input, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2");
        }

        /// <summary>
        /// list to tuple
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void Deconstruct<T>(this IList<T> list, out T first, out T second)
        {
            if (list.Count < 2)
                throw new Exception("list has no 2 values");

            first = list[0];
            second = list[1];
        }
        
        public static bool AnyInner(this Exception exception, Func<Exception, bool> expression)
        {
            if (exception == null)
                return false;

            return expression(exception) || exception.InnerException.AnyInner(expression);
        }

        public static bool AllInner(this Exception exception, Func<Exception, bool> expression)
        {
            if (exception == null)
                return false;

            return expression(exception) && exception.InnerException.AllInner(expression);
        }
    }
}