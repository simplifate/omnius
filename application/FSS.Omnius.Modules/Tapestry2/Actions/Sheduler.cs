using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Sheduler : ActionManager
    {
        [Action(186, "Register one - time task", "TaskId")]
        public static int RegisterTask(COREobject core, string Block, string TaskName, string Button = "", int ModelId = -1, DateTime? StartTime = null, int Minutes = -1)
        {
            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Application.Name;
            string serverName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

            string systemAccName = WebConfigurationManager.AppSettings["SystemAccountName"];
            string systemAccPass = WebConfigurationManager.AppSettings["SystemAccountPass"];

            string targetUrl;

            if (serverName == "localhost")
                targetUrl = $"https://omnius-as.azurewebsites.net/{appName}/{Block}/Get?modelId={ModelId}&User={systemAccName}&Password={systemAccPass}";
            else
                targetUrl = $"{hostname}/{appName}/{Block}/Get?modelId={ModelId}&User={systemAccName}&Password={systemAccPass}";

            if (Button != "")
                targetUrl += $"&button={Button}";
            
            DateTime time;
            if (StartTime != null)
            {
                time = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(StartTime.Value, DateTimeKind.Unspecified),
                    TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
            }
            else
            {
                time = DateTime.Now.AddMinutes(Minutes);
            }

            var newTask = new Task
            {
                AppId = core.Application.Id,
                Active = true,
                Name = TaskName,
                Type = ScheduleType.ONCE,
                Url = targetUrl,
                Repeat = false,
                Start_Time = new TimeSpan(time.Hour, time.Minute, 0),
                Start_Date = time
            };
            var cortex = new Cortex.Cortex();
            cortex.Save(newTask);

            return newTask.Id.Value;
        }

        [Action(189, "Unregister task")]
        public static void UnregisterTask(COREobject core, string Block, string Button = "", int ModelId = -1)
        {
            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Application.Name;
            string serverName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

            string targetUrl;

            targetUrl = $"{hostname}/{appName}/{Block}/Get?modelId={ModelId}&User=Scheduler&Password=194GsQwd/AgB4ZZnf_uF";

            if (Button != "")
                targetUrl += $"&button={Button}";

            var cortex = new Cortex.Cortex();
            cortex.Delete(targetUrl);
        }
    }
}
