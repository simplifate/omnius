using System;
using System.Collections.Generic;
using NGettext;
using System.Globalization;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.FrontEnd.Utils
{
    public class T : ITranslator
    {
        public static Dictionary<Locale, ICatalog> catalog = new Dictionary<Locale, ICatalog>();
        public Locale cultureInfo { get; set; }
        public T(Locale cultureInfo)
        {
            this.cultureInfo = cultureInfo;

            if (!catalog.ContainsKey(cultureInfo))
            {
                catalog.Add(cultureInfo, new Catalog("Omnius", System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Locale"), new CultureInfo(cultureInfo.ToString())));
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