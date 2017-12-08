using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Select : SqlQuery_Selectable<SqlQuery_Select>
    {
        public List<string> columns { get; set; }
        
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            sqlString =
                $"SELECT {_top} {((columns != null && columns.Count > 0) ? string.Join(",", columns.Select(c => $"[{c}]")) : "*")} FROM [{realTableName}] {_where} {string.Join(" ", _join)} {_group} {_order};";
            
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
                    int columnId = table != null ? table.columns.Single(x => x.Name == columnName).ColumnId : i;
                    newItem.createProperty(columnId, columnName, reader[columnName]);
                }

                items.Add(newItem);
            }

            return items;
        }

        public ListJson<DBItem> ToList()
        {
            ListJson<DBItem> output = ExecuteWithRead();
            
            foreach (DBItem item in output)
            {
                item.table = table;
            }
            return output;
        }
        public DBItem First()
        {
            DBItem output = ExecuteWithRead().First();
            output.table = table;

            return output;
        }
        public DBItem FirstOrDefault()
        {
            DBItem output = ExecuteWithRead().FirstOrDefault();
            if (output != null)
                output.table = table;

            return output;
        }
        public DBItem LastOrDefault()
        {
            DBItem output = ExecuteWithRead().LastOrDefault();
            if (output != null)
                output.table = table;

            return output;
        }
        public int Count()
        {
            return new SqlQuery_SelectCount(this).Count();
        }

        public override string ToString()
        {
            return string.Format("Select row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
