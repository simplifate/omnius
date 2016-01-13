using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSPOC_WebProject.Controllers.Persona
{
    public class ModulesController : Controller
    {
        [PersonaAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            using (var context = new DBEntities())
            {
                List<AjaxModuleAccessPermission> model = context.ModuleAccessPermissions
                    .Select(c => new AjaxModuleAccessPermission
                    {
                        Id = c.Id,
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
    }
}
