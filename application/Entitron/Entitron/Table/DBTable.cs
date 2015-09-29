using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitron.Sql;

namespace Entitron
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
        public string AppName { get; set; }
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
        private DBIndices _indices;
        public DBIndices indices
        {
            get
            {
                if (_indices == null)
                    _indices = new DBIndices(this);

                return _indices;
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
            AppName = ApplicationName;
        }

        public DBTable Create()
        {
            SqlQuery_Table_Create query = new SqlQuery_Table_Create()
            {
                applicationName = ApplicationName,
                tableName = tableName
            };

            foreach(DBColumn column in columns)
            {
                query.AddColumn(column);
            }
            queries.Add(query);

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
            

            queries.Add(new SqlQuery_Delete()
            {
                applicationName = ApplicationName,
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

        public void AddForeignKey(string tableAName, string tableAColumns, string tableBName, string tableBColumns)
        {
            queries.Add(new SqlQuery_ForeignKeyAdd()
            {
                applicationName = ApplicationName,
                tableName = tableAName,
                table2Name = tableBName,
                foreignKey = tableAColumns,
                primaryKey = tableBColumns
            });
        }

        public void DropForeignKey(string foreignKeyName)
        {
            queries.Add(new SqlQuery_ForeignKeyDrop()
            {
                applicationName = ApplicationName,
                tableName = tableName,
                foreignKeyName = foreignKeyName
            });
        }

        public List<string> GetForeignKeys()
        {
            return new SqlQuery_SelectFogreignKeys()
            {
                applicationName = AppName,
                tableName = tableName
            }.ExecuteWithRead().Select(f => (string)f["ForeignKeyName"]).ToList();
        } 

        public void AddPrimaryKey(List<string> primaryKey)
        {
            queries.Add(new SqlQuery_PrimaryKeyAdd()
            {
                applicationName = AppName,
                tableName = tableName,
                keyColumns = primaryKey
            });
        }

        public void DropPrimaryKey()
        {
            queries.Add(new SqlQuery_PrimaryKeyDrop()
            {
                applicationName = AppName,
                tableName = tableName
            });
        }

        public override string ToString()
        {
            return tableName;
        }
    }
}
