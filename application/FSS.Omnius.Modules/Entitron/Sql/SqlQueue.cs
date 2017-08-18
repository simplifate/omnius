using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using FSS.Omnius.Modules.Entitron;
using Logger;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQueue
    {
        private static readonly object _queryLock = new object();
        internal static List<SqlQuery> _queries = new List<SqlQuery>();

        public SqlQueue Add(SqlQuery query)
        {
            lock (_queryLock)
            {
                _queries.Add(query);
            }

            return this;
        }
        public T GetQuery<T>(string tableName) where T : SqlQuery_withAppTable
        {
            lock (_queryLock)
            {
                return (T)_queries.FirstOrDefault(q => q is T && (q as T).table.tableName == tableName);
            }
        }
        public List<T> GetAndRemoveQueries<T>(string tableName) where T : SqlQuery_withAppTable
        {
            List<T> output = new List<T>();
            lock (_queryLock)
            {
                for (int i = 0; i < _queries.Count; i++)
                {
                    SqlQuery q = _queries[i];
                    if (q is T && (q as T).table.tableName == tableName)
                    {
                        output.Add((T)q);
                        _queries.RemoveAt(i);
                        i--;
                    }
                }
            }

            return output;
        }

        public void ExecuteAll(string connectionString = null)
        {
            connectionString = connectionString ?? Entitron.connectionString;
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        lock (_queryLock)
                        {
                            foreach (SqlQuery query in _queries)
                            {
                                query.PreExecution();
                                query.Execute(transaction);
                            }
                            transaction.Commit();
                            _queries = new List<SqlQuery>();
                        }
                    }
                    catch (SqlException e)
                    {
                        Log.Error(string.Format("Entitron: sql query '{0}' could not be executed!", ToString()));
                        transaction.Rollback();
                        lock (_queryLock)
                        {
                            _queries = new List<SqlQuery>();
                        }
                        throw e;
                    }
                    catch (Exception e)
                    {
                        Log.Error(string.Format("Entitron: sql query '{0}' could not be executed!", ToString()));
                        transaction.Rollback();
                        lock (_queryLock)
                        {
                            _queries = new List<SqlQuery>();
                        }
                        throw e;
                    }
                }
            }
        }
    }
}
