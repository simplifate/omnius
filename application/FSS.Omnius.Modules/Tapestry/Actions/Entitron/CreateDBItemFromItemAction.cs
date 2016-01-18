using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDBItemFromItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 1004;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return 1010;

            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName", "Item" };
            }
        }

        public override string Name
        {
            get
            {
                return "CreateDBItemFromItem";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBItem item = (DBItem)vars["Item"];

            Modules.Entitron.Entitron ent = core.Entitron;
            ent.GetDynamicTable((string)vars["TableName"]).Add(item);
            ent.Application.SaveChanges();
        }
    }
}
