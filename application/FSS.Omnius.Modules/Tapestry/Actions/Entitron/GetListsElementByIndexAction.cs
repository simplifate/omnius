using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetListsElementByIndexAction : Action
    {
        public override int Id
        {
            get
            {
                return 1049;
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
                return new string[] { "List", "Index" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get Lists Element By Index";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

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
