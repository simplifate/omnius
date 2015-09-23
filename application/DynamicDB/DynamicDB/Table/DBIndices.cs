using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DynamicDB
{
    class DBIndices
    {
        public DBTable table { get { return _table; } }
        private DBTable _table;
        private List<DBIndex> _indices;
        private int position = -1;

        public DBIndices(DBTable table)
        {
            _table = table;
            _indices = new List<DBIndex>();
        }

        public DBIndices Add(DBIndex index)
        {
            // ##!!## TODO

            return this;
        }
        public DBIndices Drop(DBIndex index)
        {
            // ##!!## TODO

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
