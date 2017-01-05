using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    /// <summary>
    /// Provede SOAP volání a vrátí výsledek. Jako InputVars očekává název webové služby (WSName) a název metody (MethodName).
    /// Dále je třeba ve form collection předat všechny parametry, které metoda očekává (můžou být prázdné) ve formátu Param[...]
    /// </summary>
    [NexusRepository]
    public class CallSOAPWSAction : Action
    {
        public override int Id
        {
            get {
                return 3001;
            }
        }
        public override int? ReverseActionId
        {
            get {
                return null;
            }
        }
        public override string[] InputVar
        {
            get {
                return new string[] { "MethodName", "WSName", "?JsonBody" };
            }
        }
        public override string[] OutputVar
        {
            get {
                return new string[] { "Data" };
            }
        }
        public override string Name
        {
            get {
                return "Call SOAP";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            string serviceName = (string)vars["WSName"];
            string methodName = (string)vars["MethodName"];
            NexusWSService svc = new NexusWSService();
            JToken data;

            if (vars.ContainsKey("JsonBody")) {
                data = svc.CallWebService(serviceName, methodName, (string)vars["JsonBody"]);
            }
            else 
            {
                List<string> parameters = new List<string>();
                IEnumerable<string> urlParams = vars.Keys.Where(k => k.StartsWith("Param[") && k.EndsWith("]"));

                foreach (string key in urlParams) {
                    parameters.Add((string)vars[key]);
                }
                object[] args = parameters.ToArray<object>();
                
                data = svc.CallWebService(serviceName, methodName, args);
            }
            outputVars["Data"] = data;
        }
    }
}
