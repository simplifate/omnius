using System.Collections.Generic;
using System.Web;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class UnregisterTaskAction : Action
    {
        public override int Id => 190;
        public override int? ReverseActionId => null;
        public override string[] InputVar => new string[] { "Block", "?Button", "?ModelId" };
        public override string Name => "Unregister task";
        public override string[] OutputVar => new string[] { };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;

            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Application.Name;
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
