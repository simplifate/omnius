using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicDB.Sql;

namespace DynamicDB
{
    public class DBTable
    {
        #region static
        private static SqlQueue queries = new SqlQueue();

        public static DBTable Create(string name)
        {
            return new DBTable() { tableName = name }.Create();
        }
        public static DBTable GetTable(string name)
        {
            return new DBTable() { tableName = name };
        }
        public static List<DBTable> GetAll()
        {
            List<DBItem> items = (new SqlQuery_Select_TableList(ApplicationName)).ExecuteWithRead();
            
            return items.Select(i => new DBTable() { tableName = (string)i["Name"] }).ToList();
        }
        public static void SaveChanges()
        {
            queries.ExecuteAll();
        }
        public static string connectionString
        {
            get { return queries.connectionString; }
            set { queries.connectionString = value; }
        }
        public static string ApplicationName;
        #endregion

        public string tableName;

        public DBTable Create()
        {
            queries.Add(
                new SqlQuery_Table_Create(ApplicationName, tableName)
                );

            return this;
        }
        public DBTable Drop()
        {
            queries.Add(
                new SqlQuery_Table_Drop(ApplicationName, tableName)
                );

            return this;
        }
        public DBTable Rename(string newName)
        {
            queries.Add(
                new SqlQuery_Table_Rename(ApplicationName, tableName, newName)
                );

            return this;
        }

        public List<DBColumn> getColumnList()
        {
            SqlQuery_Select_ColumnList query = new SqlQuery_Select_ColumnList(ApplicationName, tableName);
            List<DBItem> items = query.ExecuteWithRead();
            // ##!!##
            return null;
        }
        public DBTable AddColumn(string name, System.Data.SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
        {
            SqlQuery_Table_Create query;
            if ((query = queries.GetCreate(tableName)) != null)
                query.AddColumn(name, type, maxLength, canBeNull, additionalOptions);
            else
                queries.Add(new SqlQuery_Column_Add(ApplicationName, tableName, new DBColumn()
                {
                    Name = name,
                    type = type,
                    maxLength = maxLength,
                    canBeNull = canBeNull,
                    additionalOptions = additionalOptions
                }));

            return this;
        }
    }
}
