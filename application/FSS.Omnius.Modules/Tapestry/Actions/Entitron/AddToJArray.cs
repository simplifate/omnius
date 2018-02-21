using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class AddToJarrayAction : Action
    {
        public override int Id => 10301;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "Object", "?Array" };

        public override string Name => "Add to JArray";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var Array = new JArray();
            if (vars.ContainsKey("Array"))
                Array = (JArray)vars["Array"];

            object Obj = vars.ContainsKey("Object") ? vars["Object"] : null;
            if (Obj != null)
            {
                Array.Add(Obj);
            }

            outputVars["Result"] = Array;
        }
    }
}
