using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Logger;

namespace Entitron.Sql
{
    public class SqlQuery
    {
        public const string DB_MasterApplication = "dbo.Master_Applications";
        public const string DB_EntitronMeta = "dbo.Entitron___META";

        public string sqlString;
        protected Dictionary<string, object> _params;
        protected Dictionary<string, string> _datatypes;

        public SqlQuery()
        {
            _params = new Dictionary<string, object>();
            _datatypes = new Dictionary<string, string>();
        }

        public void Execute(string connectionString = null)
        {
            connectionString = connectionString ?? DBApp.connectionString;
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        BaseExecution(transaction);
                        transaction.Commit();
                    }
                    catch (SqlException e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }
        }
        public void Execute(MarshalByRefObject connection)
        {
            BaseExecution(connection);
        }
        public virtual List<DBItem> ExecuteWithRead()
        {
            if (DBApp.connectionString == null)
                throw new ArgumentNullException("connectionString");

            List<DBItem> items = null;
            using (SqlConnection connection = new SqlConnection(DBApp.connectionString))
            {
                connection.Open();

                items = BaseExecutionWithRead(connection);
            }

            return items;
        }
        protected virtual void BaseExecution(MarshalByRefObject connection)
        {
            SqlCommand cmd = Prepare(connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(SqlException e)
            {
                if (e.Message.Contains("Could not find stored procedure 'getTableRealName'"))
                {
                    new SqlInitScript().Execute(connection);
                    cmd.ExecuteNonQuery();
                }
                else
                    Log.Error(string.Format("Entitron: sql query '{0}' could not be executed!", ToString()));
            }
        }
        protected virtual List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            SqlCommand cmd = Prepare(connection);
            SqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (SqlException e)
            {
                if (e.Message.Contains("Could not find stored procedure 'getTableRealName'") || e.Message.Contains("Could not find stored procedure 'getTableRealNameWithMeta'"))
                {
                    // declare procedure
                    new SqlInitScript().Execute(connection);
                    // repeat execution
                    reader = cmd.ExecuteReader();
                }
                else
                    throw;
            }

            return Read(reader);
        }
        
        private SqlCommand Prepare(MarshalByRefObject transaction)
        {
            SqlCommand cmd = null;
            if (transaction is SqlConnection)
                cmd = new SqlCommand(sqlString, (SqlConnection)transaction);
            else if (transaction is SqlTransaction)
                cmd = new SqlCommand(sqlString, (transaction as SqlTransaction).Connection, (SqlTransaction)transaction);
            else
                return null;

            foreach (KeyValuePair<string, object> param in _params)
            {
                cmd.Parameters.Add(new SqlParameter(param.Key, param.Value));
            }

            return cmd;
        }
        protected List<DBItem> Read(SqlDataReader reader)
        {
            List<DBItem> items = new List<DBItem>();

            while(reader.Read())
            {
                DBItem newItem = new DBItem();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    newItem[columnName] = reader[columnName];
                }

                items.Add(newItem);
            }

            return items;
        }

        public string safeAddParam(string key, object value)
        {
            // get unique key
            while(_params.ContainsKey(key))
            {
                key = key.Random(10);
            }

            // save param
            _params[key] = value;
            var a = new SqlParameter("to koukáš, co?", value);
            _datatypes[key] = (a.Size != 0) ? string.Format("{0}({1})", a.SqlDbType.ToString(), (a.Size != -1) ? a.Size.ToString() : "MAX") : a.SqlDbType.ToString();

            // return new key
            return key;
        }
        public Dictionary<string, string> safeAddParam(Dictionary<string, object> param)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (var par in param)
            {
                output[par.Key] = safeAddParam(par.Key, par.Value);
            }

            return output;
        }
        public Dictionary<DBColumn, string> safeAddParam(Dictionary<DBColumn, object> param)
        {
            Dictionary<DBColumn, string> output = new Dictionary<DBColumn, string>();
            foreach (var par in param)
            {
                output[par.Key] = safeAddParam(par.Key.Name, par.Value);
            }

            return output;
        }
    }
}
