using E = FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class SuppliersController : Controller
    {
        public ActionResult Index()
        {
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
            if (fc.AllKeys.Contains("uic1020"))
            {
                return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Create" });
            }
            return View("/Views/App/26/Page/13.cshtml");
        }
        public ActionResult Create()
        {
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření dodavatele";
            return View("/Views/App/26/Page/41.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
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
    }
}