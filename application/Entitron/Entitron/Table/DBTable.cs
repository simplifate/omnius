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
            SqlQuery_Table_exists query = new SqlQuery_Table_exists()
            {
                applicationName = ApplicationName,
                tableName = name
            };

            // if table exists
            if (query.ExecuteWithRead() != null)
            {
                return new DBTable(name);
            }

            return null;
        }
        public static List<DBTable> GetAll()
        {
            List<DBItem> items = (new SqlQuery_Select_TableList() { ApplicationName = ApplicationName }).ExecuteWithRead();
            
            return items.Select(i => new DBTable((string)i["Name"])).ToList();
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

        private string _tableNameInDB;

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
        private DBForeignKeys _foreignKeys;
        public DBForeignKeys foreignKeys
        {
            get
            {
                if (_foreignKeys == null)
                    _foreignKeys = new DBForeignKeys(this);

                return _foreignKeys;
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

        public DBTable(string tableNameInDB = null)
        {
            _tableNameInDB = tableNameInDB;
            AppName = ApplicationName;

            tableName = _tableNameInDB;
        }

        public DBTable Create()
        {
            SqlQuery_Table_Create query = new SqlQuery_Table_Create()
            {
                applicationName = ApplicationName,
                tableName = tableName
            };

            // add columns from queries
            foreach(SqlQuery_Column_Add columnQuery in queries.GetAndRemoveQueries<SqlQuery_Column_Add>(tableName))
            {
                query.AddColumn(columnQuery.column);
            }
            
            // add columns from list
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
        public bool isInDB()
        {
            if (string.IsNullOrWhiteSpace(AppName) || string.IsNullOrWhiteSpace(tableName))
                return false;

            return (_tableNameInDB != null);
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
        public DBTable Update(DBItem item, DBItem selectRow )
        {
            Dictionary<DBColumn, object> data = new Dictionary<DBColumn, object>();
            Dictionary<DBColumn, object> row = new Dictionary<DBColumn, object>();

            foreach (DBColumn column in columns)
            {
                data.Add(column, item[column.Name]);
            }
            foreach (DBColumn pkColum in getPrimaryColumns())
            {
                row.Add(pkColum, selectRow[pkColum.Name]);
            }

            queries.Add(new SqlQuery_Update()
            {
              applicationName  = AppName,
              tableName = tableName,
              changes = data,
              rowSelect = row
            });

            return this;
        }
        public DBTable Remove(DBItem item)
        {
            Dictionary<DBColumn, object> columnValueCondition = new Dictionary<DBColumn, object>();

            foreach (DBColumn primaryColumn in getPrimaryColumns())
            {
                columnValueCondition.Add(primaryColumn, item[primaryColumn.Name]);
            }

            queries.Add(new SqlQuery_Delete()
            {
                applicationName = ApplicationName,
                tableName = tableName,
                rowSelect = columnValueCondition
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

        public void AddPrimaryKey(List<string> primaryKey)
        {
            bool isClusterAlreadyCreate = false;

            if (indices != null)
            {
                foreach (DBIndex index in indices)
                {
                    if (index.indexName == "index_" + AppName + tableName)
                    {
                        isClusterAlreadyCreate = true;
                        break;

                    }
                }
            }
         
            queries.Add(new SqlQuery_PrimaryKeyAdd()
            {
                applicationName = AppName,
                tableName = tableName,
                keyColumns = primaryKey,
                isClusterCreated = isClusterAlreadyCreate
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

        public void DisableConstraint(string constraintName)
        {
            queries.Add(new SqlQuery_ConstraintDisabled()
            {
                applicationName = AppName,
                tableName = tableName,
                constraintName = constraintName
            });
        }

        public void EnableConstraint(string constraintName)
        {
            queries.Add(new SqlQuery_ConstraintEnable()
            {
                applicationName = AppName,
                tableName = tableName,
                constraintName = constraintName
            });
        }

        public List<string> getConstraints()
        {
            SqlQuery_SelectConstrains query = new SqlQuery_SelectConstrains();
            List<string> constraints=new List<string>();

            foreach (DBItem i in query.ExecuteWithRead())
            {
                constraints.Add(i["name"].ToString());
            }
            return constraints;
        } 
    }
}
