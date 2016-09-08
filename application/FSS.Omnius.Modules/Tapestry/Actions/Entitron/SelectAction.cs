﻿using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Sql;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectAction : Action
    {
        public override int Id
        {
            get
            {
                return 1020;
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
                return new string[] { "TableName", "CondColumn[index]", "CondValue[index]", "?CondOperation[index]" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select (filter)";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBTable table = core.Entitron.GetDynamicTable(vars.ContainsKey("TableName") ? (string)vars["TableName"] : (string)vars["__TableName__"]);
            DBEntities e = new DBEntities();
            
            //
            var select = table.Select();
            int CondCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();
            Conditions condition = new Conditions(select);
            Condition_concat outCondition = null;

            // setConditions
            for (int i = 0; i < CondCount; i++)
            {
                string condOperator = (string)vars[$"CondOperator[{i}]"];
                string condColumn = (string)vars[$"CondColumn[{i}]"];
                object condValue = vars[$"CondValue[{i}]"];

                DBColumn column = table.columns.Single(c => c.Name == condColumn);
                var value = Convertor.convert(DataType.ByDBColumnTypeName(column.type), condValue);

                switch (condOperator)
                {
                    case "Less":
                        outCondition = condition.column(condColumn).Less(value);
                        break;
                    case "LessOrEqual":
                        outCondition = condition.column(condColumn).LessOrEqual(value);
                        break;
                    case "Greater":
                        outCondition = condition.column(condColumn).Greater(value);
                        break;
                    case "GreaterOrEqual":
                        outCondition = condition.column(condColumn).GreaterOrEqual(value);
                        break;
                    default: // ==
                        outCondition = condition.column(condColumn).Equal(value);
                        break;
                }
                condition = outCondition.and();
            }

            // return
            outputVars["Data"] = select.where(i => outCondition).ToList();
        }
    }
}
