using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.CORE
{
    public class InfoController : Controller
    {
        [PersonaAuthorize]
        // GET: Info
        public string Index()
        {
            string result = "";
            result += $"App code using: {System.Security.Principal.WindowsIdentity.GetCurrent().Name}<br/>";
            result += $"Is user auth: {User.Identity.IsAuthenticated.ToString()}<br/>";
            result += $"Auth type: {User.Identity.AuthenticationType}<br/>";
            result += $"User name: {User.Identity.Name}<br/>";

            return result;
        }

        public bool Leave()
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();

            core.Persona.NotLogOff(User.Identity.Name);

            return true;
        }

        public bool LogOut()
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();

            core.Persona.LogOff(User.Identity.Name);

            return true;
        }
    }
}