using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class AddToListAction : Action
    {
        public override int Id => 10300;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "Value", "?List" };

        public override string Name => "Add to list";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var list = new List<object>();
            if (vars.ContainsKey("List"))
                list = (List<object>)vars["List"];

            object value = vars.ContainsKey("Value") ? vars["Value"] : null;
            if (value != null)
            {
                list.Add(value);
            }
            
            outputVars["Result"] = list;
        }
    }
}
