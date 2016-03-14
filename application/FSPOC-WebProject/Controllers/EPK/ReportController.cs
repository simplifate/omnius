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
    [PersonaAuthorize]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            HttpContext.SetApp(26);
            return RedirectToRoute("EPK", new { controller = "Report", action = "Full" });
        }
        
        public ActionResult Full()
        {
            HttpContext.SetApp(26);
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Kompletní přehled";
            return View("/Views/App/26/Page/24.cshtml", GetData());
        }
        
        public ActionResult Simple()
        {
            HttpContext.SetApp(26);
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Zjednodušený přehled";
            return View("/Views/App/26/Page/26.cshtml", GetData());
        }
        
        public ActionResult Accounting()
        {
            HttpContext.SetApp(26);
            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Podklad pro zúčtování";
            return View("/Views/App/26/Page/27.cshtml", GetData());
        }
        
        public ActionResult Basic()
        {
            HttpContext.SetApp(26);
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
        
        public ActionResult Library()
        {
            HttpContext.SetApp(26);
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("id") { Caption = "Id" });
            data.Columns.Add(new DataColumn("date_purchase") { Caption = "Datum objednání" });
            data.Columns.Add(new DataColumn("client_name") { Caption = "Objednávka pro" });
            data.Columns.Add(new DataColumn("periodical_name") { Caption = "Název periodika" });
            data.Columns.Add(new DataColumn("begin_purchase") { Caption = "Počátek objednávky" });
            data.Columns.Add(new DataColumn("item_count") { Caption = "Počet kusů" });
            data.Columns.Add(new DataColumn("ship_to_address") { Caption = "Místo doručení" });

            Entitron e = HttpContext.GetCORE().Entitron;
            DBView view = e.GetDynamicView("Orders_complete_overview");
            foreach(DBItem row in view.Select().where(v => v.column("other_purchase").Equal("knihovna")).ToList()) {
                data.Rows.Add(
                    row["id"],
                    row["date_purchase"],
                    row["client_name"],
                    row["periodical_name"],
                    row["begin_purchase"],
                    row["item_count"],
                    row["ship_to_address"]
                );
            }

            ViewData["tableData_data-table"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Objednávky knihovny";
            return View("/Views/App/26/Page/28.cshtml");
        }
        
        public ActionResult Assistant()
        {
            HttpContext.SetApp(26);
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("id") { Caption = "Id" });
            data.Columns.Add(new DataColumn("id_heap_order") { Caption = "Id hromadné obj." });
            data.Columns.Add(new DataColumn("client_name") { Caption = "Objednávka pro" });
            data.Columns.Add(new DataColumn("periodical_name") { Caption = "Název periodika" });
            data.Columns.Add(new DataColumn("date_purchase") { Caption = "Datum objednání" });
            data.Columns.Add(new DataColumn("begin_purchase") { Caption = "Počátek objednávky" });
            data.Columns.Add(new DataColumn("item_count") { Caption = "Počet kusů" });
            data.Columns.Add(new DataColumn("ship_to_address") { Caption = "Místo doručení" });

            Entitron e = HttpContext.GetCORE().Entitron;
            DBView view = e.GetDynamicView("Orders_complete_overview");
            foreach (DBItem row in view.Select().where(v => v.column("other_purchase").Equal("sapid2")).ToList()) {
                data.Rows.Add(
                    row["id"],
                    row["id_heap_order"],
                    row["client_name"],
                    row["periodical_name"],
                    row["date_purchase"],
                    row["begin_purchase"],
                    row["item_count"],
                    row["ship_to_address"]
                );
            }

            ViewData["tableData_data-table"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Report pro asistentky";
            return View("/Views/App/26/Page/29.cshtml");
        }
        
        public ActionResult Heap()
        {
            HttpContext.SetApp(26);
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