using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBTable
    {
        #region static
        public static DBTable Create(string name)
        {
            return new DBTable() { tableName = name }.Create();
        }
        #endregion

        public int? tableId;

        public string tableName { get; set; }
        public Application Application { get; set; }
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
            foreach (DBColumn column in columns)
            {
                if (primaryKeys.Contains(column.Name))
                    output.Add(column);
            }

            return output;
        }

        public DBTable()
        {
            tableId = -1;
        }
        public DBTable(int tableId)
        {
            this.tableId = tableId;
        }

        public DBTable Create()
        {
            SqlQuery_Table_Create query = new SqlQuery_Table_Create()
            {
                application = Application,
                table = this
            };

            // add columns from queries
            foreach (SqlQuery_Column_Add columnQuery in Application.queries.GetAndRemoveQueries<SqlQuery_Column_Add>(tableName))
            {
                query.AddColumn(columnQuery.column);
            }

            // add columns from list
            foreach (DBColumn column in columns)
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

            return (tableId != null);
        }

        public DBTable Add(DBItem item)
        {
            Dictionary<DBColumn, object> data = new Dictionary<DBColumn, object>();
            foreach (DBColumn column in columns)
            {
                if (item.HasProperty(column.Name))
                {
                    data.Add(column, item[column.Name]);
                }
            }

            Application.queries.Add(new SqlQuery_Insert()
            {
                application = Application,
                table = this,
                data = data
            });

            return this;
        }

        public DBTable AddCheck(string checkName,Conditions where)
        {
            Application.queries.Add(new SqlQuery_CheckAdd()
            {
                application = Application,
                table = this,
                where = where.ToString(),
                checkName = checkName
            });

            return this;
        }

        public List<string> GetCheckConstraints()
        {
            List<string> checks=new List<string>();
            SqlQuery_SelectCheckConstraints query = new SqlQuery_SelectCheckConstraints()
            {
                application = Application,
                table = this
            };
            foreach (DBItem item in query.ExecuteWithRead())
            {
                checks.Add(item["name"].ToString());
            }
            return checks;
        } 

        public List<string> GetOperators()
        {
            List<string> operators = new List<string>();
            operators.Add("");
            operators.Add("Equal");
            operators.Add("Not Equal");
            operators.Add("Less");
            operators.Add("Less or Equal");
            operators.Add("Greater");
            operators.Add("Greater or Equal");
            operators.Add("Like");
            operators.Add("Not Like");
            operators.Add("Between");
            operators.Add("Not Between");
            operators.Add("Null");
            operators.Add("Not Null");
            operators.Add("In");
            operators.Add("Not In");

            return operators;
        } 

        public DBTable Update(DBItem item, DBItem selectRow)
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
                application = Application,
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
        public DBTable Remove(int itemId)
        {
            Dictionary<DBColumn, object> columnValueCondition = new Dictionary<DBColumn, object>();
            columnValueCondition.Add(new DBColumn() { Name = "Id" }, itemId);

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
        public void DropConstraint(string constraintName, bool? isPrimaryKey=null)
        {
            if (isPrimaryKey == true)
            {
                SqlQuery_SelectPrimaryKeyName query = new SqlQuery_SelectPrimaryKeyName()
                {
                    application = Application,
                    table = this
                };
                foreach (DBItem i in query.ExecuteWithRead())
                {
                    constraintName = i["name"].ToString();
                }

            }

            Application.queries.Add(new SqlQuery_ConstraintDrop()
            {
                application = Application,
                table = this,
                constraintName = constraintName
            });
        }

        public override string ToString() => tableName;

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

        public object ConvertValue(DBColumn column, string value)
        {
            object val;
            switch (column.type.ToLower())
            {
                case "int":
                    val = Convert.ToInt32(value);
                    break;
                case "bigint":
                    val = Convert.ToInt64(value);
                    break;
                case "smallint":
                    val = Convert.ToInt16(value);
                    break;
                case "tinyint":
                    val = Convert.ToByte(value);
                    break;
                case "decimal":
                    val = Convert.ToDecimal(value);
                    break;
                case "smallmoney":
                    val = Convert.ToDecimal(value);
                    break;
                case "money":
                    val = Convert.ToDecimal(value);
                    break;
                case "float":
                    val = Convert.ToDouble(value);
                    break;
                case "real":
                    val = Convert.ToSingle(value);
                    break;
                case "date":
                    val = Convert.ToDateTime(value);
                    break;
                case "time":
                    val = TimeSpan.Parse(value);
                    break;
                case "datetime":
                    val = Convert.ToDateTime(value);
                    break;
                case "datetime2":
                    val = Convert.ToDateTime(value);
                    break;
                case "datetimeoffset":
                    val = DateTimeOffset.Parse(value);
                    break;
                case "timestamp":
                    val = Convert.ToByte(value);
                    break;
                case "varbinary":
                    val = Convert.ToByte(value);
                    break;
                case "bit":
                    val = Convert.ToBoolean(value);
                    break;
                case "binary":
                    val = Convert.ToByte(value);
                    break;
                case "uniqueidentifier":
                    val = Guid.Parse(value);
                    break;
                default:
                    val = value;
                    break;
            }

            return val;
        }

        public Condition_Operators GetConditionOperators(Conditions condition, string conditionOperator, object value)
        {
            Condition_Operators ope = new Condition_Operators(condition);
            switch (conditionOperator)
            {
                case "Equal":
                    ope.Equal(value);
                    break;
                case "Not Equal":
                    ope.NotEqual(value);
                    break;
                case "Less":
                    ope.Less(value);
                    break;
                case "Less or Equal":
                    ope.LessOrEqual(value);
                    break;
                case "Greater":
                    ope.Greater(value);
                    break;
                case "Greater or Equal":
                    ope.GreaterOrEqual(value);
                    break;
                case "Like":
                    ope.Like(value);
                    break;
                case "Not Like":
                    ope.NotLike(value);
                    break;
                //case "Between":
                //    ope.Between(value);
                //    break;
                //case "Not Between":
                //    ope.NotBetween(value);
                //    break;
                case "Null":
                    ope.Null();
                    break;
                case "Not Null":
                    ope.NotNull();
                    break;
                //case "In":
                //    ope.In(value);
                //    break;
                //case "Not In":
                //    ope.NotIn(value);
                //    break;
            }
            return ope;
        }

        public DBTable InsertSelect(List<string> tableColumns, string insertTable, 
            List<string> insertTableColumns, string whereClause)
        {
            Application.queries.Add(new SqlQuery_InsertSelect()
            {
                application = Application,
                table = this,
                columns1 = tableColumns,
                table2Name = insertTable,
                columns2 = insertTableColumns,
                where = whereClause
            });
            return this;
        }

        public List<string> getConstraints(bool isDisabled)
        {
            SqlQuery_SelectConstrains query = new SqlQuery_SelectConstrains()
            {
                application = Application,
                table = this,
                isDisable = isDisabled
            };
            List<string> constraints = new List<string>();

            foreach (DBItem i in query.ExecuteWithRead())
            {
                constraints.Add(i["name"].ToString());
            }
            return constraints;
        }

    }
}
