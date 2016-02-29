using FSS.Omnius.Modules.Tapestry.Actions.Mozaic;
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
    public class PeriodicalsController : Controller
    {
        public ActionResult Index()
        {
            E.Entitron e = HttpContext.GetCORE().Entitron;
            var periodicals = e.GetDynamicTable("Periodicals").Select().ToList();
            var suppliers = e.GetDynamicTable("Suppliers").Select().where(c => c.column("id").In(new HashSet<object>(periodicals.Select(i => i["id_supplier"])))).ToList();
            var types = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("id").In(new HashSet<object>(periodicals.Select(i => i["id_periodical_types"])))).ToList();
            var intervals = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("id").In(new HashSet<object>(periodicals.Select(i => i["id_periodical_interval"])))).ToList();
            var forms = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("id").In(new HashSet<object>(periodicals.Select(i => i["id_periodical_form"])))).ToList();

            DataTable data = new DataTable();
            data.Columns.Add("Id");
            data.Columns.Add("Dodavatel");
            data.Columns.Add("Typ");
            data.Columns.Add("Četnost");
            data.Columns.Add("Jméno");
            data.Columns.Add("Aktivní");
            data.Columns.Add("Orientační cena s DPH");
            data.Columns.Add("Orientační cena bez 20% DPH");
            data.Columns.Add("Poštovné");
            data.Columns.Add("Poznámka");
            data.Columns.Add("Forma");
            foreach (E.DBItem periodical in periodicals)
            {
                data.Rows.Add(
                    periodical["id"],
                    suppliers.Single(s => (int)s["id"] == (int)periodical["id_supplier"])["name"],
                    types.Single(s => (int)s["id"] == (int)periodical["id_periodical_types"])["name"],
                    intervals.Single(s => (int)s["id"] == (int)periodical["id_periodical_interval"])["name"],
                    periodical["name"],
                    periodical["active"],
                    periodical["tentatively_net_of_VAT10"],
                    periodical["tentatively_net_of_VAT20"],
                    periodical["post_net_of_VAT20"],
                    periodical["note"],
                    forms.Single(s => (int)s["id"] == (int)periodical["id_periodical_form"])["name"]
                );
            }
            ViewData["tableData_data-table-with-actions4"] = data;

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Přehled periodik";
            return View("/Views/App/26/Page/14.cshtml");
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic991"))
            {
                return RedirectToRoute("EPK", new { controller = "Periodicals", action = "Create" });
            }
            if(fc.AllKeys.Contains("uic990")) {
                return RedirectToRoute("EPK", new { controller = "Periodicals", action = "Export" });
            }
            return RedirectToRoute("EPK", new { controller = "Periodicals", action = "Index" });
        }
        public ActionResult Create()
        {
            E.Entitron e = HttpContext.GetCORE().Entitron;
            ViewData["dropdownData_supplier_dropdown"] = e.GetDynamicTable("Suppliers").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_count_dropdown"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_form_dropdown"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_type_dropdown"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Vytvoření periodika";
            return View("/Views/App/26/Page/40.cshtml");
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic605"))
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                try
                {
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "id_supplier", fc["uic598"]);
                    item.createProperty(-2, "id_periodical_types", fc["uic601"]);
                    item.createProperty(-3, "id_periodical_interval", fc["uic599"]);
                    item.createProperty(-4, "name", fc["uic596"]);
                    item.createProperty(-5, "active", fc.AllKeys.Contains("uic595"));
                    item.createProperty(-6, "tentatively_net_of_VAT10", fc["602"]);
                    item.createProperty(-7, "tentatively_net_of_VAT20", fc["603"]);
                    item.createProperty(-8, "post_net_of_VAT20", fc["604"]);
                    item.createProperty(-9, "note", fc["597"]);
                    item.createProperty(-10, "id_periodical_form", fc["600"]);

                    e.GetDynamicTable("Periodicals").Add(item);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic598"] = fc["uic598"];
                    ViewData["uic601"] = fc["uic601"];
                    ViewData["uic599"] = fc["uic599"];
                    ViewData["uic596"] = fc["uic596"];
                    ViewData["uic595"] = fc["uic595"];
                    ViewData["uic602"] = fc["uic602"];
                    ViewData["uic603"] = fc["uic603"];
                    ViewData["uic604"] = fc["uic604"];
                    ViewData["uic597"] = fc["uic597"];
                    ViewData["uic600"] = fc["uic600"];

                    ViewData["dropdownData_supplier_dropdown"] = e.GetDynamicTable("Suppliers").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_count_dropdown"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_form_dropdown"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_type_dropdown"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Vytvoření periodika";
                    return View("/Views/App/26/Page/40.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Periodicals", action = "Index" });
        }
        public ActionResult Update(int id)
        {
            E.Entitron e = HttpContext.GetCORE().Entitron;
            ViewData["dropdownData_supplier_dropdown"] = e.GetDynamicTable("Suppliers").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_count_dropdown"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_form_dropdown"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
            ViewData["dropdownData_type_dropdown"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

            E.DBItem item = e.GetDynamicItem("Suppliers", id);

            ViewData["uic598"] = item["id_supplier"];
            ViewData["uic601"] = item["id_periodical_types"];
            ViewData["uic599"] = item["id_periodical_interval"];
            ViewData["uic596"] = item["name"];
            ViewData["uic595"] = item["active"];
            ViewData["uic602"] = item["tentatively_net_of_VAT10"];
            ViewData["uic603"] = item["tentatively_net_of_VAT20"];
            ViewData["uic604"] = item["post_net_of_VAT20"];
            ViewData["uic597"] = item["note"];
            ViewData["uic600"] = item["id_periodical_form"];

            ViewData["appIcon"] = "fa-book";
            ViewData["appName"] = "Evidence periodik";
            ViewData["pageName"] = "Úprava periodika";
            return View("/Views/App/26/Page/40.cshtml");
        }
        [HttpPost]
        public ActionResult Update(int id, FormCollection fc)
        {
            if (fc.AllKeys.Contains("uic605"))
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                try
                {
                    E.DBItem item = new E.DBItem();
                    item.createProperty(-1, "id_supplier", fc["uic598"]);
                    item.createProperty(-2, "id_periodical_types", fc["uic601"]);
                    item.createProperty(-3, "id_periodical_interval", fc["uic599"]);
                    item.createProperty(-4, "name", fc["uic596"]);
                    item.createProperty(-5, "active", fc.AllKeys.Contains("uic595"));
                    item.createProperty(-6, "tentatively_net_of_VAT10", fc["602"]);
                    item.createProperty(-7, "tentatively_net_of_VAT20", fc["603"]);
                    item.createProperty(-8, "post_net_of_VAT20", fc["604"]);
                    item.createProperty(-9, "note", fc["597"]);
                    item.createProperty(-10, "id_periodical_form", fc["600"]);

                    e.GetDynamicTable("Periodicals").Update(item, id);
                    e.Application.SaveChanges();
                }
                catch (Exception)
                {
                    ViewData["uic598"] = fc["uic598"];
                    ViewData["uic601"] = fc["uic601"];
                    ViewData["uic599"] = fc["uic599"];
                    ViewData["uic596"] = fc["uic596"];
                    ViewData["uic595"] = fc["uic595"];
                    ViewData["uic602"] = fc["uic602"];
                    ViewData["uic603"] = fc["uic603"];
                    ViewData["uic604"] = fc["uic604"];
                    ViewData["uic597"] = fc["uic597"];
                    ViewData["uic600"] = fc["uic600"];

                    ViewData["dropdownData_supplier_dropdown"] = e.GetDynamicTable("Suppliers").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_count_dropdown"] = e.GetDynamicTable("Periodical_interval").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_form_dropdown"] = e.GetDynamicTable("Periodical_forms").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);
                    ViewData["dropdownData_type_dropdown"] = e.GetDynamicTable("Periodical_types").Select().where(c => c.column("active").Equal(true)).ToList().ToDictionary(s => (int)s["id"], s => (string)s["name"]);

                    ViewData["appIcon"] = "fa-book";
                    ViewData["appName"] = "Evidence periodik";
                    ViewData["pageName"] = "Úprava periodika";
                    return View("/Views/App/26/Page/40.cshtml");
                }
            }
            return RedirectToRoute("EPK", new { controller = "Periodicals", action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                E.Entitron e = HttpContext.GetCORE().Entitron;
                e.GetDynamicTable("Periodicals").Remove(id);
                e.Application.SaveChanges();
                return RedirectToRoute("EPK", new { controller = "Periodicals", action = "Index" });
            }
            catch (Exception)
            {
                return RedirectToRoute("EPK", new { controller = "Periodicals", action = "Index" });
            }
        }

        public void Export()
        {
            ExportToExcelAction export = new ExportToExcelAction();

            Dictionary<string, object> vars = new Dictionary<string, object>();
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core._form = new FormCollection();

            vars.Add("__CORE__", core);
            vars.Add("TableName", "Periodicals");

            Dictionary<string, string> foreignKeys = new Dictionary<string, string>();
            foreignKeys.Add("id_supplier", "Suppliers.name");
            foreignKeys.Add("id_periodical_types", "Periodical_types.name");
            foreignKeys.Add("id_periodical_interval", "Periodical_interval.name");
            foreignKeys.Add("id_periodical_form", "Periodical_forms.name");

            vars.Add("ForeignKeys", foreignKeys);

            Dictionary<string, object> dummy = new Dictionary<string, object>();

            export.InnerRun(vars, dummy, dummy);
        }
    }
}