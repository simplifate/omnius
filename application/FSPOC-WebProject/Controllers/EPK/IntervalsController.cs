using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using E = FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class IntervalsController : Controller
    {
        public ActionResult Index()
        {
            var intervals = HttpContext.GetCORE().Entitron.GetDynamicTable("Periodical_interval").Select().ToList();
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
            ViewData["pageName"] = "Přehled četností periodik";
            return View("/Views/App/26/Page/20.cshtml");
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic1048"))
            {
                return RedirectToRoute("EPK", new { controller = "Intervals", action = "Create" });
            }
            return RedirectToRoute("EPK", new { controller = "Intervals", action = "Index" });
        }
        public ActionResult Create()
        {
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření četnosti periodika";
            return View("/Views/App/26/Page/44.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic683"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic681"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic682"));

                    e.GetDynamicTable("Periodical_interval").Add(item);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic681"] = fc["uic681"];
                    ViewData["uic682"] = fc.AllKeys.Contains("uic682");

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Vytvoření četnosti periodika";
                    return View("/Views/App/26/Page/44.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Intervals", action = "Index" });
        }
        public ActionResult Update(int id)
        {
            E.DBItem item = HttpContext.GetCORE().Entitron.GetDynamicItem("Periodical_interval", id);

            ViewData["uic681"] = item["name"];
            ViewData["uic682"] = item["active"];

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Úprava četnosti periodika";
            return View("/Views/App/26/Page/44.cshtml");
        }
        [HttpPost]
        public ActionResult Update(int id, FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic683"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic681"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic682"));

                    e.GetDynamicTable("Periodical_interval").Update(item, id);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic681"] = fc["uic681"];
                    ViewData["uic682"] = fc["uic682"];

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Úprava četnosti periodika";
                    return View("/Views/App/26/Page/44.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Intervals", action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                e.GetDynamicTable("Periodical_interval").Remove(id);
                e.Application.SaveChanges();
                return RedirectToRoute("EPK", new { controller = "Intervals", action = "Index" });
            }
            catch (Exception)
            {
                return RedirectToRoute("EPK", new { controller = "Intervals", action = "Index" });
            }
        }
    }
}