using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    public class ViewPageController : Controller
    {
        // GET: ViewPage
        public ActionResult Index(int Id)
        {
            using (var context = new DBEntities())
            {
                var page = context.MozaicEditorPages.Find(Id);
                var app = page.ParentApp;
                ViewData["appName"] = app.DisplayName;
                ViewData["appIcon"] = app.Icon;
                ViewData["pageName"] = page.Name;

                var zakladniReport = new DataTable();
                zakladniReport.Columns.Add("Název periodika");
                zakladniReport.Columns.Add("Počátek objednávky");
                zakladniReport.Columns.Add("Počet kusů");
                zakladniReport.Columns.Add("Místo doručení");
                zakladniReport.Rows.Add("Euro 24", "3.1.2011", 1, "Josef.Dvorak@rwe.cz");
                zakladniReport.Rows.Add("21. století", "29.12.2010", 1, "Ústí nad Labem - Klišská, Klišská 940, Ústí nad Labem 40117");
                ViewData["tableData_tabulkaZakladniReport"] = zakladniReport;

                var prehledDodavatelu = new DataTable();
                prehledDodavatelu.Columns.Add("Jméno");
                prehledDodavatelu.Columns.Add("Adresa");
                prehledDodavatelu.Columns.Add("Poznámka");
                prehledDodavatelu.Rows.Add("CZ Press", "", "");
                prehledDodavatelu.Rows.Add("E. S. Best", "", "");
                prehledDodavatelu.Rows.Add("Economia", "", "");
                ViewData["tableData_tabulkaPrehledDodavatelu"] = prehledDodavatelu;

                var prehledPeriodik = new DataTable();
                prehledPeriodik.Columns.Add("Id");
                prehledPeriodik.Columns.Add("Název periodika");
                prehledPeriodik.Columns.Add("Četnost vydání");
                prehledPeriodik.Columns.Add("Typ periodika");
                prehledPeriodik.Columns.Add("Dodavatel");
                prehledPeriodik.Columns.Add("Cena bez DPH");
                ViewData["tableData_tabulkaPrehledPeriodik"] = prehledPeriodik;

                var prehledObjednavek = new DataTable();
                prehledObjednavek.Columns.Add("Id obj.");
                prehledObjednavek.Columns.Add("Id hromadné obj.");
                prehledObjednavek.Columns.Add("Počátek");
                prehledObjednavek.Columns.Add("Společnost odběratele");
                prehledObjednavek.Columns.Add("Název periodika");
                prehledObjednavek.Columns.Add("Kusů");
                prehledObjednavek.Columns.Add("Odběratele");
                prehledObjednavek.Columns.Add("Dodací adresa");
                ViewData["tableData_tabulkaPrehledObjednavek"] = prehledObjednavek;

                ViewData["Mode"] = "App";

                return View($"/Views/App/{app.Id}/Page/{Id}.cshtml");
            }
        }
    }
}
