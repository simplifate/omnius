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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult App(AjaxPersonaAppRoles model)
        {
            int i = 0;
            i++;

            return View(model);
        }
        
        /*#region Updates
        // Start update environment
        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            PersonaAppRole role = e.Roles.SingleOrDefault(x => x.Id == id);

            

            return SetUpdateCss(css);
        }

        public ActionResult SetUpdateCss(PersonaAppRole css)
        {
            ViewBag.CssValue = css.Value;
            ViewBag.CssName = css.Name;

            return View("Update", css);
        }

        // Save updates
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Css model)
        {
            DBEntities e = new DBEntities();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return View("Update", model);
            }

            e.Css.AddOrUpdate(model);
            e.SaveChanges();

            return View("BaseIndex", e.Css);
        }
        #endregion

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            Css css = e.Css.SingleOrDefault(x => x.Id == id);

            e.Css.Remove(css);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        //Create is like Update - but it is not taking CSS file from DB, it creates new

        public ActionResult Create()
        {
            Css newCss = new Css();
            newCss.Name = "New CSS";
            newCss.Value = "";

            return SetUpdateCss(newCss);
        }*/
    }
}
