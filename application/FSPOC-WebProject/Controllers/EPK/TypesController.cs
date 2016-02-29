using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using E = FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class TypesController : Controller
    {
        public ActionResult Index()
        {
            var intervals = HttpContext.GetCORE().Entitron.GetDynamicTable("Periodical_types").Select().ToList();
            DataTable data = new DataTable();
            data.Columns.Add("Id");
            data.Columns.Add("Jméno");
            data.Columns.Add("Aktivní");
            foreach (E.DBItem interval in intervals)
            {
                data.Rows.Add(interval["id"], interval["name"], interval["active"]);
            }
            ViewData["tableData_data-table-with-actions3"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled typů";
            return View("/Views/App/26/Page/21.cshtml");
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic1052"))
            {
                return RedirectToRoute("EPK", new { controller = "Types", action = "Create" });
            }
            return RedirectToRoute("EPK", new { controller = "Types", action = "Index" });
        }
        public ActionResult Create()
        {
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření typu";
            return View("/Views/App/26/Page/52.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic987"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic984"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic985"));

                    e.GetDynamicTable("Periodical_types").Add(item);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic984"] = fc["uic984"];
                    ViewData["uic985"] = fc.AllKeys.Contains("uic985");

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Vytvoření typu";
                    return View("/Views/App/26/Page/52.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Types", action = "Index" });
        }
        public ActionResult Update(int id)
        {
            E.DBItem item = HttpContext.GetCORE().Entitron.GetDynamicItem("Periodical_types", id);

            ViewData["uic1012"] = item["name"];
            ViewData["uic1013"] = item["active"];

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Úprava formy";
            return View("/Views/App/26/Page/53.cshtml");
        }
        [HttpPost]
        public ActionResult Update(int id, FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic1015"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic1012"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic1013"));

                    e.GetDynamicTable("Periodical_types").Update(item, id);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic1012"] = fc["uic1012"];
                    ViewData["uic1013"] = fc["uic1013"];

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Úprava formy";
                    return View("/Views/App/26/Page/53.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Types", action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                e.GetDynamicTable("Periodical_types").Remove(id);
                e.Application.SaveChanges();
                return RedirectToRoute("EPK", new { controller = "Types", action = "Index" });
            }
            catch (Exception)
            {
                return RedirectToRoute("EPK", new { controller = "Types", action = "Index" });
            }
        }
    }
}