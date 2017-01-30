using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace FSS.Omnius.Controllers.CORE
{
    public class Utilities
    {
        /// <summary>
        /// Skloňování slov (1 jablko, 2 jablka, 5 jablek...)
        /// </summary>
        /// <param name="count">Počet</param>
        /// <param name="one">1</param>
        /// <param name="two">2 - 4</param>
        /// <param name="five">5</param>
        /// <param name="zero">0 (pokud se liší od 5)</param>
        public static string Numbering(int count, string one, string two, string five, string zero = "")
        {
            switch (count)
            {
                case 0: return string.IsNullOrEmpty(zero) ? five : zero;
                case 1: return one;
                case 2: return two;
                case 3: return two;
                case 4: return two;
                default: return five;
            }
        }

        public static string StripTags(string input)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input ?? "");
            return doc.DocumentNode.InnerText;
        }
    }
}