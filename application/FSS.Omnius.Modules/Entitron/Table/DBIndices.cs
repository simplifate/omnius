using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Entitron.Sql;

namespace FSS.Omnius.Entitron
{
    public class DBIndices : List<DBIndex>
    {
        public DBTable table { get { return _table; } }
        private DBTable _table;

        public DBIndices(DBTable table)
        {
            _table = table;

            if (_table.isInDB())
            {
                SqlQuery_SelectIndexes query = new SqlQuery_SelectIndexes() { application = table.Application, table = table };

                foreach (DBItem i in query.ExecuteWithRead())
                {
                    if (i["IndexName"].GetType() != typeof(DBNull))
                    {
                        DBIndex index = new DBIndex()
                        {
                            table = _table,
                            indexName = (string)i["IndexName"]
                        };
                        Add(index);
                    }
                }
            }
        }

        public DBIndices AddToDB(string indexName, List<string> columns)
        {
            table.Application.queries.Add(new SqlQuery_IndexCreate()
            {
                application = table.Application,
                columnsName = columns,
                table = table,
                indexName = indexName
            });

            Add(new DBIndex() { table = _table, indexName = indexName });
            return this;
        }
        public DBIndices DropFromDB(string indexName)
        {
            table.Application.queries.Add(new SqlQuery_IndexDrop()
            {
                application = table.Application,
                table = table,
                indexName = indexName
            });

            Remove(this.SingleOrDefault(i => i.indexName == indexName));
            return this;
        }
    }
}
