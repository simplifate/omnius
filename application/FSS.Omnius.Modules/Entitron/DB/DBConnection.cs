using System;
using System.Collections.Generic;
using System.Data;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Data.Common;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBConnection
    {
        public DBConnection(int requestHash)
        {
            _requestHash = requestHash;
            Application = null;
            _type = Entitron.DefaultDBType;
            _connectionString = Entitron.EntitronConnectionString(_type);

            _tablesToSave = new HashSet<DBTable>();
            _commandSet = DBCommandSet.GetDBCommandSet(_type);
        }

        public DBConnection(int requestHash, Application application)
        {
            _requestHash = requestHash;
            Application = application;

            _type = application.DB_Type;

            _connectionString = (Application != null && !string.IsNullOrEmpty(Application.DB_ConnectionString))
                    ? Application.DB_ConnectionString
                    : Entitron.EntitronConnectionString(_type);

            _tablesToSave = new HashSet<DBTable>();
            _commandSet = DBCommandSet.GetDBCommandSet(_type);
        }

        private int _requestHash;

        public ESqlType Type => _type;
        private ESqlType _type;

        public Application Application { get; set; }

        public string ConnectionString => _connectionString;
        private string _connectionString;

        private HashSet<DBTable> _tablesToSave;
        private DBCommandSet _commandSet;
        public DBCommandSet CommandSet => _commandSet;
        private Queue<IDbCommand> _commands = new Queue<IDbCommand>();
        public Queue<IDbCommand> Commands => _commands;
        
        #region Data select
        public List<string> List(ETabloid list)
        {
            List<string> result = new List<string>();

            using (DBReader reader = ExecuteCommand(CommandSet.LIST_tabloid(this, list)))
            {
                while (reader.Read())
                {
                    result.Add(CommandSet.FromRealTableName(Application, (string)reader["name"]));
                }
            }

            return result;
        }

        public DBTable Table(string name, bool isShared = false)
        {
            return new DBTable(isShared ? Entitron.iShared : this)
            {
                Name = name
            };
        }

        public Tabloid Tabloid(string name, bool isShared = false)
        {
            return new Tabloid(isShared ? Entitron.iShared : this)
            {
                Name = name
            };
        }

        public Select Select(string tabloidName, bool isShared = false, params string[] columns)
        {
            return new Select(isShared ? Entitron.iShared : this, tabloidName, columns);
        }
        public Delete Delete(string tabloidName, bool isShared = false)
        {
            return new Delete(isShared ? Entitron.iShared : this, tabloidName);
        }
        public bool ExecSP(string procedureName, Dictionary<string, string> parameters)
        {
            string execParams = "";
            List<string> execParamsList = new List<string>();
            if (parameters.Count > 0)
            {
                foreach (KeyValuePair<string, string> p in parameters)
                {
                    execParamsList.Add($"@{p.Key} = '{p.Value}'");
                }
                execParams = string.Join(", ", execParamsList);
            }

            try
            {
                ExecuteNonQuery(CommandSet.ExecSP(this, procedureName, execParams));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        
        #region Edit scheme
        public DBTable TableCreate()
        {
            DBTable newTable = new DBTable(this);
            Commands.Enqueue(CommandSet.CREATE_table(this, newTable));

            return newTable;
        }
        public void TableCreate(DBTable table)
        {
            Commands.Enqueue(CommandSet.CREATE_table(this, table));
        }

        public void TableDrop(DBTable table)
        {
            foreach (DBForeignKey fk in table.AllForeignKeys)
            {
                table.ForeignKeys.Remove(fk);
            }

            Commands.Enqueue(CommandSet.DROP_table(this, table.Name));
        }

        public void TableTruncate(string tableName)
        {
            Commands.Enqueue(CommandSet.TRUNCATE_table(this, tableName));
        }
        
        public DBView ViewCreate()
        {
            DBView newView = new DBView(this);
            Commands.Enqueue(CommandSet.CREATE_view(this, newView));

            return newView;
        }
        public void ViewCreate(DBView view)
        {
            Commands.Enqueue(CommandSet.CREATE_view(this, view));
        }

        public void ViewDrop(string viewName)
        {
            Commands.Enqueue(CommandSet.DROP_view(this, viewName));
        }

        public bool Exists(string tabloidName, ETabloid list)
        {
            bool exists = false;

            using (DBReader reader = ExecuteCommand(CommandSet.LIST_tabloid(this, list, tabloidName)))
            {
                while (reader.Read())
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
        #endregion

        #region Exec
        public ListJson<DBItem> ExecuteRead(IDbCommand command, Tabloid tabloid)
        {
            using (DBReader reader = ExecuteCommand(command))
            {
                ListJson<DBItem> result = Read(reader, tabloid);
                return result;
            }
        }
        /// <summary>
        /// Don't forget to close DBReader!!!
        /// </summary>
        public DBReader ExecuteCommand(IDbCommand command)
        {
            IDbConnection connection = CommandSet.Connection;
            connection.ConnectionString = ConnectionString;
            connection.Open();

            command.Connection = connection;
            try
            {
                IDataReader reader = command.ExecuteReader();
                return new DBReader(connection, reader);
            }
            catch (DbException ex)
            {
                throw SqlExceptionMessage.TransformAndLogMessage(ex, command);
            }
        }
        public int ExecuteNonQuery(IDbCommand command)
        {
            using (IDbConnection connection = CommandSet.Connection)
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                command.Connection = connection;
                try
                {
                    return command.ExecuteNonQuery();
                }
                catch(DbException ex)
                {
                    // always throws exception
                    throw SqlExceptionMessage.TransformAndLogMessage(ex, command);
                }
            }
        }
        internal void SaveTable(DBTable table)
        {
            _tablesToSave.Add(table);
        }
        public int SaveChanges()
        {
            // add items
            foreach (DBTable table in _tablesToSave)
                table.AddRange(table._itemsToAdd);

            if (!_commands.Any())
                return 0;

            int total = 0;
            using (IDbConnection connection = _commandSet.Connection)
            {
                connection.ConnectionString = _connectionString;

                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    IDbCommand command = null;
                    try
                    {
                        while(_commands.Any())
                        {
                            command = _commands.Dequeue();

                            command.Connection = connection;
                            command.Transaction = transaction;
                            total += command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw SqlExceptionMessage.TransformAndLogMessage(ex, command);
                    }

                }
            }

            return total;
        }

        public ListJson<DBItem> Read(DBReader reader, Tabloid tabloid)
        {
            ListJson<DBItem> items = new ListJson<DBItem>();

            while (reader.Read())
            {
                DBItem newItem = new DBItem(this, tabloid);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    newItem[columnName] = reader[columnName];
                }

                items.Add(newItem);
            }

            return items;
        }
        #endregion
    }
}
