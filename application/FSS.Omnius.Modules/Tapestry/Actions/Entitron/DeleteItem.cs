using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class DeleteItemAction : Action
    {
        public override int Id
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
                return new string[] { "ItemId", "TableName" };
            }
        }

        public override string Name
        {
            get
            {
                return "DeleteItem";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            Modules.Entitron.Entitron ent = core.Entitron;
            ent.GetDynamicTable((string)vars["TableName"]).Remove((int)vars["ItemId"]);
            ent.Application.SaveChanges();
        }
    }
}
