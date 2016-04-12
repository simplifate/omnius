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
            row.Name = model.Name;
            row.Start_Date = model.Start_Date;
            row.Start_Time = model.Start_Time;
            row.Type = model.Type;
            row.Url = model.Url;
            row.Weekly_Days = GetDaysFlags("Weekly_Days[]");
            row.Weekly_Repeat = model.Weekly_Repeat;

            if(model.Id.Equals(null)) {
                e.Tasks.Add(row);
                CreateScheduleTask(row);
            }
            else {
                ChangeScheduleTask(row);
            }

            //e.SaveChanges();

            return RedirectToRoute("Cortex", new { @action = "Index" });
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
            int i = 1;
            foreach (DaysInMonth day in Enum.GetValues(typeof(DaysInMonth))) {
                if(day == DaysInMonth.LAST) {
                    daysInMonthNames.Add(day, "Poslední");
                    continue;
                }

                daysInMonthNames.Add(day, i.ToString());
                i++;
            }
        }

        private int GetDaysFlags(string formKey)
        {
            int flag = 0;
            if (!String.IsNullOrEmpty((string)Request.Form[formKey])) {
                List<string> selected = ((string)Request.Form[formKey]).Split(',').ToList();
                foreach (Days day in Enum.GetValues(typeof(Days))) {
                    if (selected.Contains(day.ToString())) {
                        flag = flag | (int)day;
                    }
                }
            }
            return flag;
        }

        private int GetModifierFlags()
        {
            int flag = 0;
            if (!String.IsNullOrEmpty((string)Request.Form["Monthly_In_Modifiers[]"])) {
                List<string> selected = ((string)Request.Form["Monthly_In_Modifiers[]"]).Split(',').ToList();
                foreach (InModifiers mod in Enum.GetValues(typeof(InModifiers))) {
                    if (selected.Contains(mod.ToString())) {
                        flag = flag | (int)mod;
                    }
                }
            }
            return flag;
        }

        private int GetMonthsFlags()
        {
            int flag = 0;
            if (!String.IsNullOrEmpty((string)Request.Form["Monthly_Months[]"])) {
                List<string> selected = ((string)Request.Form["Monthly_Months[]"]).Split(',').ToList();
                foreach (Months month in Enum.GetValues(typeof(Months))) {
                    if (selected.Contains(month.ToString())) {
                        flag = flag | (int)month;
                    }
                }
            }
            return flag;
        }

        private Int64 GetDaysInMonthFlags()
        {
            Int64 flag = 0;

            if(!String.IsNullOrEmpty((string)Request.Form["Monthly_Days[]"])) { 
                List<string> selected = ((string)Request.Form["Monthly_Days[]"]).Split(',').ToList();
                foreach (DaysInMonth day in Enum.GetValues(typeof(DaysInMonth))) {
                    if(selected.Contains(day.ToString())) {
                        flag = flag | (Int64)day;
                    }
                }
            }
            return flag;
        }

        private void CreateScheduleTask(Task model)
        {
            ConfigurationSection loggerConfig = (ConfigurationSection)WebConfigurationManager.GetSection("logger");

            string cmdText = String.Empty;
            string run = String.Empty;
            string logPath = loggerConfig.ElementInformation.Properties["rootDir"].Value.ToString();

            logPath += "\\Cortex\\" + model.Name + ".html";
            run = "wget -O \\\"" + logPath + "\\\" " + Request.Url.Host + "/" + model.Url;

            cmdText += "Schtasks /create /sc " + model.Type.ToString();     // Nastaví typ tasku 
            cmdText += " /tn \"" + model.Name + "\"";                       // Nastaví jméno tasku
            cmdText += " /tr \"" + run + "\"";                              // Spouštěná úloha

            // Modifikátor
            switch(model.Type) 
            {
                case ScheduleType.MINUTE: {
                        cmdText += model.Minute_Repeat == null ? "" : " /mo " + model.Minute_Repeat;
                        break;
                    }
                case ScheduleType.HOURLY: {
                        cmdText += model.Hourly_Repeat == null ? "" : " /mo " + model.Hourly_Repeat;
                        break;
                    }
                case ScheduleType.DAILY: {
                        cmdText += model.Daily_Repeat == null ? "" : " /mo " + model.Daily_Repeat;
                        break;
                    }
                case ScheduleType.WEEKLY: {
                        cmdText += model.Weekly_Repeat == null ? "" : " /mo " + model.Weekly_Repeat;
                        break;
                    }
                case ScheduleType.MONTHLY: {
                        if(model.Monthly_Type == MonthlyType.IN) {
                            List<string> mods = new List<string>();
                            if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(InModifiers.FIRST)) mods.Add(InModifiers.FIRST.ToString());
                            if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(InModifiers.SECOND)) mods.Add(InModifiers.SECOND.ToString());
                            if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(InModifiers.THIRD)) mods.Add(InModifiers.THIRD.ToString());
                            if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(InModifiers.FOURTH)) mods.Add(InModifiers.FOURTH.ToString());
                            if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(InModifiers.LAST)) mods.Add(InModifiers.LAST.ToString());

                            if(mods.Count() > 0) {
                                cmdText += " /mo " + String.Join(",", mods);
                            } 
                        }
                        if(model.Monthly_Type == MonthlyType.DAYS && (DaysInMonth)model.Monthly_Days == DaysInMonth.LAST) {
                            cmdText += " /mo " + DaysInMonth.LAST.ToString();
                        }
                        break;
                    }
            }

            // Den
            switch(model.Type) {
                /*case ScheduleType.WEEKLY: {
                        List<string> days = new List<string>();
                        
                    }*/
            }
            
        }

        private void ChangeScheduleTask(Task model)
        {

        }

        private void Execute(string cmdText)
        {
            using (Process process = new Process()) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C " + cmdText;
                process.StartInfo = startInfo;
                process.Start();
            }
        }
    }
}
