using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SetStateAction : Action
    {
        public override int Id
        {
            get
            {
                return 1029;
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
                return new string[] { "ColumnName", "StateId", "?RowId" };
            }
        }

        public override string Name
        {
            get
            {
                return "Set state";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron ent = core.Entitron;
            if (vars.ContainsKey("__TableName__") && (vars.ContainsKey("__ModelId__") || vars.ContainsKey("RowId")))
            {
                int rowId = vars.ContainsKey("RowId") ? (int)vars["RowId"] : (int)vars["__ModelId__"];
                DBItem model = ent.GetDynamicItem((string)vars["__TableName__"], rowId);
                model[(string)vars["ColumnName"]] = (int)vars["StateId"];
                model.table.Update(model, rowId);
                ent.Application.SaveChanges();
            }
            else
            {
                DBItem model = (DBItem)vars["__MODEL__"];
                model[(string)vars["ColumnName"]] = (int)vars["StateId"];
            }
        }
    }
}
