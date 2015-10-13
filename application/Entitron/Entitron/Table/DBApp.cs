using System;
using System.Collections.Generic;
using System.Linq;
using Entitron.Sql;

namespace Entitron
{
    public class DBApp
    {
        public static string connectionString;
        public static IEnumerable<DBApp> GetAll()
        {
            SqlQuery query = new SqlQuery();
            query.sqlString = string.Format("SELECT * FROM {0};", SqlQuery.DB_MasterApplication);
            List<DBItem> apps = query.ExecuteWithRead();
            
            return apps.Select(a => new DBApp() { Name = (string)a["Name"], DisplayName = a["DisplayName"] is string ? (string)a["DisplayName"] : (string)a["Name"], ConnectionString = connectionString });
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        internal SqlQueue queries;
        
        public DBApp()
        {
            queries = new SqlQueue();
        }

        public void Create()
        {
            SqlQuery query = new SqlQuery();
            string parName = query.safeAddParam("Name", Name);
            string parDisplayName = query.safeAddParam("display", DisplayName);
            query.sqlString = string.Format("INSERT INTO {0}(Name,DisplayName) VALUES(@{1}, @{2});", SqlQuery.DB_MasterApplication, parName, parDisplayName);
            query.Execute();
        }
        public void Rename(string newNawe, string newDisplayName)
        {
            newNawe = newNawe ?? Name;
            newDisplayName = newDisplayName ?? DisplayName;

            SqlQuery query = new SqlQuery();
            string parOldName = query.safeAddParam("oldName", Name);
            string parName = query.safeAddParam("Name", newNawe);
            string parDisplayName = query.safeAddParam("display", newDisplayName);
            query.sqlString = string.Format("UPDATE {0} SET Name=@{1},DisplayName=@{2} WHERE Name=@{3});", SqlQuery.DB_MasterApplication, parName, parDisplayName, parOldName);
            query.Execute();
        }
        public void Delete()
        {
            SqlQuery query = new SqlQuery();
            string parName = query.safeAddParam("Name", Name);
            query.sqlString = string.Format("DELETE FROM {0} WHERE Name=@{1});", SqlQuery.DB_MasterApplication, parName);
            query.Execute();
        }

        public DBTable GetTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException("Name");

            SqlQuery_Table_exists query = new SqlQuery_Table_exists()
            {
                applicationName = Name,
                tableName = tableName
            };

            // if table exists
            List<DBItem> tables = query.ExecuteWithRead();
            if (tables.Count > 0)
            {
                DBItem table = tables.First();
                return new DBTable((int)table["tableId"]) { Application = this, tableName = (string)table["Name"] };
            }

            return null;
        }

        public IEnumerable<DBTable> GetTables()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException("Name");

            List<DBItem> items = (new SqlQuery_Select_TableList() { ApplicationName = Name }).ExecuteWithRead();

            return items.Select(i =>
                new DBTable((int)i["tableId"])
                {
                    tableName = (string)i["Name"],
                    Application = this
                }).ToList();
        }

        public void SaveChanges()
        {
            queries.ExecuteAll();
        }
    }
}
