using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;

namespace FSS.Omnius.Controllers.Master
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
        public ActionResult BuildApp(int Id)
        {
            using (var context = new DBEntities())
            {
                var app = context.Applications.Find(Id);
                foreach(var editorPage in app.MozaicEditorPages)
                {
                    editorPage.Recompile();
                    string requestedPath = $"/Views/App/{Id}/Page/{editorPage.Id}.cshtml";
                    var oldPage = context.Pages.Where(c => c.ViewPath == requestedPath).FirstOrDefault();
                    if (oldPage == null)
                    {
                        var newPage = new Page
                        {
                            ViewName = editorPage.Name,
                            ViewPath = $"/Views/App/{Id}/Page/{editorPage.Id}.cshtml",
                            ViewContent = editorPage.CompiledPartialView
                        };
                        context.Pages.Add(newPage);
                    }
                    else
                    {
                        oldPage.ViewName = editorPage.Name;
                        oldPage.ViewContent = editorPage.CompiledPartialView;
                    }
                }
                app.IsPublished = true;
                context.SaveChanges();
                return View();
            }
        }
    }
}
