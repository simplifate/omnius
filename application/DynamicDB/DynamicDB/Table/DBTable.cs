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
        internal static SqlQueue queries = new SqlQueue();

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
            List<DBItem> items = (new SqlQuery_Select_TableList() { ApplicationName = ApplicationName }).ExecuteWithRead();
            
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

        public string tableName { get; set; }
        public string AppName { get { return _AppName; } }
        private string _AppName { get; set; }
        private DBColumns _columns;
        public DBColumns columns
        {
            get
            {
                if (_columns == null)
                    _columns = new DBColumns(this);
                
                return _columns;
            }
        }
        private List<string> _primaryKeys = null;
        public List<string> primaryKeys
        {
            get
            {
                if (_primaryKeys == null || _primaryKeys.Count < 1)
                {
                    List<DBItem> primaryKeyItem = new SqlQuery_Table_GetPrimaryKey() { applicationName = ApplicationName, tableName = tableName }.ExecuteWithRead();
                    _primaryKeys = primaryKeyItem.Select(pk => (string)pk["column_name"]).ToList();
                }

                return _primaryKeys;
            }
            set
            {
                _primaryKeys = value;
            }
        }
        public List<DBColumn> getPrimaryColumns()
        {
            List<DBColumn> output = new List<DBColumn>();
            foreach(DBColumn column in columns)
            {
                if (primaryKeys.Contains(column.Name))
                    output.Add(column);
            }

            return output;
        }

        public DBTable()
        {
            _AppName = ApplicationName;
        }

        public DBTable Create()
        {
            queries.Add(new SqlQuery_Table_Create()
            {
                applicationName = ApplicationName,
                tableName = tableName
            });

            return this;
        }
        public DBTable Drop()
        {
            queries.Add(new SqlQuery_Table_Drop()
            {
                applicationName = ApplicationName,
                tableName = tableName
            });

            return this;
        }
        public DBTable Rename(string newName)
        {
            queries.Add(new SqlQuery_Table_Rename()
            {
                applicationName = ApplicationName,
                tableName = tableName,
                newName = newName
            });

            return this;
        }

        public DBTable Add(DBItem item)
        {
            Dictionary<DBColumn, object> data = new Dictionary<DBColumn, object>();
            foreach(DBColumn column in columns)
            {
                data.Add(column, item[column.Name]);
            }

            queries.Add(new SqlQuery_Insert()
            {
                applicationName = ApplicationName,
                tableName = tableName,
                data = data
            });

            return this;
        }
        public DBTable Update(DBItem item)
        {

            return this;
        }
        public DBTable Remove(DBItem item)
        {
            Dictionary<DBColumn, object> columnValueCondition = new Dictionary<DBColumn, object>();
            foreach(DBColumn primaryColumn in getPrimaryColumns())
            {
                columnValueCondition.Add(primaryColumn, item[primaryColumn.Name]);
            }
            

            queries.Add(new SqlQuery_Delete(ApplicationName)
            {
                tableName = tableName,
                columnValueCondition = columnValueCondition
            });

            return this;
        }

        public SqlQuery_Select Select(params string[] columns)
        {
            return new SqlQuery_Select()
            {
                applicationName = ApplicationName,
                columns = columns.ToList(), tableName = tableName
            };
        }

        public void Index(string index,List<string> columns)
        {
            queries.Add(new SqlQuery_IndexCreate()
            {
                applicationName = ApplicationName,
                columnsName =columns,
                tableName = tableName,
                indexName = index
            });
        }
    }
}
