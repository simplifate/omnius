using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DynamicDB.Sql;

namespace DynamicDB
{
    public class DBIndices
    {
        public DBTable table { get { return _table; } }
        private DBTable _table;
        private List<DBIndex> _indices;
        private int position = -1;

        public DBIndices(DBTable table)
        {
            _table = table;
            _indices = new List<DBIndex>();

            SqlQuery_SelectIndexes query = new SqlQuery_SelectIndexes() { applicationName = table.AppName, tableName = table.tableName };
            List<DBItem> items = query.ExecuteWithRead();

            _indices = items.Select(i => new DBIndex()
            {
                indexName = (string)i["IndexName"]
            }).ToList();
        }

        public DBIndices Add(string indexName, List<string> columns)
        {
            DBTable.queries.Add(new SqlQuery_IndexCreate()
            {
                applicationName = table.AppName,
                columnsName = columns,
                tableName = table.tableName,
                indexName = indexName
            });

            return this;
        }
        public DBIndices Drop(string indexName)
        {
            DBTable.queries.Add(new SqlQuery_IndexDrop()
            {
                applicationName = table.AppName,
                tableName = table.tableName,
                indexName = indexName
            });

            return this;
        }

        #region IEnum
        public DBIndex this[int index]
        {
            get
            {
                if (_indices.Count <= index)
                    throw new IndexOutOfRangeException();

                return _indices[index];
            }
            set
            {
                if (_indices.Count <= index)
                    throw new IndexOutOfRangeException();

                _indices[index] = value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        public bool MoveNext()
        {
            position++;
            return (position < _indices.Count);
        }
        public void Reset()
        {
            position = 0;
        }
        public object Current
        {
            get { return _indices[position]; }
        }
        #endregion
    }
}
