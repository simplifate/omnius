using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
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

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();

            ViewData["ApplicationList"] = e.Applications;
            ViewData["ScheduleTypeNames"] = scheduleTypeNames;
            ViewData["DaysNames"] = daysNames;
            ViewData["MonthsNames"] = monthsNames;
            ViewData["ModifierNames"] = modifiersNames;
            ViewData["DaysInMonthNames"] = daysInMonthNames;

            return View("~/Views/Cortex/Form.cshtml", e.Tasks.Single(t => t.Id == id));
        }

        public ActionResult Save(Task model)
        {
            DBEntities e = new DBEntities();
            Task row = !model.Id.Equals(null) ? e.Tasks.Single(m => m.Id == model.Id) : new Task();
               
            row.Active = model.Active;
            row.AppId = model.AppId;
            row.Daily_Repeat = model.Daily_Repeat;
            row.Duration = model.Duration;
            row.End_Date = model.End_Date;
            row.End_Time = model.End_Time;
            row.Hourly_Repeat = model.Hourly_Repeat;
            row.Idle_Time = model.Idle_Time;
            row.Minute_Repeat = model.Minute_Repeat;
            row.Monthly_Days = GetDaysInMonthFlags();
            row.Monthly_In_Days = GetDaysFlags("Monthly_In_Days[]");
            row.Monthly_In_Modifiers = GetModifierFlags();
            row.Monthly_Months = GetMonthsFlags();
            row.Monthly_Type = model.Monthly_Type;
            row.Name = model.Name;
            row.Start_Date = model.Start_Date;
            row.Start_Time = model.Start_Time;
            row.Type = model.Type;
            row.Url = model.Url;
            row.Weekly_Days = GetDaysFlags("Weekly_Days[]");
            row.Weekly_Repeat = model.Weekly_Repeat;

            if(model.Id.Equals(null)) {
                e.Tasks.Add(row);
                CreateTask(row);
            }
            else {
                ChangeTask(row);
            }

            //e.SaveChanges();

            return RedirectToRoute("Cortex", new { @action = "Index" });
        }
        
        private void CreateTask(Task model)
        {
            string cmd = BuildTaskParams(model);
            Execute(cmd);
        }

        private void ChangeTask(Task model)
        {
            DeleteTask(model);
            CreateTask(model);
        }

        private void DeleteTask(Task model)
        {
            string cmd = "Schtasks /delete /tn \"" + model.Name + "\"";
            Execute(cmd);
        }

        private string BuildTaskParams(Task model)
        {
            string cmd = String.Empty;
            string run = String.Empty;

            ConfigurationSection loggerConfig = (ConfigurationSection)WebConfigurationManager.GetSection("logger");
            string logPath = loggerConfig.ElementInformation.Properties["rootDir"].Value.ToString();

            logPath += "\\Cortex\\" + model.Name + ".html";
            run = "wget -O \\\"" + logPath + "\\\" http://" + Request.Url.Host + model.Url;

            cmd += "Schtasks /create /sc " + model.Type.ToString();     // Nastaví typ tasku 
            cmd += " /tn \"" + model.Name + "\"";                       // Nastaví jméno tasku
            cmd += " /tr \"" + run + "\"";                              // Spouštěná úloha

            // Modifikátor
            switch (model.Type) {
                case ScheduleType.MINUTE: {
                        cmd += model.Minute_Repeat == null ? "" : " /mo " + model.Minute_Repeat;
                        break;
                    }
                case ScheduleType.HOURLY: {
                        cmd += model.Hourly_Repeat == null ? "" : " /mo " + model.Hourly_Repeat;
                        break;
                    }
                case ScheduleType.DAILY: {
                        cmd += model.Daily_Repeat == null ? "" : " /mo " + model.Daily_Repeat;
                        break;
                    }
                case ScheduleType.WEEKLY: {
                        cmd += model.Weekly_Repeat == null ? "" : " /mo " + model.Weekly_Repeat;
                        break;
                    }
                case ScheduleType.MONTHLY: {
                        if (model.Monthly_Type == MonthlyType.IN) {
                            List<string> mods = new List<string>();
                            foreach(InModifiers mod in Enums<InModifiers>()) {
                                if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(mod)) mods.Add(mod.ToString());
                            }
                            if (mods.Count() > 0) {
                                cmd += " /mo " + String.Join(",", mods);
                            }
                        }
                        if (model.Monthly_Type == MonthlyType.DAYS && (DaysInMonth)model.Monthly_Days == DaysInMonth.LAST) {
                            cmd += " /mo " + DaysInMonth.LAST.ToString();
                        }
                        break;
                    }
            }

            // Den
            List<string> days = new List<string>();
            switch (model.Type) {
                case ScheduleType.WEEKLY: {
                        foreach (Days day in Enums<Days>()) {
                            if (((Days)model.Weekly_Days).HasFlag(day)) days.Add(day.ToString());
                        }
                        break;
                    }
                case ScheduleType.MONTHLY: {
                        if (model.Monthly_Type == MonthlyType.DAYS) {
                            foreach (DaysInMonth day in Enums<DaysInMonth>()) {
                                if (((DaysInMonth)model.Monthly_Days).HasFlag(day)) days.Add(day.ToString());
                            }
                        }
                        if (model.Monthly_Type == MonthlyType.IN) {
                            foreach (Days day in Enums<Days>()) {
                                if (((Days)model.Monthly_In_Days).HasFlag(day)) days.Add(day.ToString());
                            }
                        }
                        break;
                    }
            }
            if (days.Count() > 0) {
                cmd += " /d " + String.Join(",", days);
            }

            // Měsíc
            if (model.Type == ScheduleType.MONTHLY) {
                List<string> months = new List<string>();
                foreach (Months month in Enums<Months>()) {
                    if (((Months)model.Monthly_Months).HasFlag(month)) months.Add(month.ToString());
                }
                if (months.Count() > 0) {
                    cmd += " /m " + String.Join(",", months);
                }
            }

            // Idle time
            if (model.Idle_Time > 0 && model.Type == ScheduleType.ONIDLE) {
                cmd += " /i " + model.Idle_Time;
            }

            // Start time
            List<ScheduleType> stAllowed = new List<ScheduleType>() { ScheduleType.MINUTE, ScheduleType.HOURLY, ScheduleType.DAILY, ScheduleType.WEEKLY, ScheduleType.MONTHLY, ScheduleType.ONCE };
            if (stAllowed.Contains(model.Type)) {
                cmd += " /st " + model.Start_Time.ToString(@"hh\:mm");
            }

            // End time
            if ((model.Type == ScheduleType.MINUTE || model.Type == ScheduleType.HOURLY) && model.End_Time != null) {
                cmd += " /et " + ((TimeSpan)model.End_Time).ToString(@"hh\:mm");
            }

            // Duration
            if ((model.Type == ScheduleType.MINUTE || model.Type == ScheduleType.HOURLY) && model.Duration != null) {
                cmd += " /du " + String.Format("{0:0000}:{1:00}", (int)((TimeSpan)model.Duration).TotalHours, ((TimeSpan)model.Duration).Minutes);
            }

            // Start date
            if (model.Start_Date != null) {
                cmd += " /sd " + ((DateTime)model.Start_Date).ToShortDateString();
            }

            // end date
            List<ScheduleType> edNotAllowed = new List<ScheduleType>() { ScheduleType.ONCE, ScheduleType.ONIDLE, ScheduleType.ONSTART };
            if (model.End_Date != null && !edNotAllowed.Contains(model.Type)) {
                cmd += " /ed " + ((DateTime)model.End_Date).ToShortDateString();
            }

            return cmd;
        }
        
        private void Execute(string cmdText)
        {
            using (Process process = new Process()) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/K " + cmdText;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo = startInfo;
                process.Start();
            }
        }

        #region TOOLS

        private IEnumerable Enums<T>()
        {
            return Enum.GetValues(typeof(T));
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
            monthsNames.Add(Months.JUN, "Červen");
            monthsNames.Add(Months.JUL, "Červenec");
            monthsNames.Add(Months.AUG, "Srpen");
            monthsNames.Add(Months.SEP, "Září");
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
            int i = 0;
            foreach (DaysInMonth day in Enums<DaysInMonth>()) {
                if (day == DaysInMonth.LAST) daysInMonthNames.Add(day, "Poslední");
                else daysInMonthNames.Add(day, (++i).ToString());
            }
        }

        private int GetDaysFlags(string formKey)
        {
            int flag = 0;
            if (!String.IsNullOrEmpty((string)Request.Form[formKey])) {
                List<string> selected = ((string)Request.Form[formKey]).Split(',').ToList();
                foreach (Days day in Enums<Days>()) {
                    if (selected.Contains(day.ToString())) flag = flag | (int)day;
                }
            }
            return flag;
        }

        private int GetModifierFlags()
        {
            int flag = 0;
            if (!String.IsNullOrEmpty((string)Request.Form["Monthly_In_Modifiers[]"])) {
                List<string> selected = ((string)Request.Form["Monthly_In_Modifiers[]"]).Split(',').ToList();
                foreach (InModifiers mod in Enums<InModifiers>()) {
                    if (selected.Contains(mod.ToString())) flag = flag | (int)mod;
                }
            }
            return flag;
        }

        private int GetMonthsFlags()
        {
            int flag = 0;
            if (!String.IsNullOrEmpty((string)Request.Form["Monthly_Months[]"])) {
                List<string> selected = ((string)Request.Form["Monthly_Months[]"]).Split(',').ToList();
                foreach (Months month in Enums<Months>()) {
                    if (selected.Contains(month.ToString())) flag = flag | (int)month;
                }
            }
            return flag;
        }

        private Int64 GetDaysInMonthFlags()
        {
            Int64 flag = 0;
            if (!String.IsNullOrEmpty((string)Request.Form["Monthly_Days[]"])) {
                List<string> selected = ((string)Request.Form["Monthly_Days[]"]).Split(',').ToList();
                foreach (DaysInMonth day in Enums<DaysInMonth>()) {
                    if (selected.Contains(day.ToString())) flag = flag | (Int64)day;
                }
            }
            return flag;
        }

        #endregion
    }
}
