using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_SelectCount : SqlQuery_Selectable<SqlQuery_Select>
    {
        public SqlQuery_SelectCount()
        { }
        public SqlQuery_SelectCount(SqlQuery_Select selectQuery)
        {
            application = selectQuery.application;
            table = selectQuery.table;
            _datatypes = selectQuery._datatypes;
            _group = selectQuery._group;
            _join = selectQuery._join;
            _order = selectQuery._order;
            _params = selectQuery._params;
            _where = selectQuery._where;
        }

        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(100),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('SELECT COUNT(*) count FROM [', @realTableName, '] {2} {3} {4} {5};');" +
                "exec sp_executesql @sql, N'{6}', {7};", 
                parAppName,parTableName,
                _where.ToString(),
                string.Join(" ", _join),
                _group,
                _order,
                string.Join(", ", _datatypes.Select(d => "@" + d.Key + " " + d.Value)),
                string.Join(", ", _datatypes.Select(d => "@" + d.Key))
                );
            
            return base.BaseExecutionWithRead(connection);
        }

        protected override ListJson<DBItem> Read(SqlDataReader reader)
        {
            ListJson<DBItem> items = new ListJson<DBItem>();

            while (reader.Read())
            {
                DBItem newItem = new DBItem();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    // sloupec, kt. není součástí tabulky (as, count, ...)
                    DBColumn column = table.columns.SingleOrDefault(x => x.Name == columnName);
                    int columnId = column != null ? column.ColumnId : -1;
                    newItem.createProperty(columnId, columnName, reader[columnName]);
                }

                items.Add(newItem);
            }

            return items;
        }

        public int Count()
        {
            List<DBItem> result = ExecuteWithRead();
            return (int)result.First()["count"];
        }

        public override string ToString()
        {
            return string.Format("Select row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
