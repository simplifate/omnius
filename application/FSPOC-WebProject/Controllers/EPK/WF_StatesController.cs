using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using E = FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class WF_StatesController : Controller
    {
        public ActionResult Index()
        {
            var intervals = HttpContext.GetCORE().Entitron.GetDynamicTable("WF_states").Select().ToList();
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
            ViewData["pageName"] = "Přehled stavů";
            return View("/Views/App/26/Page/18.cshtml");
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic1061"))
            {
                return RedirectToRoute("EPK", new { controller = "WF_States", action = "Create" });
            }
            return RedirectToRoute("EPK", new { controller = "WF_States", action = "Index" });
        }
        public ActionResult Create()
        {
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření stavu";
            return View("/Views/App/26/Page/54.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic1034"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic1031"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic1032"));

                    e.GetDynamicTable("WF_states").Add(item);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic1031"] = fc["uic1031"];
                    ViewData["uic1032"] = fc["uic1032"];

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Vytvoření stavu";
                    return View("/Views/App/26/Page/54.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "WF_States", action = "Index" });
        }
        public ActionResult Update(int id)
        {
            E.DBItem item = HttpContext.GetCORE().Entitron.GetDynamicItem("WF_states", id);

            ViewData["uic1038"] = item["name"];
            ViewData["uic1039"] = item["active"];

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Úprava stavu";
            return View("/Views/App/26/Page/55.cshtml");
        }
        [HttpPost]
        public ActionResult Update(int id, FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic1041"))
            {
                try
                {
                    E.Entitron e = HttpContext.GetCORE().Entitron;
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "name", fc["uic1038"]);
                    item.createProperty(-4, "active", fc.AllKeys.Contains("uic1039"));

                    e.GetDynamicTable("WF_states").Update(item, id);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic1038"] = fc["uic1038"];
                    ViewData["uic1039"] = fc["uic1039"];

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Úprava stavu";
                    return View("/Views/App/26/Page/55.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "WF_States", action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                e.GetDynamicTable("WF_states").Remove(id);
                e.Application.SaveChanges();
                return RedirectToRoute("EPK", new { controller = "WF_States", action = "Index" });
            }
            catch (Exception)
            {
                return RedirectToRoute("EPK", new { controller = "WF_States", action = "Index" });
            }
        }
    }
}