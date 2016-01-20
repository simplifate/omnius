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
        public string connectionString;
        internal static List<SqlQuery> _queries = new List<SqlQuery>();

        public SqlQueue Add(SqlQuery query)
        {
            _queries.Add(query);

            return this;
        }
        public T GetQuery<T>(string tableName) where T : SqlQuery_withApp
        {
            return (T)_queries.FirstOrDefault(q => q is T && (q as T).table.tableName == tableName);
        }
        public List<T> GetAndRemoveQueries<T>(string tableName) where T : SqlQuery_withApp
        {
            List<T> output = new List<T>();
            for(int i = 0; i < _queries.Count; i++)
            {
                SqlQuery q = _queries[i];
                if (q is T && (q as T).table.tableName == tableName)
                {
                    output.Add((T)q);
                    _queries.RemoveAt(i);
                    i--;
                }
            }

            return output;
        }

        public void ExecuteAll()
        {
            if (connectionString == null && Entitron.connectionString == null)
                throw new ArgumentNullException("connectionString");

            using (SqlConnection connection = new SqlConnection(connectionString ?? Entitron.connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (SqlQuery query in _queries)
                        {
                            query.Execute(transaction);
                        }
                        transaction.Commit();
                        _queries.Clear();
                    }
                    catch (SqlException e)
                    {
                        Log.Error(string.Format("Entitron: sql query '{0}' could not be executed!", ToString()));
                        transaction.Rollback();
                        _queries.Clear();
                        throw e;
                    }
                }
            }
        }
    }
}
