using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetListsElementByIndexAction : Action
    {
        public override int Id => 1049;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "List", "Index" };

        public override string Name => "Get Lists Element By Index";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            try
            {
                // get index
                var indexParam = vars["Index"];
                int index = indexParam is JValue ? ((JValue)indexParam).ToObject<int>() : Convert.ToInt32(indexParam);

                // get value
                if (vars["List"] is JArray)
                {
                    JArray list = (JArray)vars["List"];
                    outputVars["Result"] = list.ElementAt(index);
                }
                else
                {
                    List<Object> list = (List<Object>)vars["List"];
                    outputVars["Result"] = list.ElementAt(index);
                }
                outputVars["Error"] = false;
            }
            catch (Exception)
            {
                outputVars["Error"] = true;
                outputVars["Result"] = null;
            }
        }
    }
}
