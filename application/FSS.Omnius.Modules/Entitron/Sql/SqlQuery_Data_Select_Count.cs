using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Data_Select_Count : SqlQuery_Selectable<SqlQuery_Data_Select>
    {
        public SqlQuery_Data_Select_Count()
        { }
        public SqlQuery_Data_Select_Count(SqlQuery_Data_Select selectQuery)
        {
            application = selectQuery.application;
            tabloid = selectQuery.tabloid;
            _datatypes = selectQuery._datatypes;
            _group = selectQuery._group;
            _join = selectQuery._join;
            _order = selectQuery._order;
            _params = selectQuery._params;
            _where = selectQuery._where;
        }

        protected override string CreateString()
        {
            return
                $"SELECT COUNT(*) count FROM [{realTabloidName}] {_where} {string.Join(" ", _join)} {_group} {_order};";
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
                    DBColumn column = tabloid.columns.SingleOrDefault(x => x.Name == columnName);
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
            return $"[{application.Name}:{tabloid.Name}] Select count";
        }
    }
}
