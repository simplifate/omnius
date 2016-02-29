using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E = FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class OrdersController : Controller
    {
        public ActionResult Index()
        {
            E.Entitron e = HttpContext.GetCORE().Entitron;
            var orders = e.GetDynamicTable("Orders").Select().ToList();
            var periodicals = e.GetDynamicTable("Periodicals").Select().where(c => c.column("id").In(new HashSet<object>(orders.Select(i => i["id_periodical"])))).ToList();

            DataTable data = new DataTable();
            data.Columns.Add("Id");
            data.Columns.Add("Id hromadné objednávky");
            data.Columns.Add("Počátek");
            data.Columns.Add("Společnost odběratele");
            data.Columns.Add("Název periodika");
            data.Columns.Add("Kusů");
            data.Columns.Add("Odběratel");
            data.Columns.Add("Dodací adresa");
            foreach (E.DBItem order in orders)
            {
                data.Rows.Add(
                    order["id"],
                    order["id_heap_order"],
                    order["begin_purchase"],
                    order["client_comany_name"],
                    periodicals.Single(p => (int)p["id"] == (int)order["id_periodical"])["name"],
                    order["item_count"],
                    order["client_name"],
                    order["ship_to_address"]
                );
            }
            ViewData["tableData_data-table-with-actions2"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled objednávek";

            return View("/Views/App/26/Page/15.cshtml");
        }
        public ActionResult Create()
        {
            string username = HttpContext.GetLoggedUser().UserName.ToUpper();
            E.Entitron e = HttpContext.GetCORE().Entitron;
            ViewData["dropdownData_dropdown-select26"] = e.GetDynamicTable("Periodicals").Select().ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_dropdown-select27"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_dropdown-select28"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_dropdown-select29"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

            DBTable users = e.GetDynamicTable("Users");
            DBItem user = users.Select().where(c => c.column("ad_id").Equal(username)).ToList().FirstOrDefault();
            DBItem masterUser = user;
            while (masterUser != null && !string.IsNullOrWhiteSpace(masterUser["h_pernr"].ToString()))
            {
                masterUser = users.Select().where(c => c.column("pernr").Equal(masterUser["h_pernr"])).ToList().FirstOrDefault();
            }
            DBItem approverUser = users.Select().where(c => c.column("pernr").Equal(user["h_pernr"])).First();

            ViewData["uic451"] = user != null ? $"{user["nachn"]} {user["vorna"]}" : "";
            ViewData["uic450"] = masterUser != null ? e.GetDynamicTable("Org").Select().where(c => c.column("orgeh").Equal(masterUser["h_orgeh"])).First()["stext"] : "";
            ViewData["uic460"] = user != null ? user["kostl"] : "";
            ViewData["uic464"] = approverUser != null ? $"{approverUser["nachn"]} {approverUser["vorna"]}" : "";

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření objednávky";
            return View("/Views/App/26/Page/9.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic470"))
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                DBTable users = e.GetDynamicTable("Users");
                E.DBItem user = users.Select().where(c => c.column("ad_id").Equal(HttpContext.GetLoggedUser().UserName.ToUpper())).ToList().FirstOrDefault();
                E.DBItem periodical = e.GetDynamicItem("Periodicals", Convert.ToInt32(fc["uic471"]));

                DBItem masterUser = user;
                while(masterUser != null && masterUser["h_orgeh"] != null)
                {
                    masterUser = users.Select().where(c => c.column("orgeh").Equal(masterUser["h_orgeh"])).First();
                }
                DBItem approverUser = users.Select().where(c => c.column("orgeh").Equal(user["h_orgeh"])).First();

                try
                {
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "id_wf_state", 1);
                    item.createProperty(-3, "id_periodical", fc["uic471"]);
                    item.createProperty(-4, "purchase_year", 2017);
                    item.createProperty(-5, "begin_purchase", fc["uic462"]);
                    item.createProperty(-6, "end_purchase", fc["uic463"]);
                    item.createProperty(-7, "item_count", fc["uic467"]);
                    item.createProperty(-8, "client_name", user != null ? $"{user["nachn"]} {user["vorna"]}" : "");
                    item.createProperty(-9, "client_sapid2", user != null ? user["sapid2"] : "");
                    item.createProperty(-11, "client_function", user != null ? e.GetDynamicTable("Plans").Select().where(c => c.column("orgeh").Equal(user["orgeh"])).First()["stext"] : "");
                    item.createProperty(-12, "client_cost_centre", user != null ? user["kostl"] : "");
                    item.createProperty(-13, "client_company_name", masterUser != null ? e.GetDynamicTable("Org").Select().where(c => c.column("orgeh").Equal(masterUser["orgeh"])).First()["stext"] : "");
                    item.createProperty(-14, "approver_name", approverUser != null ? $"{approverUser["nachn"]} {approverUser["vorna"]}" : "");
                    item.createProperty(-15, "approver_sapid2", approverUser != null ? approverUser["sapid2"] : "");
                    item.createProperty(-16, "approver_ouid", "");
                    item.createProperty(-18, "ship_to_address", fc["uic465"]);
                    item.createProperty(-19, "tentatively_net_of_VAT10", Convert.ToInt32(periodical["tentatively_net_of_VAT10"]) * Convert.ToInt32(fc["uic467"]));
                    item.createProperty(-20, "tentatively_net_of_VAT20", Convert.ToInt32(periodical["tentatively_net_of_VAT20"]) * Convert.ToInt32(fc["uic467"]));
                    item.createProperty(-21, "post_net_of_VAT20", Convert.ToInt32(periodical["post_net_of_VAT20"]) * Convert.ToInt32(fc["uic467"]));
                    item.createProperty(-22, "note", fc["uic468"]);
                    item.createProperty(-23, "active", true);
                    item.createProperty(-24, "date_purchase", DateTime.Now);
                    item.createProperty(-25, "other_purchase", "");

                    e.GetDynamicTable("Orders").Add(item);
                    e.Application.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewData["uic471"] = fc["uic471"];
                    ViewData["uic462"] = fc["uic462"];
                    ViewData["uic463"] = fc["uic463"];
                    ViewData["uic467"] = fc["uic467"];
                    ViewData["uic465"] = fc["uic465"];
                    ViewData["uic468"] = fc["uic468"];
                    
                    ViewData["dropdownData_dropdown-select26"] = e.GetDynamicTable("Periodicals").Select().ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_dropdown-select27"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_dropdown-select28"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_dropdown-select29"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Vytvoření objednávky";
                    return View("/Views/App/26/Page/9.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Orders", action = "Index" });
        }
        //public ActionResult Update(int id)
        //{
        //    E.Entitron e = HttpContext.GetCORE().Entitron;
        //    ViewData["dropdownData_supplier_dropdown"] = e.GetDynamicTable("Suppliers").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
        //    ViewData["dropdownData_count_dropdown"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
        //    ViewData["dropdownData_form_dropdown"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
        //    ViewData["dropdownData_type_dropdown"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

        //    E.DBItem item = e.GetDynamicItem("Suppliers", id);

        //    ViewData["uic598"] = item["id_supplier"];
        //    ViewData["uic601"] = item["id_periodical_types"];
        //    ViewData["uic599"] = item["id_periodical_interval"];
        //    ViewData["uic596"] = item["name"];
        //    ViewData["uic595"] = item["active"];
        //    ViewData["uic602"] = item["tentatively_net_of_VAT10"];
        //    ViewData["uic603"] = item["tentatively_net_of_VAT20"];
        //    ViewData["uic604"] = item["post_net_of_VAT20"];
        //    ViewData["uic597"] = item["note"];
        //    ViewData["uic600"] = item["id_periodical_form"];

        //    ViewData["appIcon"] = "fa-book";
        //    ViewData["appName"] = "Evidence periodik";
        //    ViewData["pageName"] = "Úprava periodika";
        //    return View("/Views/App/26/Page/40.cshtml");
        //}
        //[HttpPost]
        //public ActionResult Update(int id, FormCollection fc)
        //{
        //    if (fc.AllKeys.Contains("uic605"))
        //    {
        //        E.Entitron e = HttpContext.GetCORE().Entitron;
        //        try
        //        {
        //            E.DBItem item = new E.DBItem();
        //            item.createProperty(-1, "id_supplier", fc["uic598"]);
        //            item.createProperty(-2, "id_periodical_types", fc["uic601"]);
        //            item.createProperty(-3, "id_periodical_interval", fc["uic599"]);
        //            item.createProperty(-4, "name", fc["uic596"]);
        //            item.createProperty(-5, "active", fc.AllKeys.Contains("uic595"));
        //            item.createProperty(-6, "tentatively_net_of_VAT10", fc["602"]);
        //            item.createProperty(-7, "tentatively_net_of_VAT20", fc["603"]);
        //            item.createProperty(-8, "post_net_of_VAT20", fc["604"]);
        //            item.createProperty(-9, "note", fc["597"]);
        //            item.createProperty(-10, "id_periodical_form", fc["600"]);

        //            e.GetDynamicTable("Orders").Update(item, id);
        //            e.Application.SaveChanges();
        //        }
        //        catch (Exception)
        //        {
        //            ViewData["uic598"] = fc["uic598"];
        //            ViewData["uic601"] = fc["uic601"];
        //            ViewData["uic599"] = fc["uic599"];
        //            ViewData["uic596"] = fc["uic596"];
        //            ViewData["uic595"] = fc["uic595"];
        //            ViewData["uic602"] = fc["uic602"];
        //            ViewData["uic603"] = fc["uic603"];
        //            ViewData["uic604"] = fc["uic604"];
        //            ViewData["uic597"] = fc["uic597"];
        //            ViewData["uic600"] = fc["uic600"];

        //            ViewData["dropdownData_supplier_dropdown"] = e.GetDynamicTable("Suppliers").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
        //            ViewData["dropdownData_count_dropdown"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
        //            ViewData["dropdownData_form_dropdown"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
        //            ViewData["dropdownData_type_dropdown"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

        //            ViewData["appIcon"] = "fa-book";
        //            ViewData["appName"] = "Evidence periodik";
        //            ViewData["pageName"] = "Úprava periodika";
        //            return View("/Views/App/26/Page/40.cshtml");
        //        }
        //    }
        //    return RedirectToRoute("EPK", new { controller = "Orders", action = "Index" });
        //}

        public ActionResult Delete(int id)
        {
            try
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                e.GetDynamicTable("Orders").Remove(id);
                e.Application.SaveChanges();
                return RedirectToRoute("EPK", new { controller = "Orders", action = "Index" });
            }
            catch (Exception)
            {
                return RedirectToRoute("EPK", new { controller = "Orders", action = "Index" });
            }
        }
    }
}