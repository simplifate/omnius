using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

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
            JArray list = (JArray)vars["List"];
            if (list.Count > 0)
            {
                int index = (int)vars["Index"];
                var result = list.ElementAt(index);
                outputVars["Result"] = result;
            }
            else
                outputVars["Result"] = null;
        }
    }
}
