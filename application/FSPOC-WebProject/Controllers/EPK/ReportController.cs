using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.EPK
{
    public class ReportController : Controller
    {
        // GET: Report
        [PersonaAuthorize(AppId = 26)]
        public ActionResult Index()
        {
            return RedirectToRoute("EPK", new { controller = "Report", action = "Full" });
        }

        [PersonaAuthorize(AppId = 26)]
        public ActionResult Full()
        {
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Kompletní přehled";
            return View("/Views/App/26/Page/24.cshtml", GetData());
        }

        [PersonaAuthorize(AppId = 26)]
        public ActionResult Simple()
        {
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Zjednodušený přehled";
            return View("/Views/App/26/Page/26.cshtml", GetData());
        }

        [PersonaAuthorize(AppId = 26)]
        public ActionResult Accounting()
        {
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Podklad pro zúčtování";
            return View("/Views/App/26/Page/27.cshtml", GetData());
        }

        [PersonaAuthorize(AppId = 26)]
        public ActionResult Basic()
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("id") { Caption = "Id obj." });
            data.Columns.Add(new DataColumn("id_heat_order") { Caption = "Id hromadné obj." });
            data.Columns.Add(new DataColumn("periodical_name") { Caption = "Název periodika" });
            data.Columns.Add(new DataColumn("begin_purchase") { Caption = "Začátek objednávky" });
            data.Columns.Add(new DataColumn("item_count") { Caption = "Počet kusů" });
            data.Columns.Add(new DataColumn("name") { Caption = "Stav objednávky" });
            data.Columns.Add(new DataColumn("ship_to_address") { Caption = "Místo doručení" });

            ViewData["tableData_data-table"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Základní report";
            return View("/Views/App/26/Page/11.cshtml");
        }

        [PersonaAuthorize(AppId = 26)]
        public ActionResult Heap()
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("id") { Caption = "Id hromadné obj." });
            data.Columns.Add(new DataColumn("approver_name") { Caption = "Schvalovatel" });
            data.Columns.Add(new DataColumn("client_name") { Caption = "Objednal(a)" });
            data.Columns.Add(new DataColumn("date") { Caption = "Datum objednávkay" });
            data.Columns.Add(new DataColumn("name") { Caption = "Stav" });
            data.Columns.Add(new DataColumn("approver_date") { Caption = "Datum schválení / zamítnutí" });

            List<DBItem> rows = GetData("Orders_heap_overview");
            foreach (DBItem row in rows) { 
                data.Rows.Add(
                    row["id"],
                    row["approver_name"],
                    row["client_name"],
                    row["date"],
                    row["name"],
                    row["approver_date"]
                );
            }

            ViewData["tableData_data-table-with-actions2"] = data;
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Report hromadných objednávek";
            return View("/Views/App/26/Page/16.cshtml");
        }


        private List<DBItem> GetData(string viewName = "Orders_complete_overview")
        {
            Entitron e = HttpContext.GetCORE().Entitron;
            DBView view = e.GetDynamicView(viewName);

            return view.Select().ToList();
        }
    }
}