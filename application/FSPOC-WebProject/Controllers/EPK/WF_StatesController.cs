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
    public class WF_StatesController : Controller
    {
        public ActionResult Index()
        {
            var states = HttpContext.GetCORE().Entitron.GetDynamicTable("WF_states").Select().ToList();
            DataTable data = new DataTable();
            data.Columns.Add("Id");
            data.Columns.Add("Jméno");
            data.Columns.Add("Aktivní");
            foreach (DBItem state in states)
            {
                data.Rows.Add(state["id"], state["name"], state["active"]);
            }
            ViewData["tableData_suppliers_datatable"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled příznaků objednávky";
            return View("/Views/App/26/Page/18.cshtml");
        }
    }
}