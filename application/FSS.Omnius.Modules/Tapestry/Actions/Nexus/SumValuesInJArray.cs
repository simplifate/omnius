using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    /// <summary>
    /// 
    /// </summary>
    [NexusRepository]
    public class SumValuesInJArray : Action
    {
        public override int Id
        {
            get
            {
                return 30106;
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
                return new string[] { "Array", "ValueName" };
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
                return "Sum Values In JArray";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            JArray array = new JArray();
            array = (JArray)vars["Array"];
            var valueName = (string)vars["ValueName"];
            double total = 0;
            if (array.Count > 0)
            {
                foreach (JObject obj in array)
                {
                    total += Convert.ToDouble(obj[valueName]);
                }
                outputVars["Result"] = total;
            }
            else
            {
                outputVars["Result"] = 0;
            }
        }
    }
}