using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.EPK
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
            foreach (DBItem supplier in suppliers)
            {
                data.Rows.Add(supplier["id"], supplier["name"], supplier["address"], supplier["note"], supplier["active"]);
            }
            ViewData["tableData_suppliers_datatable"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled dodavatelů";
            return View("/Views/App/26/Page/13.cshtml");
        }
        public ActionResult Create()
        {
            return View("/Views/App/26/Page/41.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            try
            {
                Entitron e = HttpContext.GetCORE().Entitron;
                DBItem item = new DBItem();
                item["name"] = fc["uic554"];
                item["address"] = fc["uic555"];
                item["note"] = fc["uic556"];
                item["active"] = fc["uic553"];

                e.GetDynamicTable("Suppliers").Add(item);
                e.Application.SaveChanges();
            }
            catch (Exception)
            {
                ViewData["uic554"] = fc["uic554"];
                ViewData["uic555"] = fc["uic555"];
                ViewData["uic556"] = fc["uic556"];
                ViewData["uic553"] = fc["uic553"];
                return View("/Views/App/26/Page/41.cshtml");
            }

            return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Index" });
        }
        public ActionResult Update(int id)
        {
            DBItem item = HttpContext.GetCORE().Entitron.GetDynamicItem("Suppliers", id);

            ViewData["uic554"] = item["name"];
            ViewData["uic555"] = item["address"];
            ViewData["uic556"] = item["note"];
            ViewData["uic553"] = item["active"];

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Úprava dodavatele";
            return View("/Views/App/26/Page/41.cshtml");
        }
        [HttpPost]
        public ActionResult Update(int id, FormCollection fc)
        {
            try
            {
                Entitron e = HttpContext.GetCORE().Entitron;
                DBItem item = new DBItem();
                item["name"] = fc["uic554"];
                item["address"] = fc["uic555"];
                item["note"] = fc["uic556"];
                item["active"] = fc["uic553"];

                e.GetDynamicTable("Suppliers").Update(item, id);
                e.Application.SaveChanges();
            }
            catch(Exception)
            {
                ViewData["uic554"] = fc["uic554"];
                ViewData["uic555"] = fc["uic555"];
                ViewData["uic556"] = fc["uic556"];
                ViewData["uic553"] = fc["uic553"];
                return View("/Views/App/26/Page/41.cshtml");
            }

            return RedirectToRoute("EPK", new { controller = "Suppliers", action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Entitron e = HttpContext.GetCORE().Entitron;
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