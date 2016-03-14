using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Sql;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBIndices : List<DBIndex>
    {
        public DBTable table { get { return _table; } }
        private DBTable _table;

        public DBIndices(DBTable table)
        {
            _table = table;

            if (DBTable.isInDB(table.Application.Name, table.tableName))
            {
                SqlQuery_SelectIndexes query = new SqlQuery_SelectIndexes() { application = table.Application, table = table };

                foreach (DBItem i in query.ExecuteWithRead())
                {
                    if (i["IndexName"].GetType() != typeof(DBNull))
                    {
                        DBIndex index = new DBIndex()
                        {
                            table = _table,
                            indexName = (string)i["IndexName"],
                        };

                        if (i["isUnique"].ToString() == "" || i["isUnique"].ToString() == "False")
                        {
                            index.isUnique = false;
                        }
                        else
                        {
                            index.isUnique = true;
                        }

                        SqlQuery_IndexColumns query2 = new SqlQuery_IndexColumns() { indexName = index.indexName, table = table, application = table.Application};
                        if (query2.ExecuteWithRead().Count > 0)
                        {
                            index.columns = new List<DBColumn>();
                            foreach (DBItem item in query2.ExecuteWithRead())
                            {
                                index.columns.Add(table.columns.SingleOrDefault(x => x.Name == Convert.ToString(item["ColName"])));
                            }
                        }
                        Add(index);
                    }
                }
            }
        }

        public DBIndices AddToDB(string indexName, List<string> columns, bool isUnique)
        {
            table.Application.queries.Add(new SqlQuery_IndexCreate()
            {
                application = table.Application,
                columnsName = columns,
                table = table,
                indexName = indexName,
                isUniqueIndex = isUnique
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
