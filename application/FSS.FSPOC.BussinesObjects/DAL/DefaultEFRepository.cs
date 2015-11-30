using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FSS.Omnius.BussinesObjects.DAL
{
    // ReSharper disable once InconsistentNaming
    public class DefaultEFRepository<T> : IRepository<T>
                                        where T : class
    {
        private DBEntities DbContext { get; set; }

        public DefaultEFRepository (DBEntities context )
        {
            if ( context == null ) throw new ArgumentNullException ( nameof(context) );
            DbContext = context;
        }

        public void AddObject ( T newObject )
        {
            if ( newObject == null ) throw new ArgumentNullException ( nameof(newObject) );
            DbContext.Set<T> ( ).Add ( newObject );
        }

        public T FindObjectById ( object id )
        {
            if ( id == null ) throw new ArgumentNullException ( nameof(id) );
            return DbContext.Set<T> ( ).Find ( id );
        }

        public int GetObjectsCount ( )
        {
            return GetObjectsQuery ( ).Count ( );
        }

        public IEnumerable<T> GetObjectsList()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetObjectsQuery ( )
        {
            return DbContext.Set<T> ( );
        }

        public IQueryable<T> GetObjectsQueryWithPaging ( int startRow , int rowsInPage )
        {
            return GetObjectsQuery ( ).Skip ( startRow ).Take ( rowsInPage );
        }

        public void RemoveById ( int id )
        {
            var obj = FindObjectById ( id );
            RemoveObject ( obj );
        }

        public void RemoveObject ( T objToRemove )
        {
            if ( objToRemove == null ) throw new ArgumentNullException ( nameof(objToRemove) );
            DbContext.Set<T> ( ).Remove ( objToRemove );
        }

        public  IEnumerable<T> Get(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = DbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return orderBy != null ? orderBy(query).ToList() : query.ToList();
        }


    }
}
