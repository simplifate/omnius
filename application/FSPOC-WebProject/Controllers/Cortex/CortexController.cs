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
using System.Xml;
using System.Xml.Linq;

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

        private XmlDocument xml;

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
            row.Repeat = model.Repeat;
            row.Repeat_Minute = model.Repeat_Minute;
            row.Repeat_Duration = model.Repeat_Duration;
            row.Idle_Time = model.Idle_Time;
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
            string cmd = BuildTaskXML(model);
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
        /*
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
                        if (model.Monthly_Type == MonthlyType.DAYS && ((DaysInMonth)model.Monthly_Days).HasFlag(DaysInMonth.LASTDAY)) {
                            cmd += " /mo " + DaysInMonth.LASTDAY.ToString();
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
                                if (((DaysInMonth)model.Monthly_Days).HasFlag(day) && day != DaysInMonth.LASTDAY) days.Add(day.ToString().Replace("_", ""));
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
        */
        private string BuildTaskXML(Task model)
        {
            string cmd = string.Empty;
            string run = string.Empty;
            string runAttr = string.Empty;

            ConfigurationSection loggerConfig = (ConfigurationSection)WebConfigurationManager.GetSection("logger");
            string logPath = loggerConfig.ElementInformation.Properties["rootDir"].Value.ToString();

            string cortexUserName = WebConfigurationManager.AppSettings["CortexUserName"];
            string cortexUserPassword = WebConfigurationManager.AppSettings["CortexUserPassword"];

            logPath += "\\Cortex\\" + model.Name + ".html";
            run = "wget";
            runAttr = "-O \\\"" + logPath + "\\\" http://" + Request.Url.Host + model.Url;

            cmd += "Schtasks /create /ru " + cortexUserName + " /rp " + cortexUserPassword + " /TN \"" + model.Name + "\" /XML ";
            
            xml = new XmlDocument();
            XmlNode n = xml.CreateXmlDeclaration("1.0", "UTF-16", null);
            xml.AppendChild(n);

            XmlNode task = createNode("Task", new {version = "1.2", xmlns = "http://schemas.microsoft.com/windows/2004/02/mit/task" });

            string triggerName = "";
            switch(model.Type) {
                case ScheduleType.ONCE:
                    triggerName = "TimeTrigger";
                    break;
                case ScheduleType.ONIDLE:
                    triggerName = "IdleTrigger";
                    break;
                default:
                    triggerName = "CalendarTrigger";
                    break;
            }

            XmlNode triggers = createNode("Triggers");
            XmlNode trigger = createNode(triggerName);

            // Počátek platnosti
            string startDateTime = string.Format("{0}T{1}",
                    (model.Start_Date != null ? (DateTime)model.Start_Date : DateTime.Now).ToString("yyyy-MM-dd"),
                    ((TimeSpan)model.Start_Time).ToString(@"hh\:mm") + ":00"
                );
            trigger.AppendChild(createNode("StartBoundary", startDateTime));

            // Konec platnosti
            if(model.End_Time != null || model.End_Date != null) {
                string endDateTime = string.Format("{0}T{1}",
                        (model.End_Date != null ? (DateTime)model.End_Date : DateTime.Now).ToString("yyyy-MM-dd"),
                        model.End_Time != null ? ((TimeSpan)model.End_Time).ToString(@"hh\:mm") + ":00" : "00:00:00"
                    );
                trigger.AppendChild(createNode("EndBoundary", endDateTime));
            }

            // Enabled
            trigger.AppendChild(createNode("Enabled", "true"));

            
            switch(model.Type) {
                case ScheduleType.MONTHLY: {
                        trigger.AppendChild(createMonthlyTrigger(model));
                        break;
                    }
                case ScheduleType.WEEKLY: {
                        trigger.AppendChild(createWeeklyTrigger(model));
                        break;
                    }
                case ScheduleType.DAILY: {
                        trigger.AppendChild(createDaylyTrigger(model));
                        break;
                    }
            }

            triggers.AppendChild(trigger);
            task.AppendChild(triggers);

            // Idle Settings
            XmlNode idleSettings = createNode("IdleSettings");
            idleSettings.AppendChild(createNode("StopOnIdleEnd", "true"));
            idleSettings.AppendChild(createNode("RestartOnIdle", "false"));

            if(model.Type == ScheduleType.ONIDLE) {
                idleSettings.AppendChild(createNode("Duration", "PT" + model.Idle_Time + "M"));
                idleSettings.AppendChild(createNode("WaitTimeout", "PT1H"));
            }

            // SETTINGS
            XmlNode settings = createNode("Settings");
            settings.AppendChild(createNode("MultipleInstancesPolicy", "IgnoreNew"));
            settings.AppendChild(createNode("DisallowStartIfOnBatteries", "false"));
            settings.AppendChild(createNode("StopIfGoingOnBatteries", "false"));
            settings.AppendChild(createNode("AllowHardTerminate", "true"));
            settings.AppendChild(createNode("StartWhenAvailable", "false"));
            settings.AppendChild(createNode("RunOnlyIfNetworkAvailable", "true"));
            settings.AppendChild(createNode("AllowStartOnDemand", "true"));
            settings.AppendChild(createNode("Enabled", model.Active ? "true" : "false"));
            settings.AppendChild(createNode("Hidden", "false"));
            settings.AppendChild(createNode("RunOnlyIfIdle", model.Type == ScheduleType.ONIDLE ? "true" : "false"));
            settings.AppendChild(createNode("WakeToRun", "false"));
            settings.AppendChild(createNode("ExecutionTimeLimit", "PT72H"));
            settings.AppendChild(createNode("Priority", "7"));
            settings.AppendChild(idleSettings);

            task.AppendChild(settings);

            // Principals
            XmlNode principals = createNode("Principals");
            XmlNode principal = createNode("Principal", new { id = "Author" });
            principal.AppendChild(createNode("UserId", System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString()));
            principal.AppendChild(createNode("LogonType", "S4U"));
            principal.AppendChild(createNode("RunLevel", "LeastPrivilege"));

            principals.AppendChild(principal);
            task.AppendChild(principals);
            
            // ACTION
            XmlNode actions = createNode("Actions", new { Context = "Author" });

            XmlNode exec = createNode("Exec");
            exec.AppendChild(createNode("Command", run));
            exec.AppendChild(createNode("Arguments", runAttr));

            actions.AppendChild(exec);
            task.AppendChild(actions);
            xml.AppendChild(task);

            // Uložení XML
            string xmlPath = @"C:\\Temp\\Task_" + model.Name + ".xml";
            
            xml.Save(xmlPath);
            cmd += "\"" + xmlPath + "\"";

            // Start time
            /*List<ScheduleType> stAllowed = new List<ScheduleType>() { ScheduleType.MINUTE, ScheduleType.HOURLY, ScheduleType.DAILY, ScheduleType.WEEKLY, ScheduleType.MONTHLY, ScheduleType.ONCE };
            if (stAllowed.Contains(model.Type)) {
                cmd += " /st " + model.Start_Time.ToString(@"hh\:mm");
            }

            // End time
            if ((model.Type == ScheduleType.MINUTE || model.Type == ScheduleType.HOURLY) && model.End_Time != null) {
                cmd += " /et " + ((TimeSpan)model.End_Time).ToString(@"hh\:mm");
            }*/

            // end date
            /*List<ScheduleType> edNotAllowed = new List<ScheduleType>() { ScheduleType.ONCE, ScheduleType.ONIDLE, ScheduleType.ONSTART };
            if (model.End_Date != null && !edNotAllowed.Contains(model.Type)) {
                cmd += " /ed " + ((DateTime)model.End_Date).ToShortDateString();
            }*/

            /*


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
                            foreach (InModifiers mod in Enums<InModifiers>()) {
                                if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(mod)) mods.Add(mod.ToString());
                            }
                            if (mods.Count() > 0) {
                                cmd += " /mo " + String.Join(",", mods);
                            }
                        }
                        if (model.Monthly_Type == MonthlyType.DAYS && ((DaysInMonth)model.Monthly_Days).HasFlag(DaysInMonth.LASTDAY)) {
                            cmd += " /mo " + DaysInMonth.LASTDAY.ToString();
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
                                if (((DaysInMonth)model.Monthly_Days).HasFlag(day) && day != DaysInMonth.LASTDAY) days.Add(day.ToString().Replace("_", ""));
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
            

            // Idle time
            if (model.Idle_Time > 0 && model.Type == ScheduleType.ONIDLE) {
                cmd += " /i " + model.Idle_Time;
            }

            

            // Duration
            if ((model.Type == ScheduleType.MINUTE || model.Type == ScheduleType.HOURLY) && model.Duration != null) {
                cmd += " /du " + String.Format("{0:0000}:{1:00}", (int)((TimeSpan)model.Duration).TotalHours, ((TimeSpan)model.Duration).Minutes);
            }
            */
            

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

        private XmlNode createNode(string nodeName, Object attributes, string content)
        {
            XmlNode node = xml.CreateElement(nodeName);

            foreach (var prop in attributes.GetType().GetProperties()) {
                XmlAttribute attr = xml.CreateAttribute(prop.Name);
                attr.Value = prop.GetValue(attributes).ToString();
                node.Attributes.Append(attr);
            }
         
            if(!String.IsNullOrEmpty(content)) {
                node.AppendChild(xml.CreateTextNode(content));
            }
            return node;
        }
        private XmlNode createNode(string nodeName, Object attributes) {
            return createNode(nodeName, attributes, string.Empty);
        }
        private XmlNode createNode(string nodeName, string content) {
            return createNode(nodeName, new { }, content);
        }
        private XmlNode createNode(string nodeName) {
            return createNode(nodeName, new { }, string.Empty);
        }

        private XmlNode createMonthlyTrigger(Task model)
        {
            XmlNode trigger;
            if (model.Monthly_Type == MonthlyType.DAYS) {
                trigger = createNode("ScheduleByMonth");
                List<string> daysOfMonth = new List<string>();
                foreach (DaysInMonth day in Enums<DaysInMonth>()) {
                    if (((DaysInMonth)model.Monthly_Days).HasFlag(day)) daysOfMonth.Add(day.ToString().Replace("_", ""));
                }
                if (daysOfMonth.Count() > 0) {
                    XmlNode daysOfMonthNode = createNode("DaysOfMonth");
                    foreach (string day in daysOfMonth) {
                        daysOfMonthNode.AppendChild(createNode("Day", day));
                    }
                    trigger.AppendChild(daysOfMonthNode);
                }
            }
            else //(model.Monthly_Type == MonthlyType.IN) 
            {
                trigger = createNode("ScheduleByMonthDayOfWeek");
                List<string> weeks = new List<string>();
                int i = 1;
                foreach (InModifiers mod in Enums<InModifiers>()) {
                    if (((InModifiers)model.Monthly_In_Modifiers).HasFlag(mod)) weeks.Add(i <= 4 ? i.ToString() : "Last");
                    i++;
                }
                if (weeks.Count > 0) {
                    XmlNode weeksNode = createNode("Weeks");
                    foreach (string week in weeks) {
                        weeksNode.AppendChild(createNode("Week", week));
                    }
                    trigger.AppendChild(weeksNode);
                }

                List<string> daysOfWeek = new List<string>();
                foreach (Days day in Enums<Days>()) {
                    if (((Days)model.Monthly_In_Days).HasFlag(day)) daysOfWeek.Add(day.ToString());
                }
                if (daysOfWeek.Count() > 0) {
                    XmlNode daysOfWeekNode = createNode("DaysOfWeek");
                    foreach (string day in daysOfWeek) {
                        daysOfWeekNode.AppendChild(createNode(day));
                    }
                    trigger.AppendChild(daysOfWeekNode);
                }
            }

            List<string> months = new List<string>();
            foreach (Months month in Enums<Months>()) {
                if (((Months)model.Monthly_Months).HasFlag(month)) months.Add(month.ToString());
            }
            if (months.Count() > 0) {
                XmlNode monthsNode = createNode("Months");
                foreach (string month in months) {
                    monthsNode.AppendChild(createNode(month));
                }
                trigger.AppendChild(monthsNode);
            }
            return trigger;
        }

        private XmlNode createWeeklyTrigger(Task model)
        {
            XmlNode trigger = createNode("ScheduleByWeek");

            List<string> daysOfWeek = new List<string>();
            foreach (Days day in Enums<Days>()) {
                if (((Days)model.Weekly_Days).HasFlag(day)) daysOfWeek.Add(day.ToString());
            }
            if (daysOfWeek.Count() > 0) {
                XmlNode daysOfWeekNode = createNode("DaysOfWeek");
                foreach (string day in daysOfWeek) {
                    daysOfWeekNode.AppendChild(createNode(day));
                }
                trigger.AppendChild(daysOfWeekNode);
            }
            trigger.AppendChild(createNode("WeeksInterval", model.Weekly_Repeat.ToString()));

            return trigger;
        }

        private XmlNode createDaylyTrigger(Task model)
        {
            XmlNode trigger = createNode("ScheduleByDay");
            trigger.AppendChild(createNode("DaysInterval", model.Daily_Repeat.ToString()));
            return trigger;
        }

        private IEnumerable Enums<T>()
        {
            return Enum.GetValues(typeof(T));
        }

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
