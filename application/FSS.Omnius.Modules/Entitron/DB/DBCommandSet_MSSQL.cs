using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.DB
{
    internal class DBCommandSet_MSSQL : DBCommandSet
    {
        public override ESqlType Type => ESqlType.MSSQL;
        public override IDbConnection Connection => new SqlConnection();
        public override IDbCommand Command => new SqlCommand();
        public override string ProviderName => "System.Data.SqlClient";

        public override string QuotesBegin => "[";
        public override string QuotesEnd => "]";

        public override string AutoIncrement => "IDENTITY(1, 1) PRIMARY KEY";

        public virtual string DefaultName(Application application, string tableName, string columnName, bool quotes = true)
        {
            string result = $"DEF_{ToRealTableName(application, tableName, false)}_{columnName}";

            if (quotes)
                return AddQuote(result);

            return result;
        }

        #region DATA
        public override IDbCommand SELECT(DBConnection db, Tabloid tabloid, IEnumerable<string> columnNames = null, Manager<Condition> conditions = null, Manager<Join> joins = null, Order order = null, GroupBy groupBy = null, int? limit = null, Page page = null, DropStep dropStep = null)
        {
            SqlCommand command = new SqlCommand();
            List<string> withTable = new List<string>();

            /// get columns to select
            List<Tuple<string, string>> columns = new List<Tuple<string, string>>();
            // column are set
            if (columnNames != null && columnNames.Any())
                columns.AddRange(ColumnsToTuple(db.Application, tabloid.Name, columnNames));
            // all columns
            else
            {
                // origin table
                columns.AddRange(tabloid.Columns.Select(c => new Tuple<string, string>(tabloid.Name, c.Name)));
                // joined tables
                foreach (Join join in joins)
                {
                    columns.AddRange(new Tabloid(db) { Name = join.joinTableName }.Columns.Select(c => new Tuple<string, string>(join.joinTableName, c.Name)));
                }
            }

            /// SELECT FROM, JOIN
            command.CommandText =
                $"SELECT {ColumnTuplesToString(db.Application, columns)} FROM {ToRealTableName(db.Application, tabloid.Name)} {joins.ToSql(this, command)}";

            /// WHERE, GROUP, HAVING
            string conditionString = conditions.ToSql(this, command);
            if (!string.IsNullOrEmpty(conditionString) || groupBy != null)
            {
                withTable.Add($"__table1 as ({command.CommandText})");

                // WHERE
                if (!string.IsNullOrEmpty(conditionString) && groupBy == null)
                {
                    command.CommandText =
                        $"SELECT * FROM __table1 {(!string.IsNullOrEmpty(conditionString) ? "WHERE" : "")} {conditionString}";
                }

                // GROUP BY
                if (groupBy != null)
                {
                    string havingString = groupBy.Having.ToSql(this, command);

                    if (groupBy.Function == ESqlFunction.none)
                    {
                        columns = ColumnsToTuple(db.Application, tabloid.Name, groupBy.Columns);

                        command.CommandText =
                            $"SELECT {ColumnTuplesToString(db.Application, columns, true, false)} FROM __table1 {(!string.IsNullOrEmpty(conditionString) || !string.IsNullOrEmpty(havingString) ? "WHERE" : "")} {conditionString} {(!string.IsNullOrEmpty(conditionString) && !string.IsNullOrEmpty(havingString) ? "AND" : "")} {havingString} GROUP BY {ColumnTuplesToString(db.Application, columns, true, false)}";
                    }
                    // first, last -> inner query
                    else if (groupBy.Function.NeedsInnerQuery())
                    {
                        command.CommandText =
                            $"SELECT * FROM __table1 WHERE {conditionString} {(!string.IsNullOrEmpty(conditionString) ? "AND" : "")} {havingString} {(!string.IsNullOrEmpty(havingString) ? "AND" : "")} {FullPrimaryKey(tabloid.Name)} IN (SELECT {(groupBy.Function == ESqlFunction.FIRST ? "MIN" : "MAX")}({FullPrimaryKey(tabloid.Name)}) FROM __table1 {(!string.IsNullOrEmpty(conditionString) || !string.IsNullOrEmpty(havingString) ? "WHERE" : "")} {conditionString} {(!string.IsNullOrEmpty(conditionString) && !string.IsNullOrEmpty(havingString) ? "AND" : "")} {havingString} GROUP BY {ColumnTuplesToString(db.Application, ColumnsToTuple(db.Application, tabloid.Name, groupBy.Columns), true, false)})";
                    }
                    // other functions
                    else
                    {
                        // get only numeric columns
                        if (groupBy.Function.RequireNumeric())
                        {
                            List<Tuple<string, string>> currentColumns = ColumnsToTuple(db.Application, tabloid.Name, groupBy.Columns);
                            foreach (string tabloidName in columns.Select(c => c.Item1).Distinct())
                            {
                                Tabloid currentTabloid = tabloidName == tabloid.Name
                                    ? tabloid
                                    : new Tabloid(db) { Name = tabloidName };

                                currentColumns = currentColumns.Concat(ColumnsToTuple(db.Application, currentTabloid.Name, currentTabloid.Columns.Where(c => columns.Contains(new Tuple<string, string>(currentTabloid.Name, c.Name)) && DataType.IsNumeric(c.Type)).Select(c => c.Name))).ToList();
                            }

                            columns = currentColumns;
                        }

                        command.CommandText =
                            $"SELECT {ColumnTuplesToString(db.Application, columns, true, false, groupBy.Function.ToString(), groupBy.Columns)} FROM __table1 GROUP BY {ColumnTuplesToString(db.Application, ColumnsToTuple(db.Application, tabloid.Name, groupBy.Columns), true, false)}";
                        if (!string.IsNullOrEmpty(havingString))
                        {
                            withTable.Add($"__table3 as ({command.CommandText})");

                            command.CommandText =
                                $"SELECT * FROM __table3 WHERE {havingString}";
                        }
                    }
                }
            }

            /// Drop step
            if (dropStep != null)
            {
                if (dropStep.Order == null)
                    throw new ArgumentException("Order needs to be filled");

                withTable.Add($"__table2 as ({command.CommandText})");
                string rowNum = $"((ROW_NUMBER() OVER ({dropStep.Order.ToSql(this, command)})-1)*{dropStep.FinalCount}/(SELECT Count(*) FROM __table2)) _rowNum";

                if (dropStep.Function == ESqlFunction.none)
                    throw new ArgumentException("DropStep with no function return no data!");

                // first, last -> inner query
                else if (dropStep.Function.NeedsInnerQuery())
                {
                    command.CommandText =
                        $"SELECT * FROM __table2 WHERE {FullPrimaryKey(tabloid.Name)} IN (SELECT {(dropStep.Function == ESqlFunction.FIRST ? "MIN" : "MAX")}({FullPrimaryKey(tabloid.Name)}) FROM (SELECT {rowNum}, __table2.* FROM __table2) __t GROUP BY _rowNum)";
                }
                // other functions
                else
                {
                    command.CommandText =
                        $"SELECT {ColumnTuplesToString(db.Application, columns, true, false, dropStep.Function.ToString())} FROM (SELECT {rowNum}, __table2.* FROM __table2) __t GROUP BY _rowNum";
                }
            }

            // limit or page - not both at the same time
            string limitString = limit != null && !(page != null && order != null)
                    ? $"TOP ({limit}) "
                    : "";

            string pageString = (page != null && order != null)
                ? pageString = $"OFFSET {page.PageIndex * page.RowsPerPage} ROWS FETCH NEXT {page.RowsPerPage} ROWS ONLY "
                : "";

            command.CommandText =
                $"{(withTable.Any() ? "with " : "")}{string.Join(",", withTable)} {command.CommandText.Insert("SELECT ".Length, limitString)} {order?.ToSql(this, command)} {pageString}";

            return command;
        }
        public override IDbCommand INSERT(DBConnection db, string tableName, Dictionary<string, object> item)
        {
            SqlCommand command = new SqlCommand();

            IEnumerable<string> columnNames = item.Keys.Except(new string[] { PrimaryKey, FullPrimaryKey(tableName, false) });

            command.CommandText =
                $"INSERT INTO {ToRealTableName(db.Application, tableName)}({string.Join(",", columnNames.Select(c => AddQuote(c)))}) " +
                $"OUTPUT INSERTED.{PrimaryKey} " +
                $"VALUES ({string.Join(",", columnNames.Select(key => $"@{command.AddParam(key, item[key])}"))});";

            return command;
        }
        public override IDbCommand INSERT(DBConnection db, string tableName, DBItem item)
        {
            SqlCommand command = new SqlCommand();

            IEnumerable<string> columnNames = item.getColumnNames().Except(new string[] { PrimaryKey, FullPrimaryKey(tableName, false) });

            command.CommandText =
                $"INSERT INTO {ToRealTableName(db.Application, tableName)}({string.Join(",", columnNames.Select(c => AddQuote(c)))}) " +
                $"OUTPUT INSERTED.{PrimaryKey} " +
                $"VALUES ({string.Join(",", columnNames.Select(key => $"@{command.AddParam(key, item[key])}"))});";

            return command;
        }

        public override IDbCommand DELETE(DBConnection db, string tableName, Manager<Condition> conditions, Manager<Join> joins = null)
        {
            SqlCommand command = new SqlCommand();

            string realTableName = ToRealTableName(db.Application, tableName);

            string where = conditions.ToSql(this, command);
            if (!string.IsNullOrEmpty(where))
                where = $"WHERE {where}";

            command.CommandText =
                $"DELETE {realTableName} OUTPUT DELETED.* FROM {realTableName} {joins?.ToSql(this, command)} {where}";

            return command;
        }

        public override IDbCommand UPDATE(DBConnection db, string tableName, int rowId, DBItem item)
        {
            SqlCommand command = new SqlCommand();

            var columnTuples = ColumnsToTuple(db.Application, tableName, item.getFullColumnNames()).Where(ct => ct.Item2 != PrimaryKey);

            command.CommandText =
                $"UPDATE {ToRealTableName(db.Application, tableName)} SET {string.Join(", ", columnTuples.Select(pair => $"{ToRealTableName(db.Application, pair.Item1)}.{AddQuote(pair.Item2)}=@{command.AddParam(pair.Item2, item[$"{pair.Item1}.{pair.Item2}"])}"))} WHERE {ToRealTableName(db.Application, tableName)}.{AddQuote(PrimaryKey)}=@_{PrimaryKey}_;";

            command.Parameters.Add(new SqlParameter($"_{PrimaryKey}_", rowId));

            return command;
        }

        public override IDbCommand ExecSP(DBConnection db, string procedureName, string parameters)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"EXEC {procedureName} {parameters};";

            return command;
        }
        #endregion

        #region TABLE
        public override IDbCommand CREATE_table(DBConnection db, DBTable table)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"CREATE TABLE {ToRealTableName(db.Application, table.Name)} " +
                $"({string.Join(",", table.Columns.Select(c => ColumnToSql(db, c)).Concat(table.Indexes.Select(i => IndexToSql(db, i))).Concat(table.ForeignKeys.Select(fk => ForeignKeyToSql(db, fk))))})";

            return command;
        }

        public override IDbCommand DROP_table(DBConnection db, string tableName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"DROP TABLE {ToRealTableName(db.Application, tableName)};";

            return command;
        }

        public override IDbCommand RENAME_table(DBConnection db, string oldTableName, string newTableName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"exec sp_rename '{ToRealTableName(db.Application, oldTableName, false)}', '{ToRealTableName(db.Application, newTableName, false)}'";

            return command;
        }

        public override IDbCommand TRUNCATE_table(DBConnection db, string tableName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"TRUNCATE TABLE {ToRealTableName(db.Application, tableName)}";

            return command;
        }

        public override IDbCommand EXISTS_table(DBConnection db, string tableName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT Count(*) exists FROM sys.tables WHERE name='{ToRealTableName(db.Application, tableName, false)}'";

            return command;
        }

        public override IDbCommand LIST_table(DBConnection db)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT name FROM sys.tables WHERE name LIKE '{ToRealTableName(db.Application, "", false)}%'";

            return command;
        }
        #endregion

        #region TABLE COLUMN
        public override IDbCommand ADD_column(DBConnection db, string tableName, DBColumn column)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ADD {ColumnToSql(db, column)}";

            return command;
        }

        public override IDbCommand DROP_column(DBConnection db, string tableName, string columnName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} DROP COLUMN {AddQuote(columnName)}";

            return command;
        }

        public override IDbCommand UPDATE_column(DBConnection db, string tableName, DBColumn column)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ALTER COLUMN {ColumnToSql(db, column)}";

            return command;
        }

        public override IDbCommand RENAME_column(DBConnection db, string tableName, string oldColumnName, DBColumn newColumn)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"sp_rename '{ToRealTableName(db.Application, tableName, false)}.{oldColumnName}', '{newColumn.Name}', 'COLUMN';";

            return command;
        }
        #endregion

        #region CONSTRAINTS
        public override IDbCommand ADD_default(DBConnection db, string tableName, string columnName, object value)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ADD CONSTRAINT {DefaultName(db.Application, tableName, columnName)} DEFAULT {value} FOR {AddQuote(columnName)}";

            return command;
        }

        public override IDbCommand LIST_default(DBConnection db, string tableName, bool disabled = false)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT * FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID('{ToRealTableName(db.Application, tableName, false)}')";

            return command;
        }

        public override IDbCommand DROP_default(DBConnection db, string tableName, string columnName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} DROP CONSTRAINT {DefaultName(db.Application, tableName, columnName)}";

            return command;
        }

        public override IDbCommand ADD_check(DBConnection db, string tableName, string checkName, Manager<Condition> conditions)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ADD CONSTRAINT {CheckName(db.Application, tableName, checkName)} CHECK {conditions.ToSql(this, command)}";

            return command;
        }
        public override IDbCommand ADD_check(DBConnection db, string tableName, string checkName, string conditions)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ADD CONSTRAINT {CheckName(db.Application, tableName, checkName)} CHECK {conditions}";

            return command;
        }

        public override IDbCommand LIST_check(DBConnection db, string tableName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT * FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('{ToRealTableName(db.Application, tableName, false)}')";

            return command;
        }

        public override IDbCommand DROP_check(DBConnection db, string tableName, string checkName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} DROP CONSTRAINT {CheckName(db.Application, tableName, checkName)}";

            return command;
        }

        public override IDbCommand ADD_index(DBConnection db, string tableName, IEnumerable<string> columns, bool unique)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"CREATE {(unique ? "UNIQUE" : "")} INDEX {IndexName(db.Application, tableName, columns)} ON {ToRealTableName(db.Application, tableName)}({string.Join(",", columns.Select(c => AddQuote(c)))})";

            return command;
        }

        public override IDbCommand LIST_index(DBConnection db, string tableName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT * FROM sys.indexes WHERE is_primary_key=0 AND object_id=object_id('{ToRealTableName(db.Application, tableName, false)}')";

            return command;
        }

        public override IDbCommand LIST_COLUMNS_index(DBConnection db, string tableName, string indexName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT i.Name IndexName , c.Name ColumnName FROM sys.indexes i " +
                $"INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id " +
                $"INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id " +
                $"INNER JOIN sys.tables t ON i.object_id = t.object_id " +
                $"WHERE i.is_primary_key = 0 AND i.name='{indexName}' AND t.name='{ToRealTableName(db.Application, tableName, false)}';";

            return command;
        }

        public override IDbCommand DROP_index(DBConnection db, string tableName, string indexName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"DROP INDEX {AddQuote(indexName)} ON {ToRealTableName(db.Application, tableName)};";

            return command;
        }

        public override IDbCommand DROP_index(DBConnection db, string tableName, IEnumerable<string> columns)
        {
            return DROP_index(db, tableName, IndexName(db.Application, tableName, columns, false));
        }

        public override IDbCommand ADD_foreignKey(DBConnection db, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName, string onUpdate = null, string onDelete = null)
        {
            SqlCommand command = new SqlCommand();

            string update = (onUpdate == "cascade")
                ? " ON UPDATE CASCADE"
                : (onUpdate == "null")
                    ? " ON UPDATE SET NULL"
                    : (onUpdate == "default") ? " ON UPDATE SET DEFAULT" : ""; ;
            string delete = (onDelete == "cascade")
                ? " ON DELETE CASCADE"
                : (onDelete == "null")
                    ? " ON DELETE SET NULL"
                    : (onDelete == "default") ? " ON DELETE SET DEFAULT" : "";


            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, sourceTableName)} ADD CONSTRAINT {ForeignKeyName(db.Application, sourceTableName, sourceColumnName, targetTableName, targetColumnName)} " +
                $"FOREIGN KEY ({AddQuote(sourceColumnName)}) REFERENCES {ToRealTableName(db.Application, targetTableName)}({AddQuote(targetColumnName)}) " +
                $"{delete} {update};";

            return command;
        }

        public override IDbCommand LIST_foreignKey(DBConnection db, string tableName, bool tableIsSource = true, bool tableIsTarget = true)
        {
            SqlCommand command = new SqlCommand();

            string realTableName = ToRealTableName(db.Application, tableName, false);
            string where = "";
            if (tableIsSource)
                where = $"fc.parent_object_id=OBJECT_ID('{realTableName}')";
            if (tableIsTarget)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " OR ";

                where += $"fc.referenced_object_id=OBJECT_ID('{realTableName}')";
            }
            // nothing selected
            if (!tableIsSource && !tableIsTarget)
                where = "1=0";

            command.CommandText =
                $"SELECT OBJECT_NAME(fc.constraint_object_id) name, OBJECT_NAME(fc.parent_object_id) sourceTable, sc.name sourceColumn, OBJECT_NAME(fc.referenced_object_id) targetTable, tc.name targetColumn, " +
                $"f.delete_referential_action_desc onDelete, f.update_referential_action_desc onUpdate FROM [sys].[foreign_key_columns] fc " +
                $"join sys.foreign_keys f ON f.object_id = fc.constraint_object_id " +
                $"JOIN sys.all_columns sc ON sc.object_id = fc.parent_object_id AND sc.column_id = fc.parent_column_id " +
                $"JOIN sys.all_columns tc ON tc.object_id = fc.referenced_object_id AND tc.column_id = fc.referenced_column_id " +
                $"WHERE {where}";

            return command;
        }

        public override IDbCommand DROP_foreignKey(DBConnection db, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, sourceTableName)} DROP CONSTRAINT {ForeignKeyName(db.Application, sourceTableName, sourceColumnName, targetTableName, targetColumnName)}";

            return command;
        }
        #endregion

        #region VIEW
        public override IDbCommand CREATE_view(DBConnection db, DBView view)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"CREATE VIEW {ToRealTableName(db.Application, view.Name)} AS {view.Sql}";

            return command;
        }

        public override IDbCommand DROP_view(DBConnection db, string viewName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"DROP VIEW {ToRealTableName(db.Application, viewName)}";

            return command;
        }

        public override IDbCommand EXISTS_view(DBConnection db, string viewName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT Count(*) exists FROM [sys].[views] WHERE [name]='{ToRealTableName(db.Application, viewName, false)}'";

            return command;
        }

        public override IDbCommand LIST_view(DBConnection db)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"SELECT * FROM [sys].[views] WHERE [name] LIKE '{ToRealTableName(db.Application, "", false)}%'";

            return command;
        }

        public override IDbCommand UPDATE_view(DBConnection db, DBView view)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                $"ALTER VIEW {ToRealTableName(db.Application, view.Name)} AS {view.Sql}";

            return command;
        }
        #endregion

        #region Tabloid
        public override IDbCommand LIST_tabloid(DBConnection db, ETabloid list, string tabloidName = "")
        {
            SqlCommand command = new SqlCommand();

            string emptyTableName = ToRealTableName(db.Application, tabloidName, false);
            string sysTableName = ToRealTableName(Application.SystemApp(), tabloidName, false);
            List<string> query = new List<string>();

            if (list.Contains(ETabloid.ApplicationTables))
                query.Add($"SELECT name, 'table' [source] FROM sys.tables WHERE name LIKE '{emptyTableName}{(string.IsNullOrEmpty(tabloidName) ? "%" : "")}'");
            if (list.Contains(ETabloid.SystemTables))
                query.Add($"SELECT name, 'system' [source] FROM sys.tables WHERE name LIKE '{sysTableName}{(string.IsNullOrEmpty(tabloidName) ? "%" : "")}'");
            if (list.Contains(ETabloid.Views))
                query.Add($"SELECT name, 'view' [source] FROM sys.views WHERE name LIKE '{emptyTableName}{(string.IsNullOrEmpty(tabloidName) ? "%" : "")}'");

            command.CommandText =
                string.Join(" UNION ALL ", query);

            return command;
        }

        public override IDbCommand LIST_column(DBConnection db, string tabloidName)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText =
                "SELECT DISTINCT columns.name [name], types.name [typeName], (columns.max_length / 2) max_length, columns.scale, columns.is_nullable, i.is_unique_constraint [is_unique], d.definition [default] FROM sys.columns columns " +
                "JOIN sys.types types ON columns.user_type_id = types.user_type_id " +
                "left join sys.index_columns ic on columns.object_id = ic.object_id and columns.column_id = ic.column_id " +
                "left join sys.indexes i on i.index_id = ic.index_id AND i.object_id = columns.object_id " +
                "left join sys.default_constraints d ON columns.default_object_id = d.object_id " +
                $"WHERE columns.object_id = OBJECT_ID('{ToRealTableName(db.Application, tabloidName, false)}')";

            return command;
        }
        #endregion
        
        public override string ColumnToSql(DBConnection db, DBColumn column)
        {
            DBForeignKey foreignKey = (column.Tabloid as DBTable).ForeignKeys.SingleOrDefault(fk => fk.SourceColumn == column.Name);
            return
                $"{AddQuote(column.Name)} " +
                $"{DataType.FullDefinition(column.Type, Type, column.MaxLength)} " +
                $"{(column.IsPrimary ? AutoIncrement : "")}" +
                $"{(column.IsNullable ? " NULL" : " NOT NULL")}" +
                $"{(!string.IsNullOrEmpty(column.DefaultValue) ? $" CONSTRAINT {DefaultName(db.Application, column.Tabloid.Name, column.Name)} DEFAULT {column.DefaultValue}" : "")}";
        }
        public override string IndexToSql(DBConnection db, DBIndex index)
        {
            return $"INDEX {IndexName(db.Application, index.Table.Name, index.Columns)} {(index.isUnique ? "UNIQUE" : "")} NONCLUSTERED ({string.Join(",", index.Columns.Select(c => AddQuote(c)))})";
        }
    }
}
