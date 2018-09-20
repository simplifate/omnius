using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Persona;

namespace System.Web.Mvc
{
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

            // Is logged / guest
            COREobject core = COREobject.i;
            if (core.User == null)
                core.User = Persona.GetAuthenticatedUser(filterContext.HttpContext.User.Identity.Name, core.Application?.IsAllowedGuests == true, filterContext.HttpContext.Request);

            // authorize
            try
            {
                core.User.Authorize(NeedsAdmin, Module, Users, core.Application);
            }
            catch (NotAuthorizedException)
            {
                filterContext.Result = new Http403Result();
            }
        }
    }
}