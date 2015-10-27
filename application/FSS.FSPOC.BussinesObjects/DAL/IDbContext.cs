using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace FSS.FSPOC.BussinesObjects.DAL
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();
        IEnumerable<TElement> SqlQuery<TElement>(string query, params object[] parameters);
        void ExecuteSqlCommand(string query, params object[] parameters);
        DbSet<T> Set<T>() where T : class;
    }
}