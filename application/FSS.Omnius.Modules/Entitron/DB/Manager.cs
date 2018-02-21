using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.DB
{
    // column, index, foreign key
    public class EntityManager<T> : ICollection<T>, IEnumerable<T> where T : IEntityMultiple<T>
    {
        public EntityManager(DBConnection db, Tabloid tabloid)
        {
            _db = db;
            _tabloid = tabloid;
        }

        private Tabloid _tabloid;
        private DBConnection _db;
        private HashSet<T> parts
        {
            get
            {
                if (_parts == null)
                    _parts = (Activator.CreateInstance(typeof(T), _db) as IEntityMultiple<T>).Load(_tabloid);

                return _parts;
            }
        }
        private HashSet<T> _parts;

        public int Count => parts.Count;

        public bool IsReadOnly => true;

        public void Add(T item)
        {
            // db
            item.AddToDB();

            // collection
            parts.Add(item);
        }

        public void Clear()
        {
            // db
            foreach(T part in parts)
            {
                part.RemoveFromDB();
            }

            // collection
            parts.Clear();
        }

        public bool Contains(T item)
        {
            return parts.Any(i => i.Compare(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            parts.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return parts.GetEnumerator();
        }

        public bool Remove(T item)
        {
            // db
            item.RemoveFromDB();

            // collection
            return parts.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
