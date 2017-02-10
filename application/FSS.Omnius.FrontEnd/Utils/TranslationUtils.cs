using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NGettext;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace FSS.Omnius.FrontEnd.Utils
{
    public class T : FSS.Omnius.Modules.CORE.ITranslator
    {
        public static Dictionary<string, ICatalog> catalog = new Dictionary<string, ICatalog>();
        public string cultureInfo { get; set; }
        public T(string cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            if (!catalog.ContainsKey(cultureInfo))
            {
                catalog.Add(cultureInfo, new Catalog("Omnius", System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Locale"), new CultureInfo(cultureInfo)));
            }
        }

        public string _(string sourceString)
        {
            return catalog[cultureInfo].GetString(sourceString);
        }

        public string _(string sourceString, params object[] args)
        {
            return catalog[cultureInfo].GetString(sourceString, args);
        }
    }
}