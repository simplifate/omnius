using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class OpenUrlAction : Action
    {
        public override int Id
        {
            get
            {
                return 1111;
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
                return new string[] {"Block","ModelId", "?Button" };
            }
        }

        public override string Name
        {
            get
            {
                return "Open Url";
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

            string systemAccName = WebConfigurationManager.AppSettings["SystemAccountName"];
            string systemAccPass = WebConfigurationManager.AppSettings["SystemAccountPass"];

            string targetUrl;

            if (serverName == "localhost")
                targetUrl = $"https://omnius-as.azurewebsites.net/{appName}/{blockName}/Get?modelId={modelId}&User={systemAccName}&Password={systemAccPass}";
            else
                targetUrl = $"{hostname}/{appName}/{blockName}/Get?modelId={modelId}&User={systemAccName}&Password={systemAccPass}";

            if (button != "")
                targetUrl += $"&button={button}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }
    }
}
