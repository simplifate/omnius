using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using Logger;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery
    {
        public const string DB_MasterApplication = "dbo.Master_Applications";
        public const string DB_EntitronMeta = "dbo.Entitron___META";

        public string sqlString;
        internal Dictionary<string, object> _params;
        internal Dictionary<string, string> _datatypes;

        public SqlQuery()
        {
            _params = new Dictionary<string, object>();
            _datatypes = new Dictionary<string, string>();
        }

        public void Execute(string connectionString = null)
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
        public virtual ListJson<DBItem> ExecuteWithRead()
        {
            if (Entitron.connectionString == null)
                throw new ArgumentNullException("connectionString");

            ListJson<DBItem> items = null;
            using (SqlConnection connection = new SqlConnection(Entitron.connectionString))
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
                    throw e;
            }
        }
        protected virtual ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
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

            string msg = cmd.CommandText + "\r\n";
            foreach (KeyValuePair<string, object> param in _params)
            {
                cmd.Parameters.Add(new SqlParameter(param.Key, param.Value));
                msg += param.Key + ": " + param.Value + "\r\n";
            }
            msg += "\r\n";
           
            Log.SQL(msg);
            return cmd;
        }
        protected virtual ListJson<DBItem> Read(SqlDataReader reader)
        {
            ListJson<DBItem> items = new ListJson<DBItem>();

            while(reader.Read())
            {
                DBItem newItem = new DBItem();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    int columnId = reader.GetOrdinal(columnName);
                    newItem.createProperty(columnId, columnName, reader[columnName]);
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
            string type = a.SqlDbType.ToString();
            if (type == "Decimal")//TODO precision a scale se už vyplňuje, ale stále dochází k chybám, float zatím neotestován
            {
                decimal dec;
                decimal.TryParse(value.ToString(), out dec);
                SqlDecimal decA = new SqlDecimal(dec);
                a.Precision = decA.Precision;
                a.Scale = decA.Scale;
                _datatypes[key] = string.Format("{0}({1}, {2})", a.SqlDbType.ToString(),
                    (a.Precision != 0) ? a.Precision.ToString() : "18", (a.Scale != 0) ? a.Scale.ToString() : "0");
            }
            else if (type == "Float")
            {
                _datatypes[key] = string.Format("{0}({1})", a.SqlDbType.ToString(),
                   (a.Precision != 0) ? a.Precision.ToString() : "53");
            }
            else
            {
                _datatypes[key] = (a.Size != 0) ? string.Format("{0}({1})", a.SqlDbType.ToString(), (a.Size != -1) ? a.Size.ToString() : "MAX") : a.SqlDbType.ToString();
            }

            
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
