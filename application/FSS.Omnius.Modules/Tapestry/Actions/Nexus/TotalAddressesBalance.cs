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
    public class TotalAddressesBalance : Action
    {
        public override int Id
        {
            get
            {
                return 30105;
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
                return new string[] { "Array" , "?s$BalanceKey" };
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        public override string Name
        {
            get
            {
                return "Total Addresses Balance";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string balanceKey = vars.ContainsKey("BalanceKey") ? (string)vars["BalanceKey"] : "balance";
            double total = 0;
            var input = vars["Array"];
            if (input is JArray) {
                JArray array = (JArray)input;
                if (array.Count > 0)
                {
                    foreach (JObject obj in array)
                    {
                        total += Convert.ToDouble(obj[balanceKey]);
                    }
                    outputVars["Result"] = total;
                }
                else
                {
                    outputVars["Result"] = 0;
                }
            }
            else if(input is JObject){
                JObject jObject = (JObject)input;
                if (jObject[balanceKey] != null)
                    outputVars["Result"] = Convert.ToDouble(jObject[balanceKey]);
                else
                    outputVars["Result"] = 0;
            }
            else{
                throw new ArgumentException("Total Address Balance Action: Input is not JArray nor JObject!");
            }

            
        }
    }
}
