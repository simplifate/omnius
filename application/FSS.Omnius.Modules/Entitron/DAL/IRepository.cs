using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FSS.Omnius.Modules.Entitron.DAL
{
    public interface IRepository<T> where T : class
    {
        void AddObject ( T newObject );

        T FindObjectById ( object id );


        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");


        int GetObjectsCount ( );

        IEnumerable<T> GetObjectsList ( );

        IQueryable<T> GetObjectsQuery ( );

        IQueryable<T> GetObjectsQueryWithPaging ( int startRow , int rowsInPage );

        void RemoveById ( int id );

        void RemoveObject ( T obj );
    }
}