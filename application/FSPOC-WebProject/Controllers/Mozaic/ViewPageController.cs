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
                prehledDodavatelu.Rows.Add("Euronews", "", "");
                prehledDodavatelu.Rows.Add("GAS", "", "");
                prehledDodavatelu.Rows.Add("Mafra", "", "");
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
                prehledObjednavek.Rows.Add("1746", "", "20.01.2016", "RWE IT Czech s.r.o.", "Business Spotlight", "1", "Čopfová Kateřina", "Praha - Prosek Point, Prosecká 855/68, Praha 9 19000");
                prehledObjednavek.Rows.Add("1745", "", "20.01.2016", "RWE IT Czech s.r.o.", "Business Spotlight", "1", "Čopfová Kateřina", "Praha - Prosek Point, Prosecká 855/68, Praha 9 19000");
                prehledObjednavek.Rows.Add("1744", "", "20.01.2016", "RWE IT Czech s.r.o.", "Business Spotlight", "1", "Čopfová Kateřina", "Praha - Prosek Point, Prosecká 855/68, Praha 9 19000");
                prehledObjednavek.Rows.Add("1743", "", "17.12.2015", "RWE IT Czech s.r.o.", "Právní rádce", "1", "Březinová Dagmar, Mgr.", "Praha - Limuzská, Limuzská 3135/12, Praha 10 10098");
                prehledObjednavek.Rows.Add("1742", "", "10.12.2015", "RWE IT Czech s.r.o.", "Hospodářské noviny - flash prohlížečka na webu", "1", "Dočekal Pavel, Ing.", "pavel.docekal@rwe.cz");
                ViewData["tableData_tabulkaPrehledObjednavek"] = prehledObjednavek;

                var prehledHromadnychObjednavek = new DataTable();
                prehledHromadnychObjednavek.Columns.Add("Id hromadné obj.");
                prehledHromadnychObjednavek.Columns.Add("Schvalovatel");
                prehledHromadnychObjednavek.Columns.Add("Objednal(a)");
                prehledHromadnychObjednavek.Columns.Add("Datum objednávky");
                prehledHromadnychObjednavek.Columns.Add("Stav");
                prehledHromadnychObjednavek.Columns.Add("Datum schválení");
                ViewData["tableData_tabulkaPrehledHromadnychObjednavek"] = prehledHromadnychObjednavek;

                var prehledPriznakuObjednavky = new DataTable();
                prehledPriznakuObjednavky.Columns.Add("Název");
                prehledPriznakuObjednavky.Columns.Add("Popis");
                prehledPriznakuObjednavky.Columns.Add("Aktivní");
                ViewData["tableData_tabulkaPrehledPriznakuObjednavky"] = prehledPriznakuObjednavky;

                var prehledForemPeriodik = new DataTable();
                prehledForemPeriodik.Columns.Add("Název");
                prehledForemPeriodik.Columns.Add("Popis");
                prehledForemPeriodik.Columns.Add("Aktivní");
                ViewData["tableData_tabulkaPrehledForemPeriodik"] = prehledForemPeriodik;

                var prehledPriznakuSchvalovani = new DataTable();
                prehledPriznakuSchvalovani.Columns.Add("Název");
                prehledPriznakuSchvalovani.Columns.Add("Popis");
                prehledPriznakuSchvalovani.Columns.Add("Aktivní");
                ViewData["tableData_tabulkaPrehledPriznakuSchvalovani"] = prehledPriznakuSchvalovani;

                var prehledCetnostiPeriodik = new DataTable();
                prehledCetnostiPeriodik.Columns.Add("Název");
                prehledCetnostiPeriodik.Columns.Add("Popis");
                prehledCetnostiPeriodik.Columns.Add("Aktivní");
                ViewData["tableData_tabulkaPrehledCetnostiPeriodik"] = prehledCetnostiPeriodik;

                var prehledTypuPeriodik = new DataTable();
                prehledTypuPeriodik.Columns.Add("Název");
                prehledTypuPeriodik.Columns.Add("Popis");
                prehledTypuPeriodik.Columns.Add("Aktivní");
                ViewData["tableData_tabulkaPrehledTypuPeriodik"] = prehledTypuPeriodik;

                var hromadnaZmena = new DataTable();
                hromadnaZmena.Columns.Add("Id obj.");
                hromadnaZmena.Columns.Add("Id hromadné obj.");
                hromadnaZmena.Columns.Add("Počátek");
                hromadnaZmena.Columns.Add("Společnost odběratele");
                hromadnaZmena.Columns.Add("Název periodika");
                hromadnaZmena.Columns.Add("Kusů");
                hromadnaZmena.Columns.Add("Odběratele");
                hromadnaZmena.Columns.Add("Dodací adresa");
                ViewData["tableData_tabulkaHromadnaZmena"] = hromadnaZmena;

                var zastupci = new DataTable();
                zastupci.Columns.Add("ID");
                zastupci.Columns.Add("Uživatel");
                zastupci.Columns.Add("Zástupce uživatele");
                zastupci.Columns.Add("Platnost od");
                zastupci.Columns.Add("Platnost do");
                ViewData["tableData_tabulkaZastupci"] = zastupci;

                var kompletniPrehled = new DataTable();
                kompletniPrehled.Columns.Add("Id obj.");
                kompletniPrehled.Columns.Add("Id hromadné obj.");
                kompletniPrehled.Columns.Add("Společnost odběratele");
                kompletniPrehled.Columns.Add("Odběratel");
                kompletniPrehled.Columns.Add("Místo doručení");
                kompletniPrehled.Columns.Add("Název periodika");
                kompletniPrehled.Columns.Add("Dodavatel");
                kompletniPrehled.Columns.Add("Počet kusů");
                kompletniPrehled.Columns.Add("Nákl. středisko");
                ViewData["tableData_tabulkaKompletniPrehled"] = kompletniPrehled;

                var zjednodusenyPrehled = new DataTable();
                zjednodusenyPrehled.Columns.Add("Id obj.");
                zjednodusenyPrehled.Columns.Add("Id hromadné obj.");
                zjednodusenyPrehled.Columns.Add("Společnost odběratele");
                zjednodusenyPrehled.Columns.Add("Odběratel");
                zjednodusenyPrehled.Columns.Add("Dodací adresa");
                zjednodusenyPrehled.Columns.Add("Název periodika");
                zjednodusenyPrehled.Columns.Add("Počet kusů");
                ViewData["tableData_tabulkaZjednodusenyPrehled"] = zjednodusenyPrehled;

                var podkladProZuctovani = new DataTable();
                podkladProZuctovani.Columns.Add("Společnost odběratele");
                podkladProZuctovani.Columns.Add("Nákl. středisko");
                podkladProZuctovani.Columns.Add("Odběratel");
                podkladProZuctovani.Columns.Add("Název periodika");
                podkladProZuctovani.Columns.Add("Počet kusů");
                podkladProZuctovani.Columns.Add("Dodavatel");
                podkladProZuctovani.Columns.Add("Orientační cena s DPH");
                podkladProZuctovani.Columns.Add("Poštovné");
                ViewData["tableData_tabulkaPodkladProZuctovani"] = podkladProZuctovani;

                var objednavkyKnihovny = new DataTable();
                objednavkyKnihovny.Columns.Add("Datum objednání");
                objednavkyKnihovny.Columns.Add("Objednávka pro");
                objednavkyKnihovny.Columns.Add("Název periodika");
                objednavkyKnihovny.Columns.Add("Počátek objednávky");
                objednavkyKnihovny.Columns.Add("Počet kusů");
                objednavkyKnihovny.Columns.Add("Místo doručení");
                ViewData["tableData_tabulkaObjednavkyKnihovny"] = objednavkyKnihovny;

                var reportProAsistentky = new DataTable();
                reportProAsistentky.Columns.Add("Id obj.");
                reportProAsistentky.Columns.Add("Id hromadné obj.");
                reportProAsistentky.Columns.Add("Objednávka pro");
                reportProAsistentky.Columns.Add("Název periodika");
                reportProAsistentky.Columns.Add("Datum obědnání");
                reportProAsistentky.Columns.Add("Počátek objednávky");
                reportProAsistentky.Columns.Add("Počet kusů");
                reportProAsistentky.Columns.Add("Místo doručení");
                ViewData["tableData_tabulkaReportProAsistentky"] = reportProAsistentky;

                var zjednodusenyPrehledProVedouciPracovniky = new DataTable();
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Id obj.");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Id hromadné obj.");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Objednávka pro");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Odběratel");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Název periodika");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Počátek objednávky");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Počet kusů");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Stav objednávky");
                zjednodusenyPrehledProVedouciPracovniky.Columns.Add("Místo doručení");
                ViewData["tableData_tabulkaZjednodusenyPrehledProVedouciPracovniky"] = zjednodusenyPrehledProVedouciPracovniky;



                ViewData["Mode"] = "App";

                return View($"/Views/App/{app.Id}/Page/{Id}.cshtml");
            }
        }
    }
}
