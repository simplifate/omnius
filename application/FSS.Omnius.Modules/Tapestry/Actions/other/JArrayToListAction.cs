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
    public class JArrayToList : Action
    {
        public override int Id
        {
            get
            {
                return 1149;
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
                return new string[] { "List" };
            }
        }

        public override string Name
        {
            get
            {
                return "JArray to list";
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
            if (vars["List"] is JArray)
            {
                JArray list = (JArray)vars["List"];
                if (list.Count > 0)
                {
                    outputVars["Result"] = list.ToObject<List<object>>();
                }
                else
                    outputVars["Result"] = null;
            }
            else
                outputVars["Result"] = null;

        }


    }
}
