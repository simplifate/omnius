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

            SqlQuery_SelectFogreignKeys query = new SqlQuery_SelectFogreignKeys() { applicationName = table.AppName, tableName = table.tableName };

            foreach (DBItem i in query.ExecuteWithRead())
            {
                DBForeignKey fk = new DBForeignKey();
                fk.name = (string)i["name"];

                // is source table
                if (table.tableName == (string)i["sourceTable"])
                {
                    fk.sourceTable = (string)i["sourceTable"];
                    fk.targetTable = (string)i["targetTable"];
                    fk.sourceColumn = (string)i["sourceColumn"];
                    fk.targetColumn = (string)i["targetColumn"];
                }
                // is target table
                else
                {
                    fk.sourceTable = (string)i["targetTable"];
                    fk.targetTable = (string)i["sourceTable"];
                    fk.sourceColumn = (string)i["targetColumn"];
                    fk.targetColumn = (string)i["sourceColumn"];
                }

                Add(fk);
            }
        }

        public DBForeignKeys AddToDB(DBForeignKey fk)
        {
            DBTable.queries.Add(new SqlQuery_ForeignKeyAdd()
            {
                applicationName = table.AppName,
                foreignName = fk.name,
                tableName = fk.sourceTable,
                table2Name = fk.targetTable,
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
            DBTable.queries.Add(new SqlQuery_ForeignKeyDrop()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                foreignKeyName = fkName
            });

            Remove(this.SingleOrDefault(i => i.name == fkName));
            return this;
        }
    }
}
