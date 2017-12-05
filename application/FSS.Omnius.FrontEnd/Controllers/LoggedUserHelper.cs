using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Principal;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity;

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
        //public static void SetApp(this HttpContextBase context, int appId)
        //{
        //    context.GetCORE().Entitron.AppId = appId;
        //}
        //public static void SetApp(this HttpContextBase context, string appName)
        //{
        //    context.GetCORE().Entitron.AppName = appName;
        //}
        //public static void SetApp(this HttpContextBase context, Application app)
        //{
        //    context.GetCORE().Entitron.Application = app;
        //}

        public static User GetLogged(this IPrincipal user, CORE core)
        {
            DBEntities context = DBEntities.instance;
            if (core.User == null || core.User.UserName != user.Identity.Name)
                core.User = core.Persona.AuthenticateUser(user.Identity.Name);

            if (core.User == null && core.Entitron.Application != null && core.Entitron.Application.IsAllowedGuests)
            {
                core.User = context.Users.SingleOrDefault(x => x.UserName == "guest");
            }
            return core.User;
        }
    }
}
