using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    public class Manager<T> : IEnumerable<T> where T : IMultiple
    {
        public Manager(DBConnection db)
        {
            _db = db;
            i = (T)Activator.CreateInstance(typeof(T), _db);
        }

        private DBConnection _db;
        private List<T> _parts = new List<T>();
        public T i { get; set; }

        public void Start()
        {
            i.Start(!_parts.Any());
        }
        public void Next()
        {
            _parts.Add(i);
            i = (T)Activator.CreateInstance(typeof(T), _db);
        }

        public string ToSql(DBCommandSet set, IDbCommand command)
        {
            return string.Join(i.Separator, _parts.Select(p => p.ToSql(set, command)));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
