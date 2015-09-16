﻿using System;
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
        private List<DBColumn> _columns = null;
        private List<string> _primaryKeys = null;
        public List<string> primaryKeys
        {
            get
            {
                if (_primaryKeys == null || _primaryKeys.Count < 1)
                {
                    List<DBItem> primaryKeyItem = new SqlQuery_Table_GetPrimaryKey(ApplicationName) { tableName = tableName }.ExecuteWithRead();
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
            foreach(DBColumn column in getColumnList())
            {
                if (primaryKeys.Contains(column.Name))
                    output.Add(column);
            }

            return output;
        }

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
                new SqlQuery_Table_Drop(ApplicationName) { tableName = tableName }
                );

            return this;
        }
        public DBTable Rename(string newName)
        {
            queries.Add(
                new SqlQuery_Table_Rename(ApplicationName) { tableName = tableName, newName = newName }
                );

            return this;
        }

        public List<DBColumn> getColumnList()
        {
            if (_columns != null)
                return _columns;

            SqlQuery_Select_ColumnList query = new SqlQuery_Select_ColumnList(ApplicationName) { tableName = tableName };
            List<DBItem> items = query.ExecuteWithRead();

            _columns = items.Select(i => new DBColumn()
            {
                Name = (string)i["name"],
                type = (System.Data.SqlDbType)Enum.Parse(typeof(System.Data.SqlDbType), (string)i["typeName"]),
                maxLength = (int)i["max_length"],
                canBeNull = (bool)i["is_nullable"]
            }).ToList();

            return _columns;
        }
        public DBTable AddColumn(string columnName, System.Data.SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
        {
            SqlQuery_Table_Create query;
            if ((query = queries.GetCreate(tableName)) != null)
                query.AddColumn(columnName, type, maxLength, canBeNull, additionalOptions);
            else
                queries.Add(new SqlQuery_Column_Add(ApplicationName, tableName, new DBColumn()
                {
                    Name = columnName,
                    type = type,
                    maxLength = maxLength,
                    canBeNull = canBeNull,
                    additionalOptions = additionalOptions
                }));

            return this;
        }
        public DBTable RenameColumn(string originColumnName, string newColumnName)
        {
            queries.Add(new SqlQuery_Column_Rename(ApplicationName)
            {
                tableName = tableName,
                originColumnName = originColumnName,
                newColumnName = newColumnName
            });

            return this;
        }
        public DBTable ModifyColumn(string columnName, System.Data.SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
        {
            new SqlQuery_Column_Modify(ApplicationName)
            {
                tableName = tableName,
                column = new DBColumn()
                {
                    Name = columnName,
                    type = type,
                    maxLength = maxLength,
                    canBeNull = canBeNull,
                    additionalOptions = additionalOptions
                }
            };
            
            return this;
        }
        public DBTable DropColumn(string columnName)
        {
            queries.Add(new SqlQuery_Column_Drop(ApplicationName)
            {
                tableName = tableName,
                columnName = columnName
            });

            return this;
        }

        public DBTable Add(DBItem item)
        {
            Dictionary<DBColumn, object> data = new Dictionary<DBColumn, object>();
            foreach(DBColumn column in getColumnList())
            {
                data.Add(column, item[column.Name]);
            }

            queries.Add(new SqlQuery_Insert(ApplicationName)
            {
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
            return new SqlQuery_Select(ApplicationName) { columns = columns.ToList() };
        }
    }
}
