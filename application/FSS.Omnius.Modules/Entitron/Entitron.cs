namespace FSS.Omnius.Modules.Entitron
{
    using DB;
    using FSS.Omnius.Modules.CORE;
    using FSS.Omnius.Modules.Entitron.Entity;
    using FSS.Omnius.Modules.Entitron.Entity.Master;
    using FSS.Omnius.Modules.Entitron.Service;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;

    public class Entitron : IModule
    {
        public Entitron(CORE core)
        {
            _CORE = core;
            FilteringService = new ConditionalFilteringService();
        }

        private CORE _CORE;
        public IConditionalFilteringService FilteringService { get; set; }


        public Tuple<List<string>, List<DBItem>> GetSystemTable(string tableName)
        {
            DBEntities context = DBEntities.instance;

            switch (tableName)
            {
                case "Omnius::AppRoles":
                    return DBSetToTable(context.Roles);
                case "Omnius::Users":
                    return DBSetToTable(context.Users);
                case "Omnius::LogItems":
                    return DBSetToTable(context.LogItems);
                default:
                    throw new InvalidOperationException($"System table {tableName} not found");
            }
        }
        public Tuple<List<string>, List<DBItem>> DBSetToTable<T>(IDbSet<T> dbset) where T : class
        {
            List<DBItem> rowList = new List<DBItem>();
            var propertyList = typeof(T).GetProperties().Where(c => c.PropertyType.IsValueType)
                .GroupBy(c => c.Name).Select(c => c.First());
            foreach (var entity in dbset.ToList())
            {
                var newRow = new DBItem(i);
                foreach (var property in propertyList)
                {
                    object value = property.GetValue(entity, null);
                    newRow[property.Name] = value;
                }
                rowList.Add(newRow);
            }
            var columnList = propertyList.Select(c => c.Name).ToList();
            return new Tuple<List<string>, List<DBItem>>(columnList, rowList);
        }


        public static string DefaultConnectionString = "DefaultConnection";
        public static ESqlType DefaultDBType = ESqlType.MSSQL;
        public static string EntityConnectionString
        {
            get
            {
                if (DefaultConnectionString == "DefaultConnection")
                    return DefaultConnectionString;

                return $"{DefaultConnectionString}{(DefaultConnectionString.EndsWith(";") ? "" : ";")}App=EntityFramework;";
            }
        }
        public static string EntitronConnectionString
        {
            get
            {
                if (DefaultConnectionString == "DefaultConnection")
                    return DefaultConnectionString;

                return $"{DefaultConnectionString}{(DefaultConnectionString.EndsWith(";") ? "" : ";")}App=Entitron;";
            }
        }

        private static int requestHash
        {
            get
            {
                try
                {
                    if (HttpContext.Current == null)
                        return 0;    //pro přístup z jiného vlákna
                    return HttpContext.Current.Request.GetHashCode();
                }
                catch (HttpException)
                {
                    return 0;
                }
            }
        }
        private static Dictionary<int, DBConnection> _connections = new Dictionary<int, DBConnection>();
        private static object _connectionLock = new object();
        public static DBConnection i
        {
            get
            {
                lock (_connectionLock)
                {
                    if (!_connections.ContainsKey(requestHash))
                    {
                        _connections[requestHash] = new DBConnection(requestHash);
                    }

                    return _connections[requestHash];
                }
            }
        }
        private static DBConnection _iShared;
        public static DBConnection iShared
        {
            get
            {
                if (_iShared == null)
                    _iShared = new DBConnection(requestHash, Application.SystemApp());

                return _iShared;
            }
        }

        public static bool Create(int applicationId)
        {
            return Create(DBEntities.instance.Applications.Find(applicationId));
        }
        public static bool Create(string applicationName)
        {
            return Create(DBEntities.instance.Applications.SingleOrDefault(a => a.Name == applicationName));
        }
        public static bool Create(Application application)
        {
            if (application != null)
            {
                if (_connections.ContainsKey(requestHash))
                {
                    Logger.Log.Warn("DBConnection overriten");
                }

                _connections[requestHash] = new DBConnection(requestHash, application);
                return true;
            }

            return false;
        }
        public static void Destroy()
        {
            lock(_connectionLock)
            {
                _connections.Remove(requestHash);
            }
        }

        public static void ClearCache()
        {
            DBColumn.Cache.Clear();
        }
    }
}
