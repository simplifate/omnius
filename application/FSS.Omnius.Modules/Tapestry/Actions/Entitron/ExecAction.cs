using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Sql;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ExecAction : Action
    {
        public override int Id
        {
            get
            {
                return 1045;
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
                return new string[] { "ProcedureName", "?paramName[index]", "?paramValue[index]" };
            }
        }

        public override string Name
        {
            get
            {
                return "EXEC SP";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            int paramsCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();
            string procedureName = (string)vars["ProcedureName"];

            // set params
            for (int i = 0; i < paramsCount; i++) {
                string paramName = (string)vars[$"paramName[{i}]"];
                string paramValue = (string)vars[$"paramValue[{i}]"];
                parameters.Add(paramName, paramValue);
            }

            // return
            outputVars["Result"] = core.Entitron.ExecSP(procedureName, parameters);
        }
    }
}
