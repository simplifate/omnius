using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectFromViewAction : Action
    {
        public override int Id
        {
            get
            {
                return 1034;
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
                return new string[] { "ViewName", "ColumnName", "Value", "?ColumnName2", "?Value2" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select from view";
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
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            if (core.Entitron.Application == null)
                core.Entitron.AppName = "EvidencePeriodik";

            var view = core.Entitron.GetDynamicView((string)vars["ViewName"]);
            if (!vars.ContainsKey("ColumnName"))
            {
                outputVars["Result"] = view.Select().ToList();
            }
            else if (vars.ContainsKey("ColumnName2"))
            {
                string columnName = (string)vars["ColumnName"];
                string columnName2 = (string)vars["ColumnName2"];
                outputVars["Result"] = view.Select().where(c => c.column(columnName).Equal(vars["Value"]).and().column(columnName2).Equal(vars["Value2"])).ToList();
            }
            else
            {
                string columnName = (string)vars["ColumnName"];
                outputVars["Result"] = view.Select().where(c => c.column(columnName).Equal(vars["Value"])).ToList();
            }
        }
    }
}
