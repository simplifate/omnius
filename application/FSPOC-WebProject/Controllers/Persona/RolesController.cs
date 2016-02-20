using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSPOC_WebProject.Controllers.Persona
{
    [PersonaAuthorize(Roles = "Admin", Module = "Persona")]
    public class RolesController : Controller
    {

        // GET: Roles
        public ActionResult App(int? Id)
        {
            using (var context = new DBEntities())
            {
                AjaxPersonaAppRoles model = new AjaxPersonaAppRoles();
                Application app = Id != null ? context.Applications.Find(Id) : context.Applications.First();
                model.AppName = app.DisplayName;

                foreach (var user in context.Users)
                {
                    model.Users.Add(new AjaxPersonaAppRoles_User { Id = user.Id, Name = user.DisplayName });
                }

                foreach (var role in context.Roles.Where(c => c.ADgroup.ApplicationId == app.Id))
                {
                    model.Roles.Add(new AjaxPersonaAppRoles_Role { Id = role.Id, Name = role.Name, MemberList = role.Users.Select(u => u.UserId).ToList() });
                }

                return View("App", model);
            }
        }
    }
}