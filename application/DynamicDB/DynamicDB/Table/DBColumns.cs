using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DynamicDB.Sql;

namespace DynamicDB
{
    public class DBColumns : IEnumerator,IEnumerable
    {
        public DBTable table { get; }
        private List<DBColumn> _colums;
        private int position = -1;

        public DBColumns(DBTable table)
        {
            this.table = table;
            _colums = new List<DBColumn>();

            SqlQuery_Select_ColumnList query = new SqlQuery_Select_ColumnList() { applicationName = table.AppName, tableName = table.tableName };
            List<DBItem> items = query.ExecuteWithRead();

            _colums = items.Select(i => new DBColumn()
            {
                Name = (string)i["name"],
                type = (System.Data.SqlDbType)Enum.Parse(typeof(System.Data.SqlDbType), (string)i["typeName"], true),
                maxLength = Convert.ToInt32((Int16)i["max_length"]),
                canBeNull = (bool)i["is_nullable"]
            }).ToList();
        }

        public DBColumns Add(DBColumn column)
        {
            SqlQuery_Table_Create query;
            if ((query = DBTable.queries.GetCreate(table.tableName)) != null)
                query.AddColumn(column);
            else
                DBTable.queries.Add(new SqlQuery_Column_Add()
                {
                    applicationName = table.AppName,
                    tableName = table.tableName,
                    column = column
                });

            _colums.Add(column);
            return this;
        }
        public DBColumns Add(string columnName, System.Data.SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
        {
            return Add(new DBColumn()
            {
                Name = columnName,
                type = type,
                maxLength = maxLength,
                canBeNull = canBeNull,
                additionalOptions = additionalOptions
            });
        }
        public DBColumns AddRange(DBColumns columns)
        {
            foreach(DBColumn column in columns)
            {
                Add(column);
            }

            return this;
        }

        public DBColumns Rename(string originColumnName, string newColumnName)
        {
            DBTable.queries.Add(new SqlQuery_Column_Rename()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                originColumnName = originColumnName,
                newColumnName = newColumnName
            });

            _colums.SingleOrDefault(c => c.Name == originColumnName).Name = newColumnName;
            return this;
        }

        public DBColumns Modify(DBColumn column)
        {
            new SqlQuery_Column_Modify()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                column = column
            };
            
            int index = _colums.IndexOf(c => c.Name == column.Name);
            _colums[index] = column;
            return this;
        }
        public DBColumns Modify(string columnName, System.Data.SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
        {
            return Modify(new DBColumn()
            {
                Name = columnName,
                type = type,
                maxLength = maxLength,
                canBeNull = canBeNull,
                additionalOptions = additionalOptions
            });
        }

        public DBColumns Drop(string columnName)
        {
            DBTable.queries.Add(new SqlQuery_Column_Drop()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                columnName = columnName
            });

            _colums.Remove(c => c.Name == columnName);
            return this;
        }

        #region IEnum
        public DBColumn this[int index]
        {
            get
            {
                if (_colums.Count <= index)
                    throw new IndexOutOfRangeException();

                return _colums[index];
            }
            set
            {
                if (_colums.Count <= index)
                    throw new IndexOutOfRangeException();

                _colums[index] = value;
            }
        }
        
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        public bool MoveNext()
        {
            position++;
            return (position < _colums.Count);
        }
        public void Reset()
        {
            position = 0;
        }
        public object Current
        {
            get { return _colums[position]; }
        }
        #endregion
    }
}
