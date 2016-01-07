using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class AnyItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 1021;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName", "CondColumn[index]", "CondValue[index]", "?CondOperation[index]" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Any?" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBTable table = core.Entitron.GetDynamicTable((string)vars["TableName"]);

            //
            var select = table.Select();
            int CondCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();
            Conditions condition = new Conditions(select);
            Condition_concat outCondition = null;

            // setConditions
            for (int i = 0; i < CondCount; i++)
            {
                // none -> ==
                if (!vars.ContainsKey($"CondOperation[{i}]"))
                    outCondition = condition.column((string)vars[$"CondColumn[{i.ToString()}]"]).Equal(vars[$"CondValue[{i.ToString()}]"]);
                else
                    switch ((string)vars[$"CondOperation[{i}]"])
                    {
                        case "Less":
                            outCondition = condition.column((string)vars[$"CondColumn[{i.ToString()}]"]).Less(vars[$"CondValue[{i.ToString()}]"]);
                            break;
                        case "LessOrEqual":
                            outCondition = condition.column((string)vars[$"CondColumn[{i.ToString()}]"]).LessOrEqual(vars[$"CondValue[{i.ToString()}]"]);
                            break;
                        case "Greater":
                            outCondition = condition.column((string)vars[$"CondColumn[{i.ToString()}]"]).Greater(vars[$"CondValue[{i.ToString()}]"]);
                            break;
                        case "GreaterOrEqual":
                            outCondition = condition.column((string)vars[$"CondColumn[{i.ToString()}]"]).GreaterOrEqual(vars[$"CondValue[{i.ToString()}]"]);
                            break;
                        default: // ==
                            outCondition = condition.column((string)vars[$"CondColumn[{i.ToString()}]"]).Equal(vars[$"CondValue[{i.ToString()}]"]);
                            break;
                    }

                condition = outCondition.and();
            }

            // return
            vars["Any?"] = select.where(i => outCondition).Count();
        }
    }
}
