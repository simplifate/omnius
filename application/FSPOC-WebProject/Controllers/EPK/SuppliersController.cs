using E = FSS.Omnius.Modules.Entitron;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Tapestry.Actions.Mozaic;
using System.Collections.Generic;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize]
    public class SuppliersController : Controller
    {
        public ActionResult Index()
        {
            HttpContext.SetApp(26);
            var suppliers = HttpContext.GetCORE().Entitron.GetDynamicTable("Suppliers").Select().ToList();
            DataTable data = new DataTable();
            data.Columns.Add("Id");
            data.Columns.Add("Jméno");
            data.Columns.Add("Adresa");
            data.Columns.Add("Poznámka");
            data.Columns.Add("Aktivní");
            foreach (E.DBItem supplier in suppliers)
            {
                data.Rows.Add(supplier["id"], supplier["name"], supplier["address"], supplier["note"], supplier["active"]);
            }
            ViewData["tableData_suppliers_datatable"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled dodavatelů";
            return View("/Views/App/26/Page/13.cshtml");
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            HttpContext.SetApp(26);
            if (fc.AllKeys.Contains("uic1020"))
            {
                return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Create" });
            }
            if (fc.AllKeys.Contains("uic1019")) {
                return RedirectToRoute("EPK", new { Controller = "Suppliers", action = "Export" });
            }
            return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Index" });
        }
        public ActionResult Create()
        {
            HttpContext.SetApp(26);
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření dodavatele";
            return View("/Views/App/26/Page/41.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            HttpContext.SetApp(26);
            if (fc.AllKeys.Contains("uic961"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic958"]);
                    item.createProperty(-2, "address", fc["uic959"]);
                    item.createProperty(-3, "note", fc["uic960"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic957"));

                    e.GetDynamicTable("Suppliers").Add(item);
                    e.Application.SaveChanges();
                }
                catch (Exception ox)
                {
                    ViewData["uic958"] = fc["uic958"];
                    ViewData["uic959"] = fc["uic959"];
                    ViewData["uic960"] = fc["uic960"];
                    ViewData["uic957"] = fc["uic957"];

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Vytvoření dodavatele";
                    return View("/Views/App/26/Page/41.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Index" });
        }
        public ActionResult Update(int id)
        {
            HttpContext.SetApp(26);
            E.DBItem item = HttpContext.GetCORE().Entitron.GetDynamicItem("Suppliers", id);

            ViewData["uic958"] = item["name"];
            ViewData["uic959"] = item["address"];
            ViewData["uic960"] = item["note"];
            ViewData["uic957"] = item["active"];

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Úprava dodavatele";
            return View("/Views/App/26/Page/41.cshtml");
        }
        [HttpPost]
        public ActionResult Update(int id, FormCollection fc)
        {
            HttpContext.SetApp(26);
            if (fc.AllKeys.Contains("uic961"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic958"]);
                    item.createProperty(-2, "address", fc["uic959"]);
                    item.createProperty(-3, "note", fc["uic960"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic957"));

                    e.GetDynamicTable("Suppliers").Update(item, id);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic958"] = fc["uic958"];
                    ViewData["uic959"] = fc["uic959"];
                    ViewData["uic960"] = fc["uic960"];
                    ViewData["uic957"] = fc["uic957"];

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Úprava dodavatele";
                    return View("/Views/App/26/Page/41.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            HttpContext.SetApp(26);
            try
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                e.GetDynamicTable("Suppliers").Remove(id);
                e.Application.SaveChanges();
                return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Index" });
            }
            catch(Exception)
            {
                return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Index" });
            }
        }

        public void Export()
        {
            HttpContext.SetApp(26);
            ExportToExcelAction export = new ExportToExcelAction();

            Dictionary<string, object> vars = new Dictionary<string, object>();
            Modules.CORE.CORE core = HttpContext.GetCORE();

            vars.Add("__CORE__", core);
            vars.Add("TableName", "Suppliers");

            Dictionary<string, object> dummy = new Dictionary<string, object>();

            export.InnerRun(vars, dummy, dummy, new Modules.CORE.Message());
        }
    }
}