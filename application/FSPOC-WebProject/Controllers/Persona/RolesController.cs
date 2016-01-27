using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

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
                var model = new AjaxPersonaAppRoles();
                var app = Id != null ? context.Applications.Find(Id) : context.Applications.First();
                model.AppName = app.DisplayName;
                foreach (var user in context.Users)
                {
                    model.Users.Add(new AjaxPersonaAppRoles_User { Id = user.Id, Name = user.DisplayName });
                }
                foreach (var role in context.PersonaAppRoles.Where(c => c.Application.Id == app.Id))
                {
                    var memberList = new List<int>();
                    int parsedId;
                    foreach (string token in role.MembersList.Split(','))
                    {
                        if(int.TryParse(token, out parsedId))
                            memberList.Add(parsedId);
                    }
                    model.Roles.Add(new AjaxPersonaAppRoles_Role { Id = role.Id, Name = role.RoleName, MemberList = memberList });
                }
                return View(model);
            }
        }
    }
}
