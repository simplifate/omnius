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
        public static DBTable Create(string name)
        {
            return new DBTable() { tableName = name }.Create();
        }
        #endregion

        private int? _tableId;

        public string tableName { get; set; }
        public DBApp Application { get; set; }
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
                    List<DBItem> primaryKeyItem = new SqlQuery_Table_GetPrimaryKey() { application = Application, table = this }.ExecuteWithRead();
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

        public DBTable()
        {
            _tableId = null;
        }
        public DBTable(int tableId)
        {
            _tableId = tableId;
        }

        public DBTable Create()
        {
            SqlQuery_Table_Create query = new SqlQuery_Table_Create()
            {
                application = Application,
                table = this
            };

            // add columns from queries
            foreach(SqlQuery_Column_Add columnQuery in Application.queries.GetAndRemoveQueries<SqlQuery_Column_Add>(tableName))
            {
                query.AddColumn(columnQuery.column);
            }
            
            // add columns from list
            foreach(DBColumn column in columns)
            {
                query.AddColumn(column);
            }

            Application.queries.Add(query);

            return this;
        }
        public DBTable Drop()
        {
            Application.queries.Add(new SqlQuery_Table_Drop()
            {
                application = Application,
                table = this
            });

            return this;
        }
        public DBTable Rename(string newName)
        {
            Application.queries.Add(new SqlQuery_Table_Rename()
            {
                application = Application,
                table = this,
                newName = newName
            });

            return this;
        }
        public DBTable Truncate()
        {
            Application.queries.Add(new SqlQuery_TableTruncate()
            {
                application = Application,
                table = this
            });

            return this;
        }
        public bool isInDB()
        {
            if (Application == null || string.IsNullOrWhiteSpace(tableName))
                return false;

            return (_tableId != null);
        }

        public DBTable Add(DBItem item)
        {
            Dictionary<DBColumn, object> data = new Dictionary<DBColumn, object>();
            foreach(DBColumn column in columns)
            {
                data.Add(column, item[column.Name]);
            }

            Application.queries.Add(new SqlQuery_Insert()
            {
                application = Application,
                table = this,
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

            Application.queries.Add(new SqlQuery_Update()
            {
              application  = Application,
              table = this,
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

            Application.queries.Add(new SqlQuery_Delete()
            {
                application = Application,
                table = this,
                rowSelect = columnValueCondition
            });

            return this;
        }

        public SqlQuery_Select Select(params string[] columns)
        {
            return new SqlQuery_Select()
            {
                application = Application,
                table = this,
                columns = columns.ToList()
            };
        }

        public void AddPrimaryKey(List<string> primaryKey)
        {
            bool isClusterAlreadyCreate = false;

            if (indices != null)
            {
                foreach (DBIndex index in indices)
                {
                    if (index.indexName == "index_" + Application.Name + tableName)
                    {
                        isClusterAlreadyCreate = true;
                        break;

                    }
                }
            }

            Application.queries.Add(new SqlQuery_PrimaryKeyAdd()
            {
                application = Application,
                table = this,
                keyColumns = primaryKey,
                isClusterCreated = isClusterAlreadyCreate
            });
        }
        public void DropPrimaryKey()
        {
            Application.queries.Add(new SqlQuery_PrimaryKeyDrop()
            {
                application = Application,
                table = this
            });
        }

        public override string ToString()
        {
            return tableName;
        }

        public void DisableConstraint(string constraintName)
        {
            Application.queries.Add(new SqlQuery_ConstraintDisabled()
            {
                application = Application,
                table = this,
                constraintName = constraintName
            });
        }
        public void EnableConstraint(string constraintName)
        {
            Application.queries.Add(new SqlQuery_ConstraintEnable()
            {
                application = Application,
                table = this,
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
