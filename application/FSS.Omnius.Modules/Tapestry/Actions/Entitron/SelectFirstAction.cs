using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    public class SelectFirstAction : Action
    {
        public override int Id => 1053;

        public override string[] InputVar => new string[] { "TableName", "?SearchInShared", "?OrderBy", "?Descending", "CondColumn[index]", "CondValue[index]", "?CondOperator[index]" };

        public override string[] OutputVar => new string[] { "Data" };

        public override int? ReverseActionId => null;

        public override string Name => "Select First(filter)";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            vars.Add("Top", 1);
            SelectAction sa = new SelectAction();
            sa.InnerRun(vars, outputVars, InvertedInputVars, message);
            outputVars["Data"] = (outputVars["Data"] as ListJson<DBItem>).FirstOrDefault();
        }
    }
}
