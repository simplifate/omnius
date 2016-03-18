using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Cortex
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Cortex")]
    public class CortexController : Controller
    {
        public Dictionary<ScheduleType, string> scheduleTypeNames = new Dictionary<ScheduleType, string>();
        public Dictionary<Days, string> daysNames = new Dictionary<Days, string>();
        public Dictionary<Months, string> monthsNames = new Dictionary<Months, string>();
        public Dictionary<DaysInMonth, string> daysInMonthNames = new Dictionary<DaysInMonth, string>();
        public Dictionary<InModifiers, string> modifiersNames = new Dictionary<InModifiers, string>();

        public CortexController()
        {
            MapScheduleTypeNames();
            MapDayNames();
            MapMonthNames();
            MapModifierNames();
            MapDaysInMonthNames();
        }

        public ActionResult Index()
        {
            DBEntities context = new DBEntities();
            return View("~/Views/Cortex/Index.cshtml", context.Tasks);
        }

        public ActionResult Create()
        {
            DBEntities e = new DBEntities();
             
            ViewData["ApplicationList"] = e.Applications;
            ViewData["ScheduleTypeNames"] = scheduleTypeNames;
            ViewData["DaysNames"] = daysNames;
            ViewData["MonthsNames"] = monthsNames;
            ViewData["ModifierNames"] = modifiersNames;
            ViewData["DaysInMonthNames"] = daysInMonthNames;

            return View("~/Views/Cortex/Form.cshtml");
        }

        private void MapScheduleTypeNames()
        { 
            scheduleTypeNames.Add(ScheduleType.MINUTE, "Minutově");
            scheduleTypeNames.Add(ScheduleType.HOURLY, "Hodinově");
            scheduleTypeNames.Add(ScheduleType.DAILY, "Denně");
            scheduleTypeNames.Add(ScheduleType.WEEKLY, "Týdně");
            scheduleTypeNames.Add(ScheduleType.MONTHLY, "Měsíčně");
            scheduleTypeNames.Add(ScheduleType.ONCE, "Jednou");
            scheduleTypeNames.Add(ScheduleType.ONIDLE, "Při nečinnosti");
            scheduleTypeNames.Add(ScheduleType.ONSTART, "Při startu");
        }

        private void MapDayNames()
        {
            daysNames.Add(Days.MON, "Pondělí");
            daysNames.Add(Days.TUE, "Úterý");
            daysNames.Add(Days.WED, "Středa");
            daysNames.Add(Days.THU, "Čtvrtek");
            daysNames.Add(Days.FRI, "Pátek");
            daysNames.Add(Days.SAT, "Sobota");
            daysNames.Add(Days.SUN, "Neděle");
        }

        private void MapMonthNames()
        {
            monthsNames.Add(Months.JAN, "Leden");
            monthsNames.Add(Months.FEB, "Únor");
            monthsNames.Add(Months.MAR, "Březen");
            monthsNames.Add(Months.APR, "Duben");
            monthsNames.Add(Months.MAY, "Květen");
            monthsNames.Add(Months.JUNE, "Červen");
            monthsNames.Add(Months.JULY, "Červenec");
            monthsNames.Add(Months.AUG, "Srpen");
            monthsNames.Add(Months.SEPT, "Září");
            monthsNames.Add(Months.OCT, "Říjen");
            monthsNames.Add(Months.NOV, "Listopad");
            monthsNames.Add(Months.DEC, "Prosinec");
        }

        private void MapModifierNames()
        {
            modifiersNames.Add(InModifiers.FIRST, "1.");
            modifiersNames.Add(InModifiers.SECOND, "2.");
            modifiersNames.Add(InModifiers.THIRD, "3.");
            modifiersNames.Add(InModifiers.FOURTH, "4.");
            modifiersNames.Add(InModifiers.LAST, "Poslední");
        }

        private void MapDaysInMonthNames()
        {
            for(int i = 1; i <= 31; i++) {
                daysInMonthNames.Add((DaysInMonth)i, i.ToString());
            }
            daysInMonthNames.Add(DaysInMonth.LAST, "Poslední");
        }
    }
}
