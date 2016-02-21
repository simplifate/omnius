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
        public static User GetLoggedUser(this HttpContextBase context)
        {
            CORE core = context.GetCORE();
            return context.User.GetLogged(core);
        }

        public static CORE GetCORE(this HttpContextBase context)
        {
            if (!context.Items.Contains("CORE"))
                context.Items.Add("CORE", new CORE());
            return (CORE)context.Items["CORE"];
        }

        public static User GetLogged(this IPrincipal user, CORE core)
        {
            
            if (core.User != null && core.User.UserName == user.Identity.Name)
                return core.User;

            return core.Persona.AuthenticateUser(user.Identity.Name);
        }
    }
}
