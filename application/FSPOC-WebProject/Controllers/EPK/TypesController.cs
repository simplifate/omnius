using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class TypesController : Controller
    {
        public ActionResult Index()
        {
            var suppliers = HttpContext.GetCORE().Entitron.GetDynamicTable("Periodical_types").Select().ToList();
            DataTable data = new DataTable();
            data.Columns.Add("Id");
            data.Columns.Add("Jméno");
            data.Columns.Add("Aktivní");
            foreach (DBItem supplier in suppliers)
            {
                data.Rows.Add(supplier["id"], supplier["name"], supplier["active"]);
            }
            ViewData["tableData_suppliers_datatable"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled typů periodik";
            return View("/Views/App/26/Page/21.cshtml");
        }
        public ActionResult Create()
        {
            return View("/Views/App/26/Page/.cshtml");
        }
    }
}