using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class OrdersController : Controller
    {
        public ActionResult Index()
        {
            DBView table = HttpContext.GetCORE().Entitron.GetDynamicView("Orders_complete_overview");

            var data = table.Select().ToList();
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled objednávek";

            return View("/Views/App/26/Page/24.cshtml", data);
        }
        public ActionResult Create()
        {
            HttpContext.SetApp(26);
            
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Zadání objednávky";

            //ViewData["dropdownData_dropdown-select26"] uic471
            //ViewData["dropdownData_dropdown-select27"] uic472
            //ViewData["dropdownData_dropdown-select28"] uic473
            //ViewData["dropdownData_dropdown-select29"] uic474

            return View("/Views/App/26/Page/9.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            return RedirectToRoute("EPK", new { controller = "Orders", action = "Index" });
        }
    }
}