using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    [PersonaAuthorize(Roles = "Admin")]
    public class CssController : Controller
    {
        // GET: Css
        public ActionResult Index()
        {
            return View();
        }

        
    }
}