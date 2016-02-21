using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    internal class Http403Result : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = 403;
        }
    }

    public class PersonaAuthorizeAttribute : AuthorizeAttribute
    {
        public string Module { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            CORE core = filterContext.HttpContext.GetCORE();

            User user = filterContext.HttpContext.User.GetLogged(core);
            // not logged -> redirect
            if (user == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    "Persona",
                    new Web.Routing.RouteValueDictionary(new {
                        @controller = "Account",
                        @action = "Login",
                        @returnUrl = filterContext.HttpContext.Request.Url.AbsolutePath
                    })
                );
                return;
            }

            if (!string.IsNullOrWhiteSpace(Module) && !user.canUseModule(Module))
            {
                filterContext.Result = new Http403Result();
                return;
            }

            if (string.IsNullOrWhiteSpace(Users) && string.IsNullOrWhiteSpace(Roles))
                return;

            if (Users.Split(' ').Contains(user.UserName))
                return;
            
            foreach(string role in Roles.Split(' '))
            {
                if (user.IsInGroup(role))
                    return;
            }

            filterContext.Result = new Http403Result();
        }
    }
}