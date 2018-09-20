using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Configuration;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class OpenUrlAction : Action
    {
        public override int Id => 1111;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] {"Block","ModelId", "?Button" };

        public override string Name => "Open Url";

        public override string[] OutputVar => new string[] { };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = COREobject.i.Entitron;

            string hostname = TapestryUtils.GetServerHostName();
            string appName = db.Application.Name;
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
