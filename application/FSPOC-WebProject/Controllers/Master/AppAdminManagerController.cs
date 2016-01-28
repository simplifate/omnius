using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Master
{
    [PersonaAuthorize(Roles = "Admin", Module = "Master")]
    public class AppAdminManagerController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new DBEntities())
            {
                var appList = new List<Application>();

                foreach (var app in context.Applications)
                {
                    appList.Add(app);
                }
                ViewData["Apps"] = appList;
                return View();
            }
        }
    }
}