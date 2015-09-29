using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entitron.Sql;

namespace Entitron
{
    public class DBIndices : List<DBIndex>
    {
        public DBTable table { get { return _table; } }
        private DBTable _table;

        public DBIndices(DBTable table)
        {
            _table = table;

            SqlQuery_SelectIndexes query = new SqlQuery_SelectIndexes() { applicationName = table.AppName, tableName = table.tableName };

            foreach (DBItem i in query.ExecuteWithRead())
            {
                DBIndex index = new DBIndex()
                {
                    table = _table,
                    indexName = (string)i["IndexName"]
                };
                Add(index);
            }
        }

        public DBIndices AddToDB(string indexName, List<string> columns)
        {
            DBTable.queries.Add(new SqlQuery_IndexCreate()
            {
                applicationName = table.AppName,
                columnsName = columns,
                tableName = table.tableName,
                indexName = indexName
            });

            Add(new DBIndex() { table = _table, indexName = indexName });
            return this;
        }
        public DBIndices DropFromDB(string indexName)
        {
            DBTable.queries.Add(new SqlQuery_IndexDrop()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                indexName = indexName
            });

            Remove(this.SingleOrDefault(i => i.indexName == indexName));
            return this;
        }
    }
}
