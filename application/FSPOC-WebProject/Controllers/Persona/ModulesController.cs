using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;

namespace FSPOC_WebProject.Controllers.Persona
{
    public class ModulesController : Controller
    {
        [PersonaAuthorize(NeedsAdmin = true, Module = "Persona")]
        public ActionResult Index()
        {
            using (var context = DBEntities.instance)
            {
                List<AjaxModuleAccessPermission> model = context.ModuleAccessPermissions
                    .Select(c => new AjaxModuleAccessPermission
                    {
                        UserId = c.UserId,
                        UserName = c.User.DisplayName,
                        Core = c.Core,
                        Master = c.Master,
                        Tapestry = c.Tapestry,
                        Entitron = c.Entitron,
                        Mozaic = c.Mozaic,
                        Persona = c.Persona,
                        Nexus = c.Nexus,
                        Sentry = c.Sentry,
                        Hermes = c.Hermes,
                        Athena = c.Athena,
                        Watchtower = c.Watchtower,
                        Cortex = c.Cortex
                    }).ToList();

                return View(model);
            }
        }

        //getUser JSON (for dataTable)
        public JsonResult loadData()
        {
            DBEntities e = ControllerContext.HttpContext.GetCORE().Entitron.GetStaticTables();
            //var data = e.Users.OrderBy(a => a.UserName).ToList();
            var data = e.ModuleAccessPermissions
                    .Select(c => new AjaxModuleAccessPermission
                    {
                        UserId = c.UserId,
                        UserName = c.User.DisplayName,
                        Core = c.Core,
                        Master = c.Master,
                        Tapestry = c.Tapestry,
                        Entitron = c.Entitron,
                        Mozaic = c.Mozaic,
                        Persona = c.Persona,
                        Nexus = c.Nexus,
                        Sentry = c.Sentry,
                        Hermes = c.Hermes,
                        Athena = c.Athena,
                        Watchtower = c.Watchtower,
                        Cortex = c.Cortex
                    }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);

        }
    }
}
