namespace FSS.Omnius.Modules.Cortex
{
    using FSS.Omnius.Modules.Cortex.Interface;
    using FSS.Omnius.Modules.Entitron.Entity.Cortex;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Web;
    using System.Web.Configuration;
    using System.Xml;

    public class CortexSchtasks : ICortexAPI
    {
        private XmlDocument xml;
        private string xmlPath;

        private HttpRequestBase Request;

        public CortexSchtasks(HttpRequestBase request)
        {
            Request = request;
        }

        public string List()
        {
            return "";
        }

        public void Create(Task t)
        {
            string cmd = BuildTaskXML(t);
            Execute(cmd);
            CleanUp();
        }

        public void Change(Task t, Task original)
        {
            Delete(original);
            Create(t);
        }

        public void Delete(Task t)
        {
            string cmd = "Schtasks /delete /tn \"" + t.Name + "\"";
            Execute(cmd);
        }

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

            XmlNode task = createNode("Task", new { version = "1.2", xmlns = "http://schemas.microsoft.com/windows/2004/02/mit/task" });

            string triggerName = "";
            switch (model.Type) {
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
            if (model.End_Time != null || model.End_Date != null) {
                string endDateTime = string.Format("{0}T{1}",
                        (model.End_Date != null ? (DateTime)model.End_Date : DateTime.Now).ToString("yyyy-MM-dd"),
                        model.End_Time != null ? ((TimeSpan)model.End_Time).ToString(@"hh\:mm") + ":00" : "00:00:00"
                    );
                trigger.AppendChild(createNode("EndBoundary", endDateTime));
            }

            // Enabled
            trigger.AppendChild(createNode("Enabled", "true"));


            switch (model.Type) {
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

            if (model.Repeat) {
                trigger.AppendChild(createRepetition(model));
            }

            triggers.AppendChild(trigger);
            task.AppendChild(triggers);

            // Idle Settings
            XmlNode idleSettings = createNode("IdleSettings");
            idleSettings.AppendChild(createNode("StopOnIdleEnd", "true"));
            idleSettings.AppendChild(createNode("RestartOnIdle", "false"));

            if (model.Type == ScheduleType.ONIDLE) {
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

            //if (model.Type == ScheduleType.ONCE) {
            //    settings.AppendChild(createNode("DeleteExpiredTaskAfter", "PT0S"));
            //}

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
            xmlPath = @"C:\\Temp\\Task_" + model.Name + ".xml";

            xml.Save(xmlPath);
            cmd += "\"" + xmlPath + "\"";

            return cmd;
        }

        private void Execute(string cmdText)
        {
            using (Process process = new Process()) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/K " + cmdText;
                //startInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo = startInfo;
                process.Start();
            }
        }

        private void CleanUp()
        {
            if (!string.IsNullOrEmpty(xmlPath)) {
                File.Delete(xmlPath);
            }
        }

        #region tools

        private XmlNode createNode(string nodeName, Object attributes, string content)
        {
            XmlNode node = xml.CreateElement(nodeName);

            foreach (var prop in attributes.GetType().GetProperties()) {
                XmlAttribute attr = xml.CreateAttribute(prop.Name);
                attr.Value = prop.GetValue(attributes).ToString();
                node.Attributes.Append(attr);
            }

            if (!String.IsNullOrEmpty(content)) {
                node.AppendChild(xml.CreateTextNode(content));
            }
            return node;
        }
        private XmlNode createNode(string nodeName, Object attributes)
        {
            return createNode(nodeName, attributes, string.Empty);
        }
        private XmlNode createNode(string nodeName, string content)
        {
            return createNode(nodeName, new { }, content);
        }
        private XmlNode createNode(string nodeName)
        {
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
                if (daysOfMonth.Count > 0) {
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
                if (daysOfWeek.Count > 0) {
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
            if (months.Count > 0) {
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
            if (daysOfWeek.Count > 0) {
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

        private XmlNode createRepetition(Task model)
        {
            XmlNode repetition = createNode("Repetition");
            repetition.AppendChild(createNode("Interval", "PT" + model.Repeat_Minute + "M"));
            repetition.AppendChild(createNode("Duration", "PT" + model.Repeat_Duration + "H"));
            repetition.AppendChild(createNode("StopAtDurationEnd", "true"));

            return repetition;
        }

        private IEnumerable Enums<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        #endregion
    }
}
