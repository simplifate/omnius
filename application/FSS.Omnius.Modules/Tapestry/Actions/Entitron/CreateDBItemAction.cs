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
                return new string[] { "TableName", "Item[PropertyName]" };
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
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron ent = core.Entitron;
            DBItem item = new DBItem();
            DBTable table = ent.GetDynamicTable((string)vars["TableName"]);

            var propertyNames = vars.Keys.Where(k => k.StartsWith("Item[") && k.EndsWith("]"));
            foreach (string propertyName in propertyNames)
            {
                string itemProperty = propertyName.Substring(5, propertyName.Length - 6);
                item.createProperty(table.columns.Single(c => c.Name == itemProperty).ColumnId, itemProperty, vars[propertyName]);
            }

            table.Add(item);
            ent.Application.SaveChanges();
        }
    }
}
