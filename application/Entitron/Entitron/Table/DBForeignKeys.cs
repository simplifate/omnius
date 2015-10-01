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
                DBForeignKey fk = new DBForeignKey()
                {
                    name = (string)i["name"],
                    sourceTable = (string)i["sourceTable"],
                    targetTable = (string)i["targetTable"],
                    sourceColumn = (string)i["sourceColumn"],
                    targetColumn = (string)i["targetColumn"]
                };
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
                primaryKey = fk.targetColumn
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
