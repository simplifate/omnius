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
    public class CountObjectInObjectAction : Action
    {
        public override int Id
        {
            get
            {
                return 30103;
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
                return new string[] { "InputObject","?Property","?Value" };
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
                return "Count Object In Object";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var dbItems = (JObject)vars["InputObject"];
            if (dbItems.Count > 0) {
                if (!(vars.ContainsKey("Property")&& vars.ContainsKey("Value") ))
                {
                    outputVars["Result"] = dbItems.Count;
                }
                else {
                    string propertyName = (string)vars["Property"];
                    string Value = (string)vars["Value"];
                    int counter = 0;
                    foreach (var item in dbItems)
                    {
                        if (((JObject)item.Value).Property(propertyName).Value.ToString() == Value)
                            counter++;
                    }
                    outputVars["Result"] = counter;
                }
            }
            else
                outputVars["Result"] = 0;

        }
    }
}
