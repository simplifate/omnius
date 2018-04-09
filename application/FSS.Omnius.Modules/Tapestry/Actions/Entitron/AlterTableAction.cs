using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using System;
using FSS.Omnius.Modules.Entitron.Service;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class AlterTableAction : Action
    {
        public override int Id => 102319;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "ColumnName[index]", "ColumnType[index]", "ColumnLength[index]" };

        public override string Name => "Alter Table";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            DBConnection db = Modules.Entitron.Entitron.i;
            int ColumnCount = vars.Keys.Where(k => k.StartsWith("ColumnName[") && k.EndsWith("]")).Count();

            //find table we want to alter and delete
            var lastCommit = db.Application.DatabaseDesignerSchemeCommits.Last();
            var tbl = lastCommit.Tables.SingleOrDefault(t => t.Name == (string)vars["TableName"]);
            var columnList = tbl.Columns.ToList();

            foreach(var dbc in columnList)
            {
                tbl.Columns.Remove(dbc);
            }

            //create id by default
            DbColumn newColumn = new DbColumn
            {
                Name = "id",
                Type = "int",
                PrimaryKey = true
            };
            tbl.Columns.Add(newColumn);

            //iterate columns
            for (int i = 0; i < ColumnCount; i++)
            {
                string ColumnName = (string)vars[$"ColumnName[{i}]"];
                string ColumnType = (string)vars[$"ColumnType[{i}]"];
                if (ColumnName.ToLower() != "id") //we create id by default
                {
                    //create column entity
                    DbColumn Column = new DbColumn
                    {
                        Name = ColumnName,
                        DisplayName = ColumnName,
                        Type = ColumnType,
                        PrimaryKey = false,
                        AllowNull = true,
                        Unique = false,
                    };
                    if (ColumnType == "nvarchar" || ColumnType == "varchar")
                    {
                        if (vars.ContainsKey($"ColumnLength[{i}]"))
                            Column.ColumnLength = (int)vars[$"ColumnLength[{i}]"];
                        else
                            Column.ColumnLengthIsMax = true;
                    }
                    tbl.Columns.Add(Column);

                }
                //add co to tbl
            }

            lastCommit.Timestamp = DateTime.UtcNow;
            lastCommit.CommitMessage = "alter table";
            db.Application.DatabaseDesignerSchemeCommits.Add(lastCommit);

            db.SaveChanges();

            new DatabaseGenerateService().GenerateDatabase(lastCommit, (CORE.CORE)vars["__CORE__"], x => { });

            // return
        }
    }
}
