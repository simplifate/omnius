using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    [PersonaAuthorize]
    public class ViewPageController : Controller
    {
        public ActionResult Index(int Id, FormCollection fc)
        {
            using (var context = new DBEntities())
            {
                CORE core = HttpContext.GetCORE();
                core.Entitron.AppName = "EvidencePeriodik";

                ViewData["appName"] = "Evidence Periodik";
                ViewData["appIcon"] = "fa-book";

                DBTable companies = null, heapOrders = null, orders = null, intervals = null, types = null, periodical = null, suppliers = null, states = null;
                orders = core.Entitron.GetDynamicTable("Orders");
                periodical = core.Entitron.GetDynamicTable("Periodicals");
                intervals = core.Entitron.GetDynamicTable("Periodical_interval");
                if (Id == -1)
                    companies = core.Entitron.GetDynamicTable("Companies");
                if (Id == 16)
                    heapOrders = core.Entitron.GetDynamicTable("Heap_orders");
                if (Id == 19 || Id == 14)
                    types = core.Entitron.GetDynamicTable("Periodical_types");
                if (Id == 13 || Id == 14)
                    suppliers = core.Entitron.GetDynamicTable("Suppliers");
                if (Id == 18)
                    states = core.Entitron.GetDynamicTable("WF_states");

                // create
                if (fc != null && fc.Count > 0)
                {
                    try
                    {
                        DBItem perio = periodical.GetById(Convert.ToInt32(fc["uic471"]));
                        int count = Convert.ToInt32(fc["uic467"]);

                        DBItem newRow = new DBItem();
                        newRow.table = orders;
                        newRow.createProperty(-1, "client_company_name", fc["uic450"]);
                        newRow.createProperty(-2, "client_name", fc["uic451"]);
                        newRow.createProperty(-3, "client_cost_centre", fc["uic460"]);
                        newRow.createProperty(-4, "id_periodical", fc["uic471"]);
                        //newRow.createProperty(-1, "", fc["uic472"]); // četnost
                        //newRow.createProperty(-1, "", fc["uic473"]); // forma
                        //newRow.createProperty(-1, "", fc["uic474"]); // typ
                        newRow.createProperty(-5, "begin_purchase", fc["uic462"]);
                        newRow.createProperty(-6, "end_purchase", fc["uic463"]);
                        newRow.createProperty(-7, "item_count", count);
                        newRow.createProperty(-8, "ship_to_address", fc["uic465"]);
                        newRow.createProperty(-9, "date_purchase", DateTime.Now);
                        newRow.createProperty(-11, "purchase_year", DateTime.Now.Year);
                        newRow.createProperty(-12, "other_purchase", "");
                        newRow.createProperty(-10, "id_wf_state", 1);
                        newRow.createProperty(-13, "active", true);
                        newRow.createProperty(-14, "note", fc["uic468"]);
                        newRow.createProperty(-15, "tentatively_net_of_VAT10", Convert.ToInt32(perio["tentatively_net_of_VAT10"]) * count);
                        newRow.createProperty(-16, "tentatively_net_of_VAT20", Convert.ToInt32(perio["tentatively_net_of_VAT20"]) * count);
                        newRow.createProperty(-17, "post_net_of_VAT20", Convert.ToInt32(perio["post_net_of_VAT20"]) * count);
                        newRow.createProperty(-18, "client_sapid2", "");
                        newRow.createProperty(-19, "client_function", "");
                        newRow.createProperty(-20, "approver_name", "");
                        newRow.createProperty(-21, "approver_sapid2", "");
                        newRow.createProperty(-22, "approver_ouid", "");
                        newRow.createProperty(-23, "approver_date", DateTime.Now);

                        orders.Add(newRow);
                        core.Entitron.Application.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ViewData["Message"] = ex.Message;
                    }
                }

                // Reporty -> základní report
                if (Id == 11)
                {
                    var zakladniReport = new DataTable();
                    zakladniReport.Columns.Add("Název periodika");
                    zakladniReport.Columns.Add("Počátek objednávky");
                    zakladniReport.Columns.Add("Počet kusů");
                    zakladniReport.Columns.Add("Místo doručení");
                    foreach (DBItem row in orders.Select().ToList())
                    {
                        zakladniReport.Rows.Add(
                            periodical.GetById((int)row["id_periodical"])["name"],
                            row["begin_purchase"],
                            row["item_count"],
                            row["ship_to_address"]);
                    }
                    ViewData["tableData_tabulkaZakladniReport"] = zakladniReport;
                }

                // Administrace -> Přehled dodavatelů
                if (Id == 13)
                {
                    var prehledDodavatelu = new DataTable();
                    prehledDodavatelu.Columns.Add("Jméno");
                    prehledDodavatelu.Columns.Add("Adresa");
                    prehledDodavatelu.Columns.Add("Poznámka");
                    foreach (DBItem row in suppliers.Select().ToList())
                    {
                        prehledDodavatelu.Rows.Add(
                            row["name"],
                            row["address"],
                            row["note"]);
                    }
                    ViewData["tableData_tabulkaPrehledDodavatelu"] = prehledDodavatelu;
                }

                // Administrace -> přehled periodik
                if (Id == 14)
                {
                    var prehledPeriodik = new DataTable();
                    prehledPeriodik.Columns.Add("Id");
                    prehledPeriodik.Columns.Add("Název periodika");
                    prehledPeriodik.Columns.Add("Četnost vydání");
                    prehledPeriodik.Columns.Add("Typ periodika");
                    prehledPeriodik.Columns.Add("Dodavatel");
                    prehledPeriodik.Columns.Add("Cena bez DPH");
                    foreach (DBItem row in periodical.Select().ToList())
                    {
                        prehledPeriodik.Rows.Add(
                            row["Id"],
                            row["name"],
                            intervals.GetById((int)row["id_periodical_interval"])["name"],
                            types.GetById((int)row["id_periodical_types"])["name"],
                            suppliers.GetById((int)row["id_supplier"])["name"],
                            row["tentatively_net_of_VAT10"]);
                    }
                    ViewData["tableData_tabulkaPrehledPeriodik"] = prehledPeriodik;
                }

                // Administrace -> přehled objednávek
                if (Id == 15)
                {
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
                    foreach (DBItem item in orders.Select().ToList())
                    {
                        prehledObjednavek.Rows.Add(
                            item["Id"], // Id obj.
                            item["id_heap_order"], // Id hromadné obj.
                            item["begin_purchase"], // Počátek
                            item["client_company_name"], // Společnost odběratele
                            periodical.GetById((int)item["id_periodical"])["name"], // Název periodika
                            item["item_count"], // Kusů
                            item["client_name"], // Odběratele
                            item["ship_to_address"]); // Dodací adresa
                    }
                    ViewData["tableData_tabulkaPrehledObjednavek"] = prehledObjednavek;
                }

                // Administrace -> přehled hromadných objednávek
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
                prehledPriznakuObjednavky.Rows.Add("nový", "nový", "Ano");
                prehledPriznakuObjednavky.Rows.Add("rozpracováno", "rozpracováno", "Ano");
                prehledPriznakuObjednavky.Rows.Add("vyřízeno", "vyřízeno", "Ano");
                prehledPriznakuObjednavky.Rows.Add("zrušeno", "zrušeno", "Ano");
                prehledPriznakuObjednavky.Rows.Add("nevyfakturováno", "nevyfakturováno", "Ano");
                prehledPriznakuObjednavky.Rows.Add("změna", "změna", "Ano");

                ViewData["tableData_tabulkaPrehledPriznakuObjednavky"] = prehledPriznakuObjednavky;

                var prehledForemPeriodik = new DataTable();
                prehledForemPeriodik.Columns.Add("Název");
                prehledForemPeriodik.Columns.Add("Popis");
                prehledForemPeriodik.Columns.Add("Aktivní");
                prehledForemPeriodik.Rows.Add("elektronické", "elektronické", "Ano");
                prehledForemPeriodik.Rows.Add("papírové", "papírové", "Ano");
                ViewData["tableData_tabulkaPrehledForemPeriodik"] = prehledForemPeriodik;

                var prehledPriznakuSchvalovani = new DataTable();
                prehledPriznakuSchvalovani.Columns.Add("Název");
                prehledPriznakuSchvalovani.Columns.Add("Popis");
                prehledPriznakuSchvalovani.Columns.Add("Aktivní");
                prehledPriznakuSchvalovani.Rows.Add("připraveno ke schválení", "připraveno ke schválení", "Ano");
                prehledPriznakuSchvalovani.Rows.Add("schváleno", "schváleno", "Ano");
                prehledPriznakuSchvalovani.Rows.Add("zamítnuto", "zamítnuto", "Ano");
                ViewData["tableData_tabulkaPrehledPriznakuSchvalovani"] = prehledPriznakuSchvalovani;

                var prehledCetnostiPeriodik = new DataTable();
                prehledCetnostiPeriodik.Columns.Add("Název");
                prehledCetnostiPeriodik.Columns.Add("Popis");
                prehledCetnostiPeriodik.Columns.Add("Aktivní");
                prehledCetnostiPeriodik.Rows.Add("1x týdně", "1x týdně", "Ano");
                prehledCetnostiPeriodik.Rows.Add("5x týdně", "5x týdně", "Ano");
                prehledCetnostiPeriodik.Rows.Add("10x ročně", "10x ročně", "Ano");
                prehledCetnostiPeriodik.Rows.Add("1x měsíčně", "1x měsíčně", "Ano");
                prehledCetnostiPeriodik.Rows.Add("6x ročně", "6x ročně", "Ano");
                ViewData["tableData_tabulkaPrehledCetnostiPeriodik"] = prehledCetnostiPeriodik;

                var prehledTypuPeriodik = new DataTable();
                prehledTypuPeriodik.Columns.Add("Název");
                prehledTypuPeriodik.Columns.Add("Popis");
                prehledTypuPeriodik.Columns.Add("Aktivní");
                prehledTypuPeriodik.Rows.Add("tuzemské", "tuzemské", "Ano");
                prehledTypuPeriodik.Rows.Add("zahraniční", "zahraniční", "Ano");
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

                var typeses = new Dictionary<int, string>();
                typeses.Add(1, "tuzemské");
                typeses.Add(2, "zahraniční");
                ViewData["dropdownData_dropdown-select29"] = typeses;

                var forms = new Dictionary<int, string>();
                forms.Add(1, "elektronické");
                forms.Add(2, "papírové");
                ViewData["dropdownData_dropdown-select28"] = forms;

                var freq = new Dictionary<int, string>();
                foreach(DBItem item in intervals.Select().ToList())
                {
                    freq.Add((int)item["id"], (string)item["name"]);
                }
                ViewData["dropdownData_dropdown-select27"] = freq;

                var mag = new Dictionary<int, string>();
                foreach (DBItem item in periodical.Select().ToList())
                {
                    mag.Add((int)item["id"], (string)item["name"]);
                }
                ViewData["dropdownData_dropdown-select26"] = mag;

                return View($"~/Views/Mozaic/EPK/{Id}.cshtml");
            }
        }
    }
}
