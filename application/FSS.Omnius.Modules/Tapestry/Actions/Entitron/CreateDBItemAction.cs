using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDBItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 1006;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "ApplicationName", "TableName", "Item[PropertyName]" };
            }
        }

        public override string Name
        {
            get
            {
                return "CreateDBItem";
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
            DBItem item = new DBItem();

            var propertyNames = vars.Keys.Where(k => k.StartsWith("Item[") && k.EndsWith("]"));
            foreach (string propertyName in propertyNames)
            {
                string itemProperty = propertyName.Substring(5, propertyName.Length - 6);
                item[itemProperty] = vars[propertyName];
            }

            Modules.Entitron.Entitron ent = core.Entitron;
            ent.AppName = (string)vars["ApplicationName"];
            ent.GetDynamicTable((string)vars["TableName"]).Add(item);
            ent.Application.SaveChanges();
        }
    }
}
