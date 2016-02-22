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
                return new string[] { "MethodName", "WSName" };
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
                return "SOAP CALL";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            string serviceName = (string)vars["WSName"];
            string methodName = (string)vars["MethodName"];

            object[] args;
            List<string> parameters = new List<string>();
            IEnumerable<string> urlParams = core._form.AllKeys.Where(k => k.StartsWith("Param[") && k.EndsWith("]"));

            if(urlParams.Count() > 0) {
                foreach(string key in urlParams) {
                    parameters.Add(core._form[key]);
                }
                args = parameters.ToArray<object>();
            }
            else {
                args = new object[] { };
            }

            NexusWSService svc = new NexusWSService();
            JToken data = svc.CallWebService(serviceName, methodName, args);

            outputVars["Data"] = data;
        }
    }
}
