using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using E = FSS.Omnius.Modules.Entitron;

namespace FSPOC_WebProject.Controllers.EPK
{
    [PersonaAuthorize]
    public class FormsController : Controller
    {
        public ActionResult Index()
        {
            HttpContext.SetApp(26);
            var intervals = HttpContext.GetCORE().Entitron.GetDynamicTable("Periodical_forms").Select().ToList();
            DataTable data = new DataTable();
            data.Columns.Add("Id");
            data.Columns.Add("Jméno");
            data.Columns.Add("Aktivní");
            foreach (E.DBItem interval in intervals)
            {
                data.Rows.Add(interval["id"], interval["name"], interval["active"]);
            }
            ViewData["tableData_data-table-with-actions2"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled forem";
            return View("/Views/App/26/Page/19.cshtml");
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            HttpContext.SetApp(26);
            if (fc.AllKeys.Contains("uic1057"))
            {
                return RedirectToRoute("EPK", new { controller = "Forms", action = "Create" });
            }
            return RedirectToRoute("EPK", new { controller = "Forms", action = "Index" });
        }
        public ActionResult Create()
        {
            HttpContext.SetApp(26);
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření formy";
            return View("/Views/App/26/Page/49.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            HttpContext.SetApp(26);
            if (fc.AllKeys.Contains("uic951"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic948"]);
                    item.createProperty(-2, "short_name", fc["uic948"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic949"));

                    e.GetDynamicTable("Periodical_forms").Add(item);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic948"] = fc["uic948"];
                    ViewData["uic949"] = fc.AllKeys.Contains("uic949");

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Vytvoření formy";
                    return View("/Views/App/26/Page/49.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Forms", action = "Index" });
        }
        public ActionResult Update(int id)
        {
            HttpContext.SetApp(26);
            E.DBItem item = HttpContext.GetCORE().Entitron.GetDynamicItem("Periodical_forms", id);

            ViewData["uic977"] = item["name"];
            ViewData["uic978"] = item["active"];

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Úprava formy";
            return View("/Views/App/26/Page/51.cshtml");
        }
        [HttpPost]
        public ActionResult Update(int id, FormCollection fc)
        {
            HttpContext.SetApp(26);
            if (fc.AllKeys.Contains("uic980"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic977"]);
                    item.createProperty(-2, "short_name", fc["uic977"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic978"));

                    e.GetDynamicTable("Periodical_forms").Update(item, id);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic977"] = fc["uic977"];
                    ViewData["uic978"] = fc["uic978"];

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Úprava formy";
                    return View("/Views/App/26/Page/51.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Forms", action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            HttpContext.SetApp(26);
            try
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                e.GetDynamicTable("Periodical_forms").Remove(id);
                e.Application.SaveChanges();
                return RedirectToRoute("EPK", new { controller = "Forms", action = "Index" });
            }
            catch (Exception)
            {
                return RedirectToRoute("EPK", new { controller = "Forms", action = "Index" });
            }
        }
    }
}