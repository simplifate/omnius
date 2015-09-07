using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System
{
    public static partial class ExtendMethods
    {
        public static string Random(this string str, int length, string chars = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            var random = new Random();
            str = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return str;
        }
    }
}
