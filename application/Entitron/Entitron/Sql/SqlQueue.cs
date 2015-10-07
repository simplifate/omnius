using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Entitron.Sql
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
        public SqlQuery_Table_Create GetCreate(string tableName)
        {
            foreach(SqlQuery query in _queries)
            {
                if (query is SqlQuery_Table_Create && (query as SqlQuery_Table_Create).tableName == tableName)
                    return (SqlQuery_Table_Create)query;
            }

            return null;
        }

        public void ExecuteAll()
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
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
                        transaction.Rollback();
                        _queries.Clear();
                        throw e;
                    }
                }
            }
        }
    }
}
