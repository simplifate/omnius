using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class TruncateAction : Action
    {
        public override int Id => 1046;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName" };

        public override string Name => "Truncate table";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            DBConnection db = Modules.Entitron.Entitron.i;
            string tableName = (string)vars["TableName"];

            // return
            db.TableTruncate(tableName);
            outputVars["Result"] = true;
        }
    }
}
