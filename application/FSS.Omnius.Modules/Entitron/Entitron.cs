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
    using System.Data.Entity;
    public class Entitron : IModule
    {
        public const string connectionString = "data source=omnius-develop.database.windows.net;initial catalog=Omnius_Dev;user id=fss;password=dLsvPd$3?Wh%_52F;MultipleActiveResultSets=True;App=EntityFramework;Min Pool Size=3;Load Balance Timeout=180;";
        private CORE _CORE;
        private DBEntities entities = null;

        public Application Application { get; set; }
        public string AppName
        {
            get { return (Application != null) ? Application.Name : null; }
            set
            {
                if (value != null)
                    Application = this.GetAppSchemeByName(value);
            }
        }
        public int AppId
        {
            get { return Application.Id; }
            set
            {
                Application = this.GetAppSchemeById(value);
            }
        }
        public IConditionalFilteringService filteringService { get; set; }

        public Application GetAppSchemeById(int id)
        {
            return GetStaticTables().Applications.SingleOrDefault(app => app.Id == id);
        }

        public Application GetAppSchemeByName(string name)
        {
            return GetStaticTables().Applications.SingleOrDefault(app => app.Name == name);
        }

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
        public Tuple<List<string>, List<DBItem>> GetSystemTable(string tableName)
        {
            switch (tableName)
            {
                case "Omnius::AppRoles":
                    return DBSetToTable(entities.Roles);
                case "Omnius::Users":
                    return DBSetToTable(entities.Users);
                case "Omnius::LogItems":
                    return DBSetToTable(entities.LogItems);
                default:
                    throw new InvalidOperationException($"System table {tableName} not found");
            }
        }
        public IEnumerable<DBTable> GetDynamicTables()
        {
            if (Application == null)
                throw new ArgumentNullException("Application");

            return Application.GetTables();
        }
        public Tuple<List<string>, List<DBItem>> DBSetToTable<T>(IDbSet<T> dbset) where T : class
        {
            List<T> inputList = dbset.ToList();
            List<DBItem> rowList = new List<DBItem>();
            var propertyList = typeof(T).GetProperties().Where(c => c.PropertyType.IsValueType)
                .GroupBy(c => c.Name).Select(c => c.First());
            foreach (var entity in inputList)
            {
                var newRow = new DBItem();
                int columnId = 0;
                foreach (var property in propertyList)
                {
                    object value = property.GetValue(entity, null);
                    newRow.createProperty(columnId, property.Name, value);
                    columnId++;
                }
                rowList.Add(newRow);
            }
            var columnList = propertyList.Select(c => c.Name).ToList();
            return new Tuple<List<string>, List<DBItem>>(columnList, rowList);
        }
        public DBTable GetDynamicTable(string tableName, bool shared = false)
        {
            if (Application == null)
                throw new ArgumentNullException("Application");

            if (!shared)
            {
                return Application.GetTable(tableName);
            }
            else
            {
                return this.GetAppSchemeById(SharedTables.AppId).GetTable(tableName);
            }
        }

        public DBView GetDynamicView(string viewName, bool shared = false)
        {
            if (Application == null)
                throw new ArgumentNullException("Application");

            if (!shared)
            {
                return Application.GetView(viewName);
            }
            else
            {
                return this.GetAppSchemeById(SharedTables.AppId).GetView(viewName);
            }
        }

        public DBItem GetDynamicItem(string tableName, int modelId, bool shared = false)
        {
            if (string.IsNullOrWhiteSpace(tableName) || modelId < 0)
                return null;

            if (!shared)
            {
                return Application.GetTable(tableName).Select().where(c => c.column("Id").Equal(modelId)).ToList().FirstOrDefault();
            }
            else
            {
                return this.GetAppSchemeById(SharedTables.AppId).GetTable(tableName).Select().where(c => c.column("Id").Equal(modelId)).ToList().FirstOrDefault();
            }
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
            string realTableName = $"Entitron_{(Application.Id == SharedTables.AppId ? SharedTables.Prefix : Application.Name)}_{tableName}";

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
