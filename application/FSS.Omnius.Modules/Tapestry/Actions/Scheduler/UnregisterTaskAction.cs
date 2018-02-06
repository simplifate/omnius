using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using System.Web;
using System.Web.Configuration;
using System;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class UnregisterTaskAction : Action
    {
        public override int Id
        {
            get
            {
                return 190;
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
                return new string[] { "Block", "?Button", "?ModelId" };
            }
        }

        public override string Name
        {
            get
            {
                return "Unregister task";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
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
            
            string targetUrl;
            
            targetUrl = $"{hostname}/{appName}/{blockName}/Get?modelId={modelId}&User=Scheduler&Password=194GsQwd/AgB4ZZnf_uF";

            if (button != "")
                targetUrl += $"&button={button}";

            var cortex = new Cortex.Cortex();
            cortex.Delete(targetUrl);
        }
    }
}
