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
    public class CreateTableAction : Action
    {
        public override int Id => 102399;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "ColumnName[index]", "ColumnType[index]","ColumnLength[index]"};

        public override string Name => "Create Table";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;
            //
            int ColumnCount = vars.Keys.Where(k => k.StartsWith("ColumnName[") && k.EndsWith("]")).Count();

            //create table entity
            DbTable newTable = new DbTable { Name = (string)vars["TableName"], PositionX = 0, PositionY = 0 };

            //create id by default
            DbColumn newColumn = new DbColumn
            {
                Name = "id",
                Type = "int",
                PrimaryKey = true
            };
            newTable.Columns.Add(newColumn);

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
                        Unique = false
                    };
                    if(ColumnType == "nvarchar" || ColumnType == "varchar")
                    {
                        if (vars.ContainsKey($"ColumnLength[{i}]"))
                            Column.ColumnLength = (int)vars[$"ColumnLength[{i}]"];
                        else
                            Column.ColumnLengthIsMax = true;
                    }
                    newTable.Columns.Add(Column);

                }
                //add co to tbl
            }

            var lastCommit = db.Application.DatabaseDesignerSchemeCommits.Last();
            lastCommit.Tables.Add(newTable);
            lastCommit.Timestamp = DateTime.UtcNow;
            lastCommit.CommitMessage = "add table";
            db.Application.DatabaseDesignerSchemeCommits.Add(lastCommit);

            db.SaveChanges();

            new DatabaseGenerateService(null).GenerateDatabase(lastCommit,(COREobject)vars["__CORE__"]);

            // return
        }
    }
}
