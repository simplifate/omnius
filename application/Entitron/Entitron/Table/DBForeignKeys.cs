using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitron.Sql;

namespace Entitron
{
    public class DBForeignKeys : List<DBForeignKey>
    {
        public DBTable table { get { return _table; } }
        private DBTable _table;

        public DBForeignKeys(DBTable table)
        {
            _table = table;

            if (_table.isInDB())
            {
                SqlQuery_SelectFogreignKeys query = new SqlQuery_SelectFogreignKeys() { application = table.Application, table = table };

                foreach (DBItem i in query.ExecuteWithRead())
                {
                    DBForeignKey fk = new DBForeignKey();
                    fk.name = (string)i["name"];

                    // is source table
                    if (table.tableName == (string)i["sourceTable"])
                    {
                        fk.sourceTable = table.Application.GetTable((string)i["sourceTable"]);
                        fk.targetTable = table.Application.GetTable((string)i["targetTable"]);
                        fk.sourceColumn = (string)i["sourceColumn"];
                        fk.targetColumn = (string)i["targetColumn"];
                    }
                    // is target table
                    else
                    {
                        fk.sourceTable = table.Application.GetTable((string)i["targetTable"]);
                        fk.targetTable = table.Application.GetTable((string)i["sourceTable"]);
                        fk.sourceColumn = (string)i["targetColumn"];
                        fk.targetColumn = (string)i["sourceColumn"];
                    }

                    Add(fk);
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
        public DBForeignKeys DropFromDB(string fkName)
        {
            table.Application.queries.Add(new SqlQuery_ForeignKeyDrop()
            {
                application = table.Application,
                table = table,
                foreignKeyName = fkName
            });

            Remove(this.SingleOrDefault(i => i.name == fkName));
            return this;
        }
    }
}
