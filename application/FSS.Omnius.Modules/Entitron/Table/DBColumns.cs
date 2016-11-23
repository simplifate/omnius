using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Sql;
using Boolean = DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle.Boolean;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBColumns : List<DBColumn>
    {
        public DBTable table { get { return _table; } }
        private DBTable _table { get; set; }

        public DBColumns(DBTable table)
        {
            _table = table;

            // if table exists - get columns
            if (DBTable.isInDB(table.Application.Name,table.tableName))
            {
                SqlQuery_Select_ColumnList query = new SqlQuery_Select_ColumnList() { application = table.Application, table = table };

                foreach (DBItem i in query.ExecuteWithRead())
                {
                    DBColumn column = new DBColumn()
                    {
                        ColumnId = Convert.ToInt32(i["column_id"]),
                        Name = (string)i["name"],
                        type = (string)i["typeName"],
                        maxLength = Convert.ToInt32(i["max_length"]),
                        precision = Convert.ToInt32(i["precision"]),
                        scale = Convert.ToInt32(i["scale"]),
                        canBeNull = (bool)i["is_nullable"],
                    };
                    if (i["is_unique"].ToString() == "" || i["is_unique"].ToString()=="False")
                    {
                        column.isUnique = false;
                    }
                    else
                    {
                        column.isUnique = true;
                    }
                    Add(column);
                }
            }
        }

        public DBTable AddToDB(DBColumn column)
        {
            SqlQuery_Table_Create query = table.Application.queries.GetQuery<SqlQuery_Table_Create>(table.tableName);
            if (query != null)
                query.AddColumn(column);
            else
                table.Application.queries.Add(new SqlQuery_Column_Add()
                {
                    application = table.Application,
                    table = table,
                    column = column
                });

            Add(column);

            return _table;
        }
        public DBTable AddToDB(
            string columnName,
            string type,
            bool isPrimary,
            bool allowColumnLength,
            bool allowPrecisionScale,
            int? maxLength = null,
            int? precision = null,
            int? scale = null,
            bool canBeNull = true,
            bool isUnique = false,
            string additionalOptions = null)
        {
            return AddToDB(new DBColumn()
            {
                Name = columnName,
                type = type,
                isPrimary = isPrimary,
                maxLength = maxLength,
                precision = precision,
                scale = scale,
                canBeNull = canBeNull,
                isUnique = isUnique,
                additionalOptions = additionalOptions
            });
        }
        public DBTable AddRangeToDB(DBColumns columns)
        {
            foreach (DBColumn column in columns)
            {
                Add(column);
            }

            return _table;
        }

        public DBTable RenameInDB(string originColumnName, string newColumnName)
        {
            table.Application.queries.Add(new SqlQuery_Column_Rename()
            {
                application = table.Application,
                table = table,
                originColumnName = originColumnName,
                newColumnName = newColumnName
            });

            this.SingleOrDefault(c => c.Name == originColumnName).Name = newColumnName;
            return _table;
        }

        public DBTable ModifyInDB(DBColumn column)
        {
            table.Application.queries.Add(new SqlQuery_Column_Modify()
            {
                application = table.Application,
                table = table,
                column = column
            });

            this[this.IndexOf(c => c.Name == column.Name)] = column;
            return _table;
        }
        public DBTable ModifyInDB(
            string columnName,
            string type,
            int? maxLength = null,
            int? precision = null,
            int? scale = null,
            bool canBeNull = true)
        {
            return ModifyInDB(new DBColumn()
            {
                Name = columnName,
                type = type,
                maxLength = maxLength,
                precision = precision,
                scale = scale,
                canBeNull = canBeNull,
            });
        }

        public DBTable DropFromDB(string columnName)
        {
            table.Application.queries.Add(new SqlQuery_Column_Drop()
            {
                application = table.Application,
                table = table,
                columnName = columnName
            });

            Remove(this.SingleOrDefault(c => c.Name == columnName));
            return _table;
        }

        public DBTable AddDefaultValue(string column, object defValue)
        {
            table.Application.queries.Add(new SqlQuery_DefaultAdd()
            {
                application = table.Application,
                table = table,
                column = column.ToLower(),
                value = defValue
            });

            return _table;
        }

        public Dictionary<string, string> GetDefaults()
        {
            SqlQuery_SelectDefaultVal query = new SqlQuery_SelectDefaultVal() { application = table.Application, table = table };
            Dictionary<string, string> defaultVal = new Dictionary<string, string>();

            if (query.ExecuteWithRead().Count != 0)
            {
                foreach (DBItem i in query.ExecuteWithRead())
                {
                    defaultVal.Add(i["name"].ToString(), i["def"].ToString());
                }
            }
            else
            {
                defaultVal.Add(null, null);
            }

            return defaultVal;
        }

        public Dictionary<string, string> GetSpecificDefault(string columnName)
        {
            SqlQuery_SelectSpecificDefault query = new SqlQuery_SelectSpecificDefault() { application = table.Application, table = table,columnName = columnName};

            Dictionary<string, string> defaultVal = new Dictionary<string, string>();
            foreach (DBItem i in query.ExecuteWithRead())
            {
                defaultVal.Add(i["name"].ToString(), i["def"].ToString());
            }
            return defaultVal;

        }

        public DBTable AddUniqueValue(string uniqueColumns)
        {
            table.Application.queries.Add(new SqlQuery_UniqueAdd()
            {
                application = table.Application,
                table = table,
                keyColumns = uniqueColumns
            });

            return _table;
        }

        public List<string> GetUniqueConstrainst()
        {
            List<string> uniqueConstraints=new List<string>();
            SqlQuery_SelectUniqueConstraints query = new SqlQuery_SelectUniqueConstraints(){application = table.Application,table = table};
            foreach (DBItem i in query.ExecuteWithRead())
            {
                uniqueConstraints.Add(i["uniqueName"].ToString());
            }
            
            return uniqueConstraints;
        }

        public static List<string> getMaxLenghtDataTypes()
        {
            return new List<string>
            {
                "varbinary",
                "varchar",
                "binary",
                "char",
                "nvarchar",
                "nchar"
            };
            //List<string> maxLenghtDataType = new List<string>();
            //SqlQuery_SelectStringsTypes query = new SqlQuery_SelectStringsTypes();

            //foreach (DBItem s in query.ExecuteWithRead())
            //{
            //    maxLenghtDataType.Add(s["name"].ToString());
            //}
            //return maxLenghtDataType;
        }
    }
}
