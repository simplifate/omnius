using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.CORE
{
    [Authorize]
    public class InfoController : Controller
    {
        // GET: Info
        public void Index()
        {
            Response.Write("App code using: ");
            Response.Write(System.Security.Principal.WindowsIdentity.GetCurrent().Name + "<br/>");
            Response.Write("Is user auth: ");
            Response.Write(User.Identity.IsAuthenticated.ToString() + "<br/>");
            Response.Write("Auth type: ");
            Response.Write(User.Identity.AuthenticationType + "<br/>");
            Response.Write("User name: ");
            Response.Write(User.Identity.Name + "<br/>");
        }
    }
}