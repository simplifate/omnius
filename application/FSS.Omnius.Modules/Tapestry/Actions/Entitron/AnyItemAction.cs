using FSS.Omnius.Modules.CORE;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class AnyItemAction : Action
    {
        public override int Id => 1021;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "CondColumn[index]", "CondValue[index]", "?CondOperation[index]", "?SearchInShared" };

        public override string Name => "Select (Any)";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string,object> invertedVars, Message message)
        {
            // init
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            DBTable table = db.Table((string)vars["TableName"], searchInShared);

            //
            var select = table.Select();
            int CondCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();

            // setConditions
            for (int i = 0; i < CondCount; i++)
            {
                // none -> ==
                if (!vars.ContainsKey($"CondOperation[{i}]")) 
                    select.Where(c => c.Column((string)vars[$"CondColumn[{i.ToString()}]"]).Equal(vars[$"CondValue[{i.ToString()}]"]));
                else
                    switch ((string)vars[$"CondOperation[{i}]"])
                    {
                        case "Less":
                            select.Where(c => c.Column((string)vars[$"CondColumn[{i.ToString()}]"]).Less(vars[$"CondValue[{i.ToString()}]"]));
                            break;
                        case "LessOrEqual":
                            select.Where(c => c.Column((string)vars[$"CondColumn[{i.ToString()}]"]).LessOrEqual(vars[$"CondValue[{i.ToString()}]"]));
                            break;
                        case "Greater":
                            select.Where(c => c.Column((string)vars[$"CondColumn[{i.ToString()}]"]).Greater(vars[$"CondValue[{i.ToString()}]"]));
                            break;
                        case "GreaterOrEqual":
                            select.Where(c => c.Column((string)vars[$"CondColumn[{i.ToString()}]"]).GreaterOrEqual(vars[$"CondValue[{i.ToString()}]"]));
                            break;
                        default: // ==
                            select.Where(c => c.Column((string)vars[$"CondColumn[{i.ToString()}]"]).Equal(vars[$"CondValue[{i.ToString()}]"]));
                            break;
                    }
            }

            // return
            outputVars["Result"] = (select.Count() > 0);
        }
    }
}
