using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    public class ViewPageController : Controller
    {
        // GET: ViewPage
        public ActionResult Index(int Id)
        {
            using (var context = new DBEntities())
            {
                var page = context.MozaicEditorPages.Find(Id);
                var app = page.ParentApp;
                ViewData["appName"] = app.DisplayName;
                ViewData["appIcon"] = app.Icon;
                ViewData["pageName"] = page.Name;

                var dataSource1 = new Dictionary<int, string>();
                dataSource1.Add(1, "National geographic");
                dataSource1.Add(2, "New york times");
                dataSource1.Add(3, "The guardian");

                var dataSource2 = new Dictionary<int, string>();
                dataSource2.Add(1, "Deník");
                dataSource2.Add(2, "Týdeník");
                dataSource2.Add(3, "Měsíčník");

                ViewData["dropdownData_uic294"] = dataSource1;
                ViewData["dropdownData_uic295"] = dataSource2;

                ViewData["Mode"] = "App";

                return View($"/Views/App/{app.Id}/Page/{Id}.cshtml");
            }
        }
    }
}
