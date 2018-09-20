using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
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

    /// </summary>
    [NexusRepository]
    public class GetTotalIncomeByAddressAction : Action
    {
        public override int Id
        {
            get
            {
                return 30119;
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
                return new string[] {"TransactionArray","Address"};
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
                return "Get Total Income By Address";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var transactions = (JArray)vars["TransactionArray"];
            double totalIncome = 0;
            //iterate transaction in the JSON array
            foreach (JObject transaction in transactions) {
                string destinationAddress = transaction.GetValue("to").ToString(); //get destination address value
                if (destinationAddress == (string)vars["Address"]) {
                    var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                    totalIncome += ethereumValue;
                }
            }

            outputVars["Result"] = totalIncome;

        }
    }
}
