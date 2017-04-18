using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class RegisterTaskAction : Action
    {
        public override int Id
        {
            get
            {
                return 186;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "TaskName", "Block", "Minutes", "?Button", "?ModelId", "?StartTime" };
            }
        }

        public override string Name
        {
            get
            {
                return "Register one-time task";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "TaskId" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Entitron.AppName;
            string blockName = (string)vars["Block"];
            string button = vars.ContainsKey("Button") ? (string)vars["Button"] : "";
            int modelId = vars.ContainsKey("ModelId") ? (int)vars["ModelId"] : -1;
            string serverName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

            string systemAccName = WebConfigurationManager.AppSettings["SystemAccountName"];
            string systemAccPass = WebConfigurationManager.AppSettings["SystemAccountPass"];

            string targetUrl;

            if (serverName == "localhost")
                targetUrl = $"https://omniuswork.azurewebsites.net/{appName}/{blockName}/Get?modelId={modelId}&User={systemAccName}&Password={systemAccPass}";
            else
                targetUrl = $"{hostname}/{appName}/{blockName}/Get?modelId={modelId}&User={systemAccName}&Password={systemAccPass}";

            if (button != "")
                targetUrl += $"&button={button}";

            string taskName = (string)vars["TaskName"];
            DateTime time;
            if(vars.ContainsKey("StartTime"))
            {
                var localTime = (DateTime)vars["StartTime"];
                time = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified),
                    TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
            }
            else
            {
                int minutes = Convert.ToInt32(vars["Minutes"]);
                time = DateTime.Now.AddMinutes(minutes);
            }

            var newTask = new Task
            {
                AppId = core.Entitron.AppId,
                Active = true,
                Name = taskName,
                Type = ScheduleType.ONCE,
                Url = targetUrl,
                Repeat = false,
                Start_Time = new TimeSpan(time.Hour, time.Minute, 0),
                Start_Date = time
            };
            var cortex = new Cortex.Cortex();
            cortex.Save(newTask);

            outputVars["TaskId"] = newTask.Id;
        }
    }
}
