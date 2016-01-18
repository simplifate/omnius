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

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars)
        {
            DBItem model = (DBItem)vars["__MODEL__"];
            outputVars["Data"] = model[(int)vars["ColumnId"]];
        }
    }
}
