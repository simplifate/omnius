using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron
{
    using DB;
    using FSS.Omnius.Modules.CORE;
    using FSS.Omnius.Modules.Entitron.Entity;
    using System.Configuration;

    public class Entitron : IModule
    {
        public static Tuple<List<string>, List<DBItem>> GetSystemTable(string tableName)
        {
            DBEntities context = COREobject.i.Context;

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
        public static Tuple<List<string>, List<DBItem>> DBSetToTable<T>(IDbSet<T> dbset) where T : class
        {
            List<DBItem> rowList = new List<DBItem>();
            var propertyList = typeof(T).GetProperties().Where(c => c.PropertyType.IsValueType)
                .GroupBy(c => c.Name).Select(c => c.First());
            foreach (var entity in dbset.ToList())
            {
                var newRow = new DBItem(COREobject.i.Entitron, null);
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

        public static void ParseConnectionString(string connectionStringName)
        {
            DefaultConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            DefaultDBType = DBCommandSet.GetSqlType(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);
        }

        public static string DefaultConnectionString = "DefaultConnection";
        public static ESqlType DefaultDBType = ESqlType.MSSQL;
        public static string EntityConnectionString
        {
            get
            {
                if (DefaultConnectionString == "DefaultConnection")
                    return DefaultConnectionString;

                if (DefaultDBType == ESqlType.MSSQL)
                    return $"{DefaultConnectionString}{(DefaultConnectionString.EndsWith(";") ? "" : ";")}App=EntityFramework;";

                return DefaultConnectionString;
            }
        }
        public static string EntitronConnectionString(ESqlType dbType)
        {
            if (DefaultConnectionString == "DefaultConnection")
                return DefaultConnectionString;

            if (DefaultDBType == ESqlType.MSSQL)
                return $"{DefaultConnectionString}{(DefaultConnectionString.EndsWith(";") ? "" : ";")}App=EntityFramework;";

            if (DefaultDBType == ESqlType.MySQL)
                return $"{DefaultConnectionString}{(DefaultConnectionString.EndsWith(";") ? "" : ";")}Allow User Variables=True;";

            return DefaultConnectionString;
        }

        public static void ClearCache()
        {
            DBColumn.Cache.Clear();
        }
    }
}
