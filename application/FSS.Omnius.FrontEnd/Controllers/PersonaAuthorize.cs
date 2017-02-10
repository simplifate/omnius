using FSS.Omnius.FrontEnd;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OneLogin.Saml;
using System.Linq;
using System.Threading.Tasks;

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
        public bool NeedsAdmin { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Allow Anonymous
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                return;

            CORE core = filterContext.HttpContext.GetCORE();
            User user = filterContext.HttpContext.GetLoggedUser();

            // not logged -> redirect
            if (user == null)
            {
                // Otherwise, perform auth over IDS
                //SAML Authentication
                //RequestBuilder builder = new RequestBuilder();
                //filterContext.Result = new RedirectResult(builder.GetAuthUrl());

                //OLD Authentication
                filterContext.Result = new RedirectToRouteResult(
                    "Persona",
                    new Web.Routing.RouteValueDictionary(new
                    {
                        @controller = "Account",
                        @action = "Login",
                        @returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery
                    })
                );

                return;
            }

            // Module
            if (!string.IsNullOrWhiteSpace(Module) && !user.canUseModule(Module))
            {
                filterContext.Result = new Http403Result();
                return;
            }

            // User
            if (!string.IsNullOrWhiteSpace(Users) && !Users.Split(' ').Contains(user.UserName))
            {
                filterContext.Result = new Http403Result();
                return;
            }

            // Role
            //if (!string.IsNullOrWhiteSpace(Roles))
            //{
            //    bool containsGroup = false;
            //    foreach (string role in Roles.Split(' '))
            //    {
            //        if (user.IsInGroup(role))
            //            containsGroup = true;
            //    }

            //    if (!containsGroup)
            //    {
            //        filterContext.Result = new Http403Result();
            //        return;
            //    }
            //}

            // Admin
            if (NeedsAdmin && !user.isAdmin())
            {
                filterContext.Result = new Http403Result();
                return;
            }
        }
    }
}