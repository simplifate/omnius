using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class MassEditAction : Action
    {
        public override int Id => 1035;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "ColumnName", "Value", "?IdList", "?TableData", "?ValueType", "?SearchInShared" };

        public override string Name => "Mass edit";

        public override string[] OutputVar => new string[0];

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            DBTable table = db.Table((string)vars["TableName"], searchInShared);
            string targetColumnName = (string)vars["ColumnName"];
            object targetValue = 0;
            if (vars.ContainsKey("ValueType"))
            {
                if( (string)vars["ValueType"] == "string")
                    targetValue = (string)vars["Value"];
                if ((string)vars["ValueType"] == "datetime")
                    targetValue = DateTime.ParseExact((string)vars["Value"], "dd.MM.yyyy",CultureInfo.InvariantCulture);
            }
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
                    results = rowList;
                }
                else
                {
                    results = (List<DBItem>)vars["TableData"];
                }
            }
            else if (vars.ContainsKey("IdList"))
            {
                idList = ((string)vars["IdList"]).Split(',').Select(int.Parse).Cast<object>().ToList();
                results = table.Select().Where(c => c.Column(DBCommandSet.PrimaryKey).In(idList)).ToList();
            }
            else
            {
                //results = table.Select().ToList();
            }
            foreach (var row in results)
            {
                row[targetColumnName] = targetValue;
                table.Update(row, (int)row[DBCommandSet.PrimaryKey]);
            }
            db.SaveChanges();
        }
    }
}
