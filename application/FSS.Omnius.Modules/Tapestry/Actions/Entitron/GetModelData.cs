using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetModelData : Action
    {
        public override int Id
        {
            get
            {
                return 1002;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "ColumnId" };
            }
        }

        public override string Name
        {
            get
            {
                return "GetModelData";
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
                DBItem model = (DBItem)vars["__MODEL__"];
                outputVars["Data"] = model[(int)vars["ColumnId"]];
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
