using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Principal;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.CORE;

namespace System
{
    public static partial class ExtendMethods
    {
        public static User GetLogged(this IPrincipal user, CORE core = null)
        {
            core = core ?? new CORE();
            string username = user.Identity.Name;
            int domainIndex = username.IndexOf('\\');
            string server = null;

            // username with domain
            if (domainIndex != -1)
            {
                server = username.Substring(0, domainIndex);
                username = username.Substring(domainIndex + 1);
            }

            return core.Persona.getUser(username, server);
        }
    }
}
