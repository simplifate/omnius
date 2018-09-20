using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;

namespace FSS.Omnius.Controllers.Nexus
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Nexus")]
    public class WebDAVController : Controller
    {
        public ActionResult Index()
        {
            DBEntities e = COREobject.i.Context;
            ViewData["LdapServersCount"] = e.Ldaps.Count();
            ViewData["WebServicesCount"] = e.WSs.Count();
            ViewData["ExtDatabasesCount"] = e.ExtDBs.Count();
            ViewData["WebDavServersCount"] = e.WebDavServers.Count();
            ViewData["APICount"] = e.APIs.Count();
            ViewData["TCPSocketCount"] = e.TCPListeners.Count();
            ViewData["RabbitMQCount"] = e.RabbitMQs.Count();
            return View(e.WebDavServers);
        }
        public ActionResult Create()
        {
            return View("~/Views/Nexus/WebDAV/Form.cshtml");
        }
        [HttpPost]
        public ActionResult Save(WebDavServer model)
        {
            DBEntities e = COREobject.i.Context;
            if (!model.Id.Equals(null) && model.Id > 0)
            {
                WebDavServer row = e.WebDavServers.Single(m => m.Id == model.Id);
                row.Name = model.Name;
                row.UriBasePath = model.UriBasePath;
                row.AnonymousMode = model.AnonymousMode;
                row.AuthUsername = model.AuthUsername;
                if (model.AuthPassword.Length > 0)
                    row.AuthPassword = model.AuthPassword;
                e.SaveChanges();
            }
            else
            {
                e.WebDavServers.Add(model);
                e.SaveChanges();
            }
            return RedirectToRoute("Nexus", new { @action = "Index" });
        }
        public ActionResult Detail(int id)
        {
            DBEntities e = COREobject.i.Context;
            return View("~/Views/Nexus/WebDAV/Detail.cshtml", e.WebDavServers.Single(m => m.Id == id));
        }
        public ActionResult Edit(int id)
        {
            DBEntities e = COREobject.i.Context;
            return View("~/Views/Nexus/WebDAV/Form.cshtml", e.WebDavServers.Single(m => m.Id == id));
        }
        public ActionResult Delete(int id)
        {
            DBEntities e = COREobject.i.Context;
            WebDavServer row = e.WebDavServers.Single(m => m.Id == id);

            e.WebDavServers.Remove(row);
            e.SaveChanges();

            return RedirectToRoute("Nexus", new { @action = "Index" });
        }
    }
}
