using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    public class PersonaAuthorizeAttribute : AuthorizeAttribute
    {
        public string Module { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            CORE core = new CORE();
            User user = filterContext.HttpContext.User.GetLogged(core);
            if (user == null)
                throw new UnauthorizedAccessException();

            if (!string.IsNullOrWhiteSpace(Module) && !user.canUseModule(Module))
                throw new UnauthorizedAccessException();

            if (string.IsNullOrWhiteSpace(Users) && string.IsNullOrWhiteSpace(Roles))
                return;

            if (Users.Split(' ').Contains(user.username))
                return;
            
            foreach(string role in Roles.Split(' '))
            {
                if (user.IsInGroup(role))
                    return;
            }

            throw new UnauthorizedAccessException();
        }
    }
}