using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entitron.Sql;

namespace Entitron
{
    public class DBColumns : IEnumerator, IEnumerable
    {
        public DBTable table { get { return _table; }  }
        private DBTable _table { get; set; }
        private List<DBColumn> _colums;
        private int position = -1;

        public DBColumns(DBTable table)
        {
            _table = table;
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

        public DBTable Add(DBColumn column)
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
            return _table;
        }
        public DBTable Add(string columnName, System.Data.SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
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
        public DBTable AddRange(DBColumns columns)
        {
            foreach(DBColumn column in columns)
            {
                Add(column);
            }

            return _table;
        }

        public DBTable Rename(string originColumnName, string newColumnName)
        {
            DBTable.queries.Add(new SqlQuery_Column_Rename()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                originColumnName = originColumnName,
                newColumnName = newColumnName
            });

            _colums.SingleOrDefault(c => c.Name == originColumnName).Name = newColumnName;
            return _table;
        }

        public DBTable Modify(DBColumn column)
        {
            new SqlQuery_Column_Modify()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                column = column
            };
            
            int index = _colums.IndexOf(c => c.Name == column.Name);
            _colums[index] = column;
            return _table;
        }
        public DBTable Modify(string columnName, System.Data.SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
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

        public DBTable Drop(string columnName)
        {
            DBTable.queries.Add(new SqlQuery_Column_Drop()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                columnName = columnName
            });

            _colums.Remove(_colums.SingleOrDefault(c => c.Name == columnName));
            return _table;
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
                if (_colums.Count == index)
                    _colums.Add(value); 

                else if (_colums.Count <= index)
                    throw new IndexOutOfRangeException();

                else _colums[index] = value;
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

        public List<T> Select<T>(Func<DBColumn,T> selection)
        {
            return _colums.Select(selection).ToList();
        }
        public DBColumn FirstOrDefault(Func<DBColumn, bool> selection)
        {
            return _colums.FirstOrDefault(selection);
        }
        #endregion
    }
}
