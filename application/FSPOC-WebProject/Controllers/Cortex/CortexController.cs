using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using FSS.Omnius.Modules.Cortex;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Cortex
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Cortex")]
    public class CortexController : Controller
    {
        public Dictionary<ScheduleType, string> scheduleTypeNames = new Dictionary<ScheduleType, string>();
        public Dictionary<Days, string> daysNames = new Dictionary<Days, string>();
        public Dictionary<Days, string> daysShortNames = new Dictionary<Days, string>();
        public Dictionary<Months, string> monthsNames = new Dictionary<Months, string>();
        public Dictionary<Months, string> monthsShortNames = new Dictionary<Months, string>();
        public Dictionary<DaysInMonth, string> daysInMonthNames = new Dictionary<DaysInMonth, string>();
        public Dictionary<InModifiers, string> modifiersNames = new Dictionary<InModifiers, string>();
        public Dictionary<RepetitionMinutes, string> repetitionMinutesNames = new Dictionary<RepetitionMinutes, string>();

        private List<string> sequences = new List<string>();
        private List<string> sequence = new List<string>();
        
        public CortexController()
        {
            MapScheduleTypeNames();
            MapDayNames();
            MapDayShortNames();
            MapMonthNames();
            MapMonthShortNames();
            MapModifierNames();
            MapDaysInMonthNames();
            MapRepetitionMinutesNames();
        }

        public ActionResult Index()
        {
            DBEntities context = new DBEntities();

            ViewBag.ScheduleTypeNames = scheduleTypeNames;
            ViewBag.DaysNames = daysNames;
            ViewBag.MonthsNames = monthsNames;
            ViewBag.ModifierNames = modifiersNames;
            ViewBag.DaysInMonthNames = daysInMonthNames;
            
            return View("~/Views/Cortex/Index.cshtml", context.Tasks);
        }

        public ActionResult Create()
        {
            DBEntities e = new DBEntities();
             
            ViewData["ApplicationList"] = e.Applications;

            return View("~/Views/Cortex/Form.cshtml");
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();

            ViewData["ApplicationList"] = e.Applications;

            return View("~/Views/Cortex/Form.cshtml", e.Tasks.Single(t => t.Id == id));
        }

        public ActionResult Detail(int? id)
        {
            DBEntities e = new DBEntities();

            ViewData["ApplicationList"] = e.Applications;
            ViewData["ScheduleTypeNames"] = scheduleTypeNames;
            ViewData["DaysNames"] = daysNames;
            ViewData["MonthsNames"] = monthsNames;
            ViewData["ModifierNames"] = modifiersNames;
            ViewData["DaysInMonthNames"] = daysInMonthNames;

            return View("~/Views/Cortex/Detail.cshtml", e.Tasks.Single(t => t.Id == id));
        }

        public ActionResult Test()
        {
            Modules.Cortex.Cortex cortex = new Modules.Cortex.Cortex(Request);
            ViewData["Data"] = cortex.List();
            
            return View("~/Views/Cortex/Test.cshtml");
        }

        public ActionResult Save(Task model)
        {
            Modules.Cortex.Cortex cortex = new Modules.Cortex.Cortex(Request);
            cortex.Save(model);

            return RedirectToRoute("Cortex", new { @action = "Index" });
        }

        public ActionResult Delete(int id)
        {
            Modules.Cortex.Cortex cortex = new Modules.Cortex.Cortex();
            cortex.Delete(id);

            return RedirectToRoute("Cortex", new { @action = "Index" });
        }

        #region tools

        private void MapScheduleTypeNames()
        {
            scheduleTypeNames.Add(ScheduleType.DAILY, "Denně");
            scheduleTypeNames.Add(ScheduleType.WEEKLY, "Týdně");
            scheduleTypeNames.Add(ScheduleType.MONTHLY, "Měsíčně");
            scheduleTypeNames.Add(ScheduleType.ONCE, "Jednou");
            scheduleTypeNames.Add(ScheduleType.ONIDLE, "Při nečinnosti");
        }

        private void MapDayNames()
        {
            daysNames.Add(Days.Monday, "Pondělí");
            daysNames.Add(Days.Tuesday, "Úterý");
            daysNames.Add(Days.Wednesday, "Středa");
            daysNames.Add(Days.Thursday, "Čtvrtek");
            daysNames.Add(Days.Friday, "Pátek");
            daysNames.Add(Days.Saturday, "Sobota");
            daysNames.Add(Days.Sunday, "Neděle");
        }

        private void MapDayShortNames()
        {
            daysShortNames.Add(Days.Monday, "Po");
            daysShortNames.Add(Days.Tuesday, "Út");
            daysShortNames.Add(Days.Wednesday, "St");
            daysShortNames.Add(Days.Thursday, "Čt");
            daysShortNames.Add(Days.Friday, "Pá");
            daysShortNames.Add(Days.Saturday, "So");
            daysShortNames.Add(Days.Sunday, "Ne");
        }

        private void MapMonthNames()
        {
            monthsNames.Add(Months.January, "Leden");
            monthsNames.Add(Months.February, "Únor");
            monthsNames.Add(Months.March, "Březen");
            monthsNames.Add(Months.April, "Duben");
            monthsNames.Add(Months.May, "Květen");
            monthsNames.Add(Months.June, "Červen");
            monthsNames.Add(Months.July, "Červenec");
            monthsNames.Add(Months.August, "Srpen");
            monthsNames.Add(Months.September, "Září");
            monthsNames.Add(Months.October, "Říjen");
            monthsNames.Add(Months.November, "Listopad");
            monthsNames.Add(Months.December, "Prosinec");
        }

        private void MapMonthShortNames()
        {
            monthsShortNames.Add(Months.January, "1.");
            monthsShortNames.Add(Months.February, "2.");
            monthsShortNames.Add(Months.March, "3.");
            monthsShortNames.Add(Months.April, "4.");
            monthsShortNames.Add(Months.May, "5.");
            monthsShortNames.Add(Months.June, "6.");
            monthsShortNames.Add(Months.July, "7.");
            monthsShortNames.Add(Months.August, "8.");
            monthsShortNames.Add(Months.September, "9.");
            monthsShortNames.Add(Months.October, "10.");
            monthsShortNames.Add(Months.November, "11.");
            monthsShortNames.Add(Months.December, "12.");
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
            int i = 0;
            foreach (DaysInMonth day in Enums<DaysInMonth>()) {
                if (day == DaysInMonth.Last) daysInMonthNames.Add(day, "Poslední");
                else daysInMonthNames.Add(day, (++i).ToString());
            }
        }

        private void MapRepetitionMinutesNames()
        {
            foreach(RepetitionMinutes m in Enums<RepetitionMinutes>()) {
                repetitionMinutesNames.Add(m, m.ToString().Replace("m", ""));
            }
        }

        public string ViewWeekDays(Days set)
        {
            List<string> days = new List<string>();
            foreach(Days d in Enums<Days>()) {
                if (set.HasFlag(d)) days.Add(daysNames[d]);
            }
            return string.Join(", ", days);
        }

        public string ViewMonths(Task t)
        {
            List<string> months = new List<string>();
            if(t.Monthly_Months != null) {
                foreach(Months m in Enums<Months>()) {
                    if (((Months)t.Monthly_Months).HasFlag(m)) months.Add(monthsNames[m]);
                }
            }
            return string.Join(", ", months);
        }

        public string ViewInModifiers(Task t)
        {
            List<string> mod = new List<string>();
            if(t.Monthly_In_Modifiers != null) {
                foreach(InModifiers m in Enums<InModifiers>()) {
                    if (((InModifiers)t.Monthly_In_Modifiers).HasFlag(m)) mod.Add(modifiersNames[m]);
                }
            }
            return string.Join(", ", mod);
        }

        public string ViewMonthDays(Task t)
        {
            List<string> days = new List<string>();
            if(t.Monthly_Days != null) {
                foreach(DaysInMonth d in Enums<DaysInMonth>()) {
                    if (((DaysInMonth)t.Monthly_Days).HasFlag(d)) days.Add(daysInMonthNames[d]);
                }
            }
            return string.Join(", ", days);
        }


        public string BuildWeekDays(Task t)
        {
            ResetSequences();
            string days = "";
            
            if(t.Weekly_Days != null) {
                days += " (";
                
                foreach(Days d in Enums<Days>()) {
                    if(((Days)t.Weekly_Days).HasFlag(d)) sequence.Add(daysShortNames[d]);
                    else SetSequence();
                }
                SetSequence();
                
                days += string.Join(", ", sequences);
                days += ")";
            }

            return days;
        }

        public string BuildMonths(Task t)
        {
            ResetSequences();
            string months = "";

            if (t.Monthly_Months != null) {
                months += " (";
                
                foreach (Months m in Enums<Months>()) {
                    if (((Months)t.Monthly_Months).HasFlag(m)) sequence.Add(monthsShortNames[m]);
                    else SetSequence();
                }
                SetSequence();

                months += string.Join(", ", sequences);
                months += ")";
            }

            return months;
        }

        public string BuildMonthsIn(Task t)
        {
            ResetSequences();
            string str = "";

            str += " (";

            foreach (InModifiers m in Enums<InModifiers>()) {
                if (((InModifiers)t.Monthly_In_Modifiers).HasFlag(m)) sequence.Add(modifiersNames[m]);
                else SetSequence();
            }
            SetSequence();
            
            str += string.Join(", ", sequences);

            sequences = new List<string>();

            foreach (Days d in Enums<Days>()) {
                if (((Days)t.Monthly_In_Days).HasFlag(d)) sequence.Add(daysShortNames[d]);
                else SetSequence();
            }
            SetSequence();

            str += " ";
            str += string.Join(", ", sequences);
            
            str += ")";

            return str;
        }

        public string BuildMonthsDays(Task t)
        {
            ResetSequences();
            string days = "";

            if (t.Monthly_Days != null) {
                days += " (";

                foreach (DaysInMonth d in Enums<DaysInMonth>()) {
                    if (((DaysInMonth)t.Monthly_Days).HasFlag(d)) sequence.Add(daysInMonthNames[d]);
                    else SetSequence();
                }
                SetSequence();

                days += string.Join(", ", sequences);
                days += ")";
            }

            return days;
        }

        private void SetSequence()
        {
            if (sequence.Count() > 1) {
                sequences.Add(string.Format("{0}-{1}", sequence.First(), sequence.Last()));
            }
            if (sequence.Count() == 1) {
                sequences.Add(sequence.First());
            }
            sequence = new List<string>();
        }

        private void ResetSequences()
        {
            sequences = new List<string>();
            sequence = new List<string>();
        }
        
        private IEnumerable Enums<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        #endregion
    }
}
