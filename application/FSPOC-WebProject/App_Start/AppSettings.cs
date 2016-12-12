using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneLogin.Saml
{
    public class AppSettings
    {
        public string assertionConsumerServiceUrl;

        // dev
        public string issuer = "omnius-test";

        // production
        //public string issuer = "omnius-production";

        public AppSettings()
        {
            HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
            UrlHelper url = new UrlHelper(context.Request.RequestContext);

            // dev
            this.assertionConsumerServiceUrl = "http://osstest.rwe.cz/CORE/Account/LoginSaml";

            // production
            //this.assertionConsumerServiceUrl = "http://oss.rwe.cz/CORE/Account/LoginSaml";
        }
    }
}
