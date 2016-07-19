using FSPOC_WebProject;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
            CORE core = filterContext.HttpContext.GetCORE();
            User user = filterContext.HttpContext.GetLoggedUser();

            // not logged
            if (user == null)
            {
                Task<IdentityResult> createUserTaks = null;

                // SAML -> create new user
                if (filterContext.HttpContext.User.Identity.AuthenticationType == "ApplicationCookie")
                {
                    string username = filterContext.HttpContext.User.Identity.Name;
                    user = new User
                    {
                        UserName = username,
                        DisplayName = username,
                        Email = username,
                        isLocalUser = true,
                        localExpiresAt = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow,
                        LastLogout = DateTime.UtcNow,
                        CurrentLogin = DateTime.UtcNow
                    };
                    createUserTaks = filterContext.HttpContext.GetOwinContext().Get<ApplicationUserManager>().CreateAsync(user, "".Random(20) + "".Random(2, "_-+=<>;:,.") + "".Random(3, "1234567890"));
                    createUserTaks.Wait();
                }

                // redirect
                if (createUserTaks == null || !createUserTaks.Result.Succeeded)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        "Persona",
                        new Web.Routing.RouteValueDictionary(new
                        {
                            @controller = "Account",
                            @action = "Login",
                            @returnUrl = filterContext.HttpContext.Request.Url.AbsolutePath
                        })
                    );
                    return;
                }
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