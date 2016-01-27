using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Entitron
{
    [PersonaAuthorize(Roles = "Admin", Module = "Entitron")]
    public class DbDesignerController : Controller
    {
        // GET: DbDesigner
        public ActionResult Index()
        {
            return View();
        }
    }
}