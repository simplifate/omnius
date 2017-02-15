using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace FSS.Omnius.FrontEnd.Utils
{
    public static class JsTranslationsCollection
    {
        public static Dictionary<string, string> strings = new Dictionary<string, string>();

        static JsTranslationsCollection()
        {
            strings.Add("financialSecurityCoef", "Koeficient jistoty");
            strings.Add("financialSecurityAmount", "Výše jistoty");
            strings.Add("notInsertedFinancialDepo", "Nevložili jste finanční jistotu");
            strings.Add("notEnoughtFinancialDepo", "Nevložili jste dostatečnou finanční jistotu<br><small>Zbývá: $remaining</small>");
            strings.Add("auctionPaused", "Aukce byla pozastavena");
            strings.Add("auctionEnded", "Aukce již skončila");
            strings.Add("auctionCanceled", "Aukce byla zrušena");
        }

        public static Dictionary<string, string> Translate(T t)
        {
            return strings.Select(x => new KeyValuePair<string, string>(x.Key, t._(x.Value))).ToDictionary(x => x.Key, y => y.Value);
        }

        public static string GetJson(T t)
        {
            return Json.Encode(Translate(t));
        }

        public static void Export(string file, T t)
        {
            File.WriteAllText(file, GetJson(t));
        }

        public static void ExportKeysForPo(string file)
        {
            var dict = strings.Select(x => "msgid \"" + x.Value + "\"\nmsgstr \"\"");
            File.WriteAllText(file, string.Join("\n\n", dict));
        }
    }
}