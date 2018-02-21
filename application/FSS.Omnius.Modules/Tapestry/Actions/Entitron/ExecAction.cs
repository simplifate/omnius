using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ExecAction : Action
    {
        public override int Id => 1045;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "ProcedureName", "?paramName[index]", "?paramValue[index]" };

        public override string Name => "EXEC SP";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            DBConnection db = Modules.Entitron.Entitron.i;
            
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
            outputVars["Result"] = db.ExecSP(procedureName, parameters);
        }
    }
}
