using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class MassEditAction : Action
    {
        public override int Id
        {
            get
            {
                return 1035;
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
                return new string[] { "TableName", "ColumnName", "Value", "?IdList", "?TableData", "?ValueType" };
            }
        }

        public override string Name
        {
            get
            {
                return "Mass edit";
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

            DBTable table = ent.GetDynamicTable((string)vars["TableName"]);
            string targetColumnName = (string)vars["ColumnName"];
            object targetValue;
            if (vars.ContainsKey("ValueType") && (string)vars["ValueType"] == "string")
                targetValue = (string)vars["Value"];
            else
                targetValue = Convert.ToInt32(vars["Value"]);
            var idList = new List<object>();
            List<DBItem> results = null;
            if (vars.ContainsKey("TableData"))
            {
                if (vars["TableData"] is DBItem)
                {
                    var rowList = new List<DBItem>();
                    rowList.Add((DBItem)vars["TableData"]);
                }
                else
                {
                    results = (List<DBItem>)vars["TableData"];
                }
            }
            else if (vars.ContainsKey("IdList"))
            {
                idList = ((string)vars["IdList"]).Split(',').Select(int.Parse).Cast<object>().ToList();
                results = table.Select().where(c => c.column("id").In(idList)).ToList();
            }
            else
            {
                //results = table.Select().ToList();
            }
            foreach (var row in results)
            {
                row[targetColumnName] = targetValue;
                table.Update(row, (int)row["id"]);
            }
            ent.Application.SaveChanges();
        }
    }
}
