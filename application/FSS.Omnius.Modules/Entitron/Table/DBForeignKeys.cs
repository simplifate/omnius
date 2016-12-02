using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Sql;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBForeignKeys : List<DBForeignKey>
    {
        public DBTable table { get { return _table; } }
        private DBTable _table;
        public bool? isForDrop { get; set; }
        public DBForeignKeys(DBTable table)
        {
            _table = table;

            if (table.isInDB)
            {
                SqlQuery_SelectFogreignKeys query = new SqlQuery_SelectFogreignKeys() { application = table.Application, table = table,isForDrop = isForDrop};

                foreach (DBItem i in query.ExecuteWithRead())
                {
                    // is source table
                    if (table.tableName == (string)i["sourceTable"])
                    {
                        DBForeignKey fk = new DBForeignKey();

                        fk.name = (string)i["name"];
                        fk.sourceTable = table.Application.GetTable((string)i["sourceTable"]);
                        fk.targetTable = table.Application.GetTable((string)i["targetTable"]);
                        fk.sourceColumn = (string)i["sourceColumn"];
                        fk.targetColumn = (string)i["targetColumn"];
                        Add(fk);

                    }
                    // is target table
                    else
                    {
                        //fk.sourceTable = table.Application.GetTable((string)i["targetTable"]);
                        //fk.targetTable = table.Application.GetTable((string)i["sourceTable"]);
                        //fk.sourceColumn = (string)i["targetColumn"];
                        //fk.targetColumn = (string)i["sourceColumn"];
                    }

                }
            }
        }

        public DBForeignKeys AddToDB(DBForeignKey fk)
        {
            table.Application.queries.Add(new SqlQuery_ForeignKeyAdd()
            {
                application = table.Application,
                foreignName = fk.name,
                table = fk.sourceTable,
                table2 = fk.targetTable,
                foreignKey = fk.sourceColumn,
                primaryKey = fk.targetColumn,
                onDelete = fk.onDelete,
                onUpdate = fk.onUpdate
            });

            Add(fk);
            return this;
        }
        public List<string> GetForeignKeyForDrop()
        {
            SqlQuery_GetForeignKeysForDrop query = new SqlQuery_GetForeignKeysForDrop()
            {
                application = table.Application,
                table = table
            };
            List<string> fkList = new List<string>();

            foreach (DBItem i in query.ExecuteWithRead())
            {
                DBForeignKey fk = new DBForeignKey();
                fk.name = (string)i["name"];
                fkList.Add(fk.name);
            }
            return fkList;
        } 
    }
}
