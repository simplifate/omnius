using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class GetTableAction : Action
    {
        public override int Id
        {
            get
            {
                return 1003;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName" };
            }
        }

        public override string Name
        {
            get
            {
                return "GetTable";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }

        public override ActionResultCollection run(Dictionary<string, object> vars)
        {
            var outputVars = new Dictionary<string, object>();
            var outputStatus = ActionResultType.Success;
            var outputMessage = "OK";

            try
            {
                CORE.CORE core = (CORE.CORE)vars["__CORE__"];
                var table = core.Entitron.GetDynamicTable((string)vars["TableName"]);
                outputVars["Data"] = table.Select().ToList();
            }
            catch (Exception ex)
            {
                outputStatus = ActionResultType.Error;
                outputMessage = ex.Message;
            }

            return new ActionResultCollection(outputStatus, outputMessage, outputVars);
        }
    }
}
