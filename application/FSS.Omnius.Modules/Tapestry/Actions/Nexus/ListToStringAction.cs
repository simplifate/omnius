using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ListToStringAction : Action
    {
        public override int Id
        {
            get
            {
                return 3015;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "List", "?Separator" };
            }
        }

        public override string Name
        {
            get
            {
                return "List to string";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var List = (List<string>)vars["List"];
            var Separator = vars.ContainsKey("Separator") ? (string)vars["Separator"] : ",";
            
            outputVars["Result"] = string.Join(Separator, List.ToArray());
        }
    }
}
