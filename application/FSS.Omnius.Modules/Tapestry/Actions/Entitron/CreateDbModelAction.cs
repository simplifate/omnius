using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDbModelAction : Action
    {
        public override int Id
        {
            get
            {
                return 1009;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public override string Name
        {
            get
            {
                return "Create DB Model";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return 1010;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBTable table = core.Entitron.GetDynamicTable((string)vars["__TableName__"]);

            DBItem item = new DBItem();
            foreach(DBColumn column in table.columns)
            {
                if (column.type == "bit")
                    item.createProperty(column.ColumnId, column.Name, vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"));
                else if (vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"))
                    item.createProperty(column.ColumnId, column.Name, vars[$"__Model.{table.tableName}.{column.Name}"]);
            }
            
            table.Add(item);
            core.Entitron.Application.SaveChanges();
        }
    }
}
