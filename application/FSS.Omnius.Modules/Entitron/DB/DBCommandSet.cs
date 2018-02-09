using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System;
using MySql.Data.MySqlClient;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public abstract class DBCommandSet
    {
        public const string PrimaryKey = "id";
        public const string TablePrefix = "Entitron";

        public abstract ESqlType Type { get; }
        public abstract IDbConnection Connection { get; }
        public abstract IDbCommand Command { get; }
        public abstract string ProviderName { get; }

        public abstract string QuotesBegin { get; }
        public abstract string QuotesEnd { get; }

        public virtual string FullPrimaryKey(string tableName, bool quotes = true)
        {
            string result = $"{tableName}.{PrimaryKey}";

            if (quotes)
                return AddQuote(result);

            return result;
        }
        public virtual string AddQuote(string text)
        {
            return $"{QuotesBegin}{text}{QuotesEnd}";
        }
        public virtual List<Tuple<string, string>> ColumnsToTuple(Application application, string tableName, IEnumerable<string> columnNames)
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();

            // include table name
            foreach(string columnName in columnNames)
            {
                if (columnName.Contains('.'))
                {
                    string[] pair = columnName.Split('.');
                    result.Add(new Tuple<string, string>(pair[0], pair[1]));
                }
                else
                    result.Add(new Tuple<string, string>(tableName, columnName));
            }

            return result;
        }
        public virtual string ColumnTuplesToString(Application application, IEnumerable<Tuple<string, string>> columns, bool finalName = true, bool eachPart = true, string function = null, IEnumerable<string> functionSkipColumn = null)
        {
            List<string> result = new List<string>();
            functionSkipColumn = functionSkipColumn ?? new List<string>();

            foreach(Tuple<string, string> column in columns)
            {
                string eachPartS = eachPart
                    ? $"{ToRealTableName(application, column.Item1)}.{AddQuote(column.Item2)}"
                    : null;

                string finalNameS = finalName
                    ? AddQuote($"{column.Item1}.{column.Item2}")
                    : null;

                string part = !string.IsNullOrEmpty(function) && !functionSkipColumn.Contains($"{column.Item1}.{column.Item2}")
                    ? $"{function}({eachPartS ?? finalNameS}) {finalNameS}"
                    : $"{eachPartS} {finalNameS}";

                result.Add(part);
            }

            return string.Join(",", result);
        }
        public virtual string ToRealTableName(Application application, string tableName, bool quotes = true)
        {
            string result = $"{TablePrefix}_{application.Name}_{tableName}";

            if (quotes)
                return AddQuote(result);

            return result;
        }
        public virtual string FromRealTableName(Application application, string realTableName)
        {
            string tableName = realTableName;
            tableName = tableName.TrimStart(QuotesBegin.ToCharArray()).TrimEnd(QuotesEnd.ToCharArray());

            int iStart = tableName.IndexOf('_', TablePrefix.Length + 1) + 1;

            return tableName.Substring(iStart);
        }
        public virtual string CheckName(Application application, string tableName, string checkName, bool quotes = true)
        {
            string result = $"CHK_{ToRealTableName(application, tableName, false)}_{checkName}";

            if (quotes)
                return AddQuote(result);

            return result;
        }
        public virtual string FromCheckName(Application application, string tableName, string realCheckName, bool quotes)
        {
            string checkName = realCheckName;
            if (quotes)
                checkName = checkName.Substring(QuotesBegin.Length, checkName.Length - QuotesBegin.Length - QuotesEnd.Length);

            string realTableName = ToRealTableName(application, tableName, false);

            int iStart = checkName.IndexOf('_', 4 + realTableName.Length) + 1;

            return checkName.Substring(iStart);
        }
        public virtual string IndexName(Application application, string tableName, IEnumerable<string> columns, bool quotes = true)
        {
            string result = $"INDEX_{ToRealTableName(application, tableName, false)}_{string.Join("_", columns)}";

            if (quotes)
                return AddQuote(result);

            return result;
        }
        public virtual string ForeignKeyName(Application application, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName, bool quotes = true)
        {
            string result = $"FK_{ToRealTableName(application, sourceTableName, false)}_{sourceColumnName}__{ToRealTableName(application, targetTableName, false)}_{targetColumnName}";

            if (quotes)
                return AddQuote(result);

            return result;
        }

        public abstract string AutoIncrement { get; }

        #region DATA
        public abstract IDbCommand SELECT(DBConnection db, Tabloid table, IEnumerable<string> columnNames = null, Manager<Condition> conditions = null, Manager<Join> joins = null, Order order = null, GroupBy groupBy = null, int? limit = null, Page page = null, DropStep dropStep = null);

        public abstract IDbCommand INSERT(DBConnection db, string tableName, DBItem item);
        public abstract IDbCommand INSERT(DBConnection db, string tableName, Dictionary<string, object> item);

        public abstract IDbCommand DELETE(DBConnection db, string tableName, Manager<Condition> conditions, Manager<Join> joins = null);

        public abstract IDbCommand UPDATE(DBConnection db, string tableName, int rowId, DBItem values);

        public virtual IDbCommand SELECT_count(DBConnection db, Tabloid table, Manager<Condition> conditions = null, Manager<Join> joins = null, GroupBy groupBy = null)
        {
            IDbCommand command = Command;

            string conditionString = conditions != null && conditions.Any()
                ? $"WHERE {conditions.ToSql(this, command)}"
                : null;
            string groupByString = groupBy != null
                ? ColumnTuplesToString(db.Application, ColumnsToTuple(db.Application, table.Name, groupBy.Columns), false, true)
                : null;

            string tableString = groupBy != null
                ? $"(SELECT {groupByString} FROM {ToRealTableName(db.Application, table.Name)} GROUP BY {groupByString}) _table1"
                : ToRealTableName(db.Application, table.Name);

            command.CommandText = groupBy != null
                ? $"SELECT Count(*) count FROM (SELECT {groupByString} FROM {ToRealTableName(db.Application, table.Name)} {joins.ToSql(this, command)} {conditionString} GROUP BY {groupByString}) a"
                : $"SELECT Count(*) count FROM {ToRealTableName(db.Application, table.Name)} {joins.ToSql(this, command)} {conditionString}";

            return command;
        }

        public abstract IDbCommand ExecSP(DBConnection db, string procedureName, string parameters);
        #endregion

        #region TABLE
        public abstract IDbCommand CREATE_table(DBConnection db, DBTable table);

        public abstract IDbCommand DROP_table(DBConnection db, string tableName);

        public abstract IDbCommand RENAME_table(DBConnection db, string oldTableName, string newTableName);

        public abstract IDbCommand TRUNCATE_table(DBConnection db, string tableName);

        public abstract IDbCommand EXISTS_table(DBConnection db, string tableName);

        public abstract IDbCommand LIST_table(DBConnection db);
        #endregion

        #region TABLE COLUMN
        public abstract IDbCommand ADD_column(DBConnection db, string tableName, DBColumn column);

        public abstract IDbCommand DROP_column(DBConnection db, string tableName, string columnName);

        public abstract IDbCommand UPDATE_column(DBConnection db, string tableName, DBColumn column);

        public abstract IDbCommand RENAME_column(DBConnection db, string tableName, string oldColumnName, DBColumn newColumn);
        #endregion

        #region CONSTRAINTS
        public abstract IDbCommand ADD_default(DBConnection db, string tableName, string columnName, object value);

        public abstract IDbCommand LIST_default(DBConnection db, string tableName, bool disabled = false);

        public abstract IDbCommand DROP_default(DBConnection db, string tableName, string columnName);

        public abstract IDbCommand ADD_check(DBConnection db, string tableName, string checkName, Manager<Condition> conditions);
        public abstract IDbCommand ADD_check(DBConnection db, string tableName, string checkName, string conditions);

        public abstract IDbCommand LIST_check(DBConnection db, string tableName);

        public abstract IDbCommand DROP_check(DBConnection db, string tableName, string checkName);

        public abstract IDbCommand ADD_index(DBConnection db, string tableName, IEnumerable<string> columns, bool unique);

        /// <summary>
        /// PK is not included
        /// </summary>
        public abstract IDbCommand LIST_index(DBConnection db, string tableName);

        public abstract IDbCommand LIST_COLUMNS_index(DBConnection db, string tableName, string indexName);

        public abstract IDbCommand DROP_index(DBConnection db, string tableName, IEnumerable<string> columns);

        public abstract IDbCommand DROP_index(DBConnection db, string tableName, string indexName);

        public abstract IDbCommand ADD_foreignKey(DBConnection db, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName, string onUpdate = null, string onDelete = null);

        public abstract IDbCommand LIST_foreignKey(DBConnection db, string tableName, bool tableIsSource = true, bool tableIsTarget = true);

        public abstract IDbCommand DROP_foreignKey(DBConnection db, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName);
        #endregion

        #region VIEW
        public abstract IDbCommand CREATE_view(DBConnection db, DBView view);

        public abstract IDbCommand DROP_view(DBConnection db, string viewName);

        public abstract IDbCommand EXISTS_view(DBConnection db, string viewName);

        public abstract IDbCommand LIST_view(DBConnection db);

        public abstract IDbCommand UPDATE_view(DBConnection db, DBView view);
        #endregion

        #region Tabloid
        public abstract IDbCommand LIST_tabloid(DBConnection db, ETabloid list, string tabloidNameStartsWith = "");

        public abstract IDbCommand LIST_column(DBConnection db, string tabloidName);
        #endregion
        
        public virtual string ColumnToSql(DBConnection db, DBColumn column)
        {
            DBForeignKey foreignKey = (column.Tabloid as DBTable).ForeignKeys.SingleOrDefault(fk => fk.SourceColumn == column.Name);
            return
                $"{AddQuote(column.Name)} " +
                $"{DataType.FullDefinition(column.Type, Type, column.MaxLength)} " +
                $"{(column.IsPrimary ? AutoIncrement : "")}" +
                $"{(column.IsNullable ? " NULL" : " NOT NULL")}" +
                $"{(!string.IsNullOrEmpty(column.DefaultValue) ? $" DEFAULT {column.DefaultValue}" : "")}";
        }
        public virtual string ForeignKeyToSql(DBConnection db, DBForeignKey fk)
        {
            return $"CONSTRAINT {ForeignKeyName(db.Application, fk.SourceTable.Name, fk.SourceColumn, fk.TargetTable.Name, fk.TargetColumn)} " +
                $"FOREIGN KEY ({AddQuote(fk.SourceColumn)}) " +
                $"REFERENCES {ToRealTableName(db.Application, fk.TargetTable.Name)}({AddQuote(fk.TargetColumn)})";
        }
        public virtual string IndexToSql(DBConnection db, DBIndex index)
        {
            return $"INDEX {IndexName(db.Application, index.Table.Name, index.Columns)} ({string.Join(",", index.Columns.Select(c => AddQuote(c)))})";
        }

        public static readonly Type[] CommandSets = new Type[] { typeof(DBCommandSet_MSSQL), typeof(DBCommandSet_MySQL) };
        public static DBCommandSet GetDBCommandSet(ESqlType type)
        {
            foreach(Type commandSetType in CommandSets)
            {
                DBCommandSet commandSet = (DBCommandSet)Activator.CreateInstance(commandSetType);
                if (commandSet.Type == type)
                    return commandSet;
            }

            throw new InvalidOperationException("Unknown ESqlType");
        }
        public static ESqlType GetSqlType(string typeProvider)
        {
            foreach (Type commandSetType in CommandSets)
            {
                DBCommandSet commandSet = (DBCommandSet)Activator.CreateInstance(commandSetType);
                if (commandSet.ProviderName == typeProvider)
                    return commandSet.Type;
            }

            throw new InvalidOperationException("Unknown provider name");
        }
    }

    public static class extendCommand
    {
        public static string AddParam(this IDbCommand command, string name, object value)
        {
            string finalName = name;
            int count = 0;
            while (command.Parameters.Contains(finalName))
            {
                count++;
                finalName = $"{name}{count}";
            }

            if (command.GetType() == typeof(SqlCommand))
                (command as SqlCommand).Parameters.Add(new SqlParameter(finalName, value));
            else if (command.GetType() == typeof(MySqlCommand))
                (command as MySqlCommand).Parameters.Add(new MySqlParameter(finalName, value));
            else
                throw new InvalidOperationException();

            return finalName;
        }
    }
}
