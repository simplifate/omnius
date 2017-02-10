using System;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron
{
    using Entity;
    using CORE;
    using Entity.Master;
    using Service;
    using Table;
    using System.Data.SqlClient;
    public class Entitron : IModule
    {
        public const string connectionString = "data source=osqlsrv001-iason.database.windows.net;initial catalog=omniustest-db;user id=osqlsrv001_master@osqlsrv001-iason.database.windows.net;password=39Am%frV;MultipleActiveResultSets=True;App=EntityFramework;";
        private CORE _CORE;
        private DBEntities entities = null;

        public Application Application { get; set; }
        public string AppName
        {
            get { return (Application != null) ? Application.Name : null; }
            set
            {
                if (value != null)
                    Application = GetStaticTables().Applications.SingleOrDefault(a => a.Name == value);
            }
        }
        public int AppId
        {
            get { return Application.Id; }
            set
            {
                Application = GetStaticTables().Applications.SingleOrDefault(a => a.Id == value);
            }
        }
        public IConditionalFilteringService filteringService { get; set; }

        public Entitron(CORE core, string ApplicationName = null)
        {
            _CORE = core;
            AppName = ApplicationName;
            filteringService = new ConditionalFilteringService();
        }

        public DBEntities GetStaticTables()
        {
            if (entities == null)
                entities = DBEntities.instance;

            return entities;
        }
        public void CloseStaticTables()
        {
            if (entities != null)
                entities.Dispose();
        }

        public IEnumerable<DBTable> GetDynamicTables()
        {
            if (Application == null)
                throw new ArgumentNullException("Application");

            return Application.GetTables();
        }

        public DBTable GetDynamicTable(string tableName)
        {
            if (Application == null)
                throw new ArgumentNullException("Application");

            return Application.GetTable(tableName);
        }

        public DBView GetDynamicView(string viewName)
        {
            if (Application == null)
                throw new ArgumentNullException("Application");

            return Application.GetView(viewName);
        }

        public DBItem GetDynamicItem(string tableName, int modelId)
        {
            if (string.IsNullOrWhiteSpace(tableName) || modelId < 0)
                return null;

            return Application.GetTable(tableName).Select().where(c => c.column("Id").Equal(modelId)).ToList().FirstOrDefault();
        }

        public bool ExecSP(string procedureName, Dictionary<string, string> parameters)
        {
            string execParams = "";
            List<string> execParamsList = new List<string>();
            if(parameters.Count > 0) {
                foreach(KeyValuePair<string,string> p in parameters) {
                    execParamsList.Add($"@{p.Key} = '{p.Value}'");
                }
                execParams = " " + string.Join(", ", execParamsList);
            }

            try {
                var cmd = new SqlCommand($"EXEC {procedureName}{execParams};", new SqlConnection(connectionString));
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception) {
                return false;
            }
        }

        public bool TruncateTable(string tableName)
        {
            string realTableName = $"Entitron_{Application.Name}_{tableName}";

            try {
                using (var connection = new SqlConnection(connectionString)) {
                    connection.Open();

                    var cmd = new SqlCommand($"TRUNCATE TABLE {realTableName};", connection);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
}
