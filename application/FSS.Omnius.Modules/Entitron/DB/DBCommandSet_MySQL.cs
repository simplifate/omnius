using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.DB
{
    internal class DBCommandSet_MySQL : DBCommandSet
    {
        public override ESqlType Type => ESqlType.MySQL;
        public override IDbConnection Connection => new MySqlConnection();
        public override IDbCommand Command => new MySqlCommand();

        public override string QuotesBegin => "`";
        public override string QuotesEnd => "`";

        public override string AutoIncrement => "AUTO_INCREMENT";

        public override string ForeignKeyName(Application application, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName, bool quotes = true)
        {
            string name = base.ForeignKeyName(application, sourceTableName, sourceColumnName, targetTableName, targetColumnName, false).Substring(0, 64);

            if (quotes)
                return AddQuote(name);

            return name;
        }

        private string _database;
        private string databaseName(DBConnection db)
        {
            if (_database == null)
            {
                string dbString = db.ConnectionString.Split(';').SingleOrDefault(p => p.StartsWith("database="));
                if (dbString == null)
                    throw new Exception("Database not found!");
                _database = dbString.Split('=')[1];
            }

            return _database;
        }

        #region DATA
        public override IDbCommand SELECT(DBConnection db, Tabloid tabloid, bool isSystem = false, IEnumerable<string> columnNames = null, Manager<Condition> conditions = null, Manager<Join> joins = null, Order order = null, GroupBy groupBy = null, int? limit = null, Page page = null, DropStep dropStep = null)
        {
            MySqlCommand command = new MySqlCommand();
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
                if (!string.IsNullOrEmpty(conditionString))
                {
                    conditionString =
                        $"WHERE {conditionString}";

                    if (groupBy == null)
                        command.CommandText =
                            $"SELECT * FROM __table1 {conditionString}";

                }

                // GROUP BY
                if (groupBy != null)
                {
                    string havingString = groupBy.Having.ToSql(this, command);
                    if (!string.IsNullOrEmpty(havingString))
                        havingString = $"HAVING {havingString}";

                    if (groupBy.Function == ESqlFunction.none)
                    {
                        columns = ColumnsToTuple(db.Application, tabloid.Name, groupBy.Columns);

                        command.CommandText =
                            $"SELECT {ColumnTuplesToString(db.Application, columns, true, false)} FROM __table1 {conditionString} GROUP BY {ColumnTuplesToString(db.Application, columns, true, false)} {havingString}";
                    }
                    // first, last -> inner query
                    else if (groupBy.Function.NeedsInnerQuery())
                    {
                        command.CommandText =
                            $"SELECT * FROM __table1 WHERE {conditionString} {(string.IsNullOrEmpty(conditionString) ? "" : "AND")} {FullPrimaryKey(tabloid.Name)} IN (SELECT {(groupBy.Function == ESqlFunction.FIRST ? "MIN" : "MAX")}({FullPrimaryKey(tabloid.Name)}) FROM __table1 GROUP BY {ColumnTuplesToString(db.Application, ColumnsToTuple(db.Application, tabloid.Name, groupBy.Columns), true, false)} {havingString})";
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
                            $"SELECT {ColumnTuplesToString(db.Application, columns, true, false, groupBy.Function.ToString(), groupBy.Columns)} FROM __table1 GROUP BY {ColumnTuplesToString(db.Application, ColumnsToTuple(db.Application, tabloid.Name, groupBy.Columns), true, false)} {havingString}";
                    }
                }
            }

            /// Drop step
            if (dropStep != null)
            {
                if (dropStep.Order == null)
                    throw new ArgumentException("Order needs to be filled");

                withTable.Add($"__table2 as ({command.CommandText})");
                string rowNum = $"FLOOR((@__ROWNUM:=@__ROWNUM+1)*{dropStep.FinalCount}/(SELECT Count(*) FROM __table2)) _rowNum";
                
                if (dropStep.Function == ESqlFunction.none)
                    throw new ArgumentException("DropStep with no function return no data!");

                // first, last -> inner query
                else if (dropStep.Function.NeedsInnerQuery())
                {
                    command.CommandText =
                        $"SELECT * FROM __table2 WHERE {FullPrimaryKey(tabloid.Name)} IN (SELECT {(dropStep.Function == ESqlFunction.FIRST ? "MIN" : "MAX")}({FullPrimaryKey(tabloid.Name)}) FROM (SELECT {rowNum}, __table2.* FROM __table2, (SELECT @__ROWNUM:=-1) __fooo) __t GROUP BY _rowNum)";
                }
                // other functions
                else
                {
                    command.CommandText =
                        $"SELECT {ColumnTuplesToString(db.Application, columns, true, false, dropStep.Function.ToString())} FROM (SELECT {rowNum}, __table2.* FROM __table2, (SELECT @__ROWNUM:=-1) __fooo) __t GROUP BY _rowNum";
                }
            }

            string limitString = page != null
                ? $"LIMIT {page.RowsPerPage} OFFSET {page.PageIndex * page.RowsPerPage}"
                : limit != null
                    ? $"LIMIT {limit}"
                    : "";

            command.CommandText =
                $"{(withTable.Any() ? "with " : "")}{string.Join(",", withTable)} {command.CommandText} {order?.ToSql(this, command)} {limitString}";

            return command;
        }

        public override IDbCommand INSERT(DBConnection db, string tableName, DBItem item)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"INSERT INTO {ToRealTableName(db.Application, tableName)}({string.Join(",", item.getFullColumnNames().Select(c => AddQuote(c)))}) " +
                $"VALUES ({string.Join(",", item.getFullColumnNames().Select(c => $"@{command.AddParam(c, item[c])}"))});" +
                $"SELECT LAST_INSERT_ID();";

            return command;
        }
        public override IDbCommand INSERT(DBConnection db, string tableName, Dictionary<string, object> item)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"INSERT INTO {ToRealTableName(db.Application, tableName)}({string.Join(",", item.Keys.Select(c => AddQuote(c)))}) " +
                $"VALUES ({string.Join(",", item.Keys.Select(c => $"@{command.AddParam(c, item[c])}"))});" +
                $"SELECT LAST_INSERT_ID() {PrimaryKey};";

            return command;
        }

        public override IDbCommand DELETE(DBConnection db, string tableName, Manager<Condition> conditions, Manager<Join> joins = null)
        {
            MySqlCommand command = new MySqlCommand();

            string realTableName = ToRealTableName(db.Application, tableName);

            command.CommandText =
                $"SELECT * FROM {realTableName} {joins?.ToSql(this, command)} WHERE {conditions.ToSql(this, command)};" +
                $"DELETE {realTableName} FROM {realTableName} {joins?.ToSql(this, command)} WHERE {conditions.ToSql(this, command)};";

            return command;
        }

        public override IDbCommand UPDATE(DBConnection db, string tableName, int rowId, DBItem item)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"UPDATE {ToRealTableName(db.Application, tableName)} SET {string.Join(", ", item.getFullColumnNames().Select(c => $"{AddQuote(c)}=@{command.AddParam(c, item[c])}"))} WHERE {FullPrimaryKey(tableName)}=@_{PrimaryKey}_;";

            command.Parameters.Add(new MySqlParameter($"_{PrimaryKey}_", rowId));

            return command;
        }
        
        public override IDbCommand ExecSP(DBConnection db, string procedureName, string parameters)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"CALL {procedureName}({parameters});";

            return command;
        }
        #endregion

        #region TABLE
        public override IDbCommand CREATE_table(DBConnection db, DBTable table)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"CREATE TABLE {ToRealTableName(db.Application, table.Name)} " +
                $"({string.Join(",", table.Columns.Select(c => ColumnToSql(db, c)).Concat(table.Indexes.Select(i => IndexToSql(db, i))).Concat(table.ForeignKeys.Select(fk => ForeignKeyToSql(db, fk))))}, PRIMARY KEY ({PrimaryKey}))";

            return command;
        }

        public override IDbCommand DROP_table(DBConnection db, string tableName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"DROP TABLE {ToRealTableName(db.Application, tableName)};";

            return command;
        }

        public override IDbCommand RENAME_table(DBConnection db, string oldTableName, string newTableName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"RENAME TABLE {ToRealTableName(db.Application, oldTableName)} TO {ToRealTableName(db.Application, newTableName)}";

            return command;
        }

        public override IDbCommand TRUNCATE_table(DBConnection db, string tableName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"TRUNCATE TABLE {ToRealTableName(db.Application, tableName)}";

            return command;
        }

        public override IDbCommand EXISTS_table(DBConnection db, string tableName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT Count(*) `exists` FROM information_schema.`TABLES` WHERE TABLE_SCHEMA='{databaseName(db)}' AND TABLE_NAME='{ToRealTableName(db.Application, tableName, false)}'";

            return command;
        }

        public override IDbCommand LIST_table(DBConnection db)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT TABLE_NAME `name` FROM information_schema.`TABLES` WHERE TABLE_SCHEMA='{databaseName(db)}' AND TABLE_NAME LIKE '{ToRealTableName(db.Application, "", false)}%'";

            return command;
        }
        #endregion

        #region TABLE COLUMN
        public override IDbCommand ADD_column(DBConnection db, string tableName, DBColumn column)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ADD {ColumnToSql(db, column)}";

            return command;
        }

        public override IDbCommand DROP_column(DBConnection db, string tableName, string columnName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} DROP COLUMN {AddQuote(columnName)}";

            return command;
        }

        public override IDbCommand UPDATE_column(DBConnection db, string tableName, DBColumn column)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} MODIFY COLUMN {ColumnToSql(db, column)}";

            return command;
        }

        public override IDbCommand RENAME_column(DBConnection db, string tableName, string oldColumnName, DBColumn newColumn)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} CHANGE COLUMN {AddQuote(oldColumnName)} {ColumnToSql(db, newColumn)}";

            return command;
        }
        #endregion

        #region CONSTRAINTS
        public override IDbCommand ADD_default(DBConnection db, string tableName, string columnName, object value)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ALTER {AddQuote(columnName)} SET DEFAULT {value};";

            return command;
        }

        public override IDbCommand LIST_default(DBConnection db, string tableName, bool disabled = false)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT COLUMN_DEFAULT `value` FROM information_schema.`COLUMNS` WHERE TABLE_NAME='{ToRealTableName(db.Application, tableName, false)}' AND COLUMN_DEFAULT IS NOT NULL;";

            return command;
        }

        public override IDbCommand DROP_default(DBConnection db, string tableName, string columnName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ALTER {AddQuote(columnName)} DROP DEFAULT;";

            return command;
        }

        public override IDbCommand ADD_check(DBConnection db, string tableName, string checkName, Manager<Condition> conditions)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ADD CONSTRAINT {CheckName(db.Application, tableName, checkName)} CHECK ({conditions.ToSql(this, command)})";

            return command;
        }
        public override IDbCommand ADD_check(DBConnection db, string tableName, string checkName, string conditions)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} ADD CONSTRAINT {CheckName(db.Application, tableName, checkName)} CHECK ({conditions})";

            return command;
        }

        public override IDbCommand LIST_check(DBConnection db, string tableName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT CONSTRAINT_NAME `name` FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='CHECK' AND TABLE_NAME='{ToRealTableName(db.Application, tableName, false)}'";

            return command;
        }

        public override IDbCommand DROP_check(DBConnection db, string tableName, string checkName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} DROP CHECK {CheckName(db.Application, tableName, checkName)}";

            return command;
        }

        public override IDbCommand ADD_index(DBConnection db, string tableName, IEnumerable<string> columns, bool unique)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"CREATE {(unique ? "UNIQUE" : "")} INDEX {IndexName(db.Application, tableName, columns)} ON {ToRealTableName(db.Application, tableName)}({string.Join(",", columns.Select(c => AddQuote(c)))})";

            return command;
        }

        public override IDbCommand LIST_index(DBConnection db, string tableName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT INDEX_NAME name, if(MAX(NON_UNIQUE) = 0, 1, 0) is_unique FROM information_schema.STATISTICS " +
                $"WHERE INDEX_NAME<>'PRIMARY' AND INDEX_NAME NOT LIKE 'FK_%' AND INDEX_SCHEMA='{databaseName(db)}' AND TABLE_NAME='{ToRealTableName(db.Application, tableName, false)}' " +
                $"GROUP BY INDEX_SCHEMA, INDEX_NAME";

            return command;
        }

        public override IDbCommand LIST_COLUMNS_index(DBConnection db, string tableName, string indexName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT COLUMN_NAME ColumnName FROM information_schema.STATISTICS WHERE TABLE_NAME='{ToRealTableName(db.Application, tableName, false)}' AND INDEX_NAME='{indexName}'";

            return command;
        }

        public override IDbCommand DROP_index(DBConnection db, string tableName, string indexName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, tableName)} DROP INDEX {AddQuote(indexName)};";

            return command;
        }

        public override IDbCommand DROP_index(DBConnection db, string tableName, IEnumerable<string> columns)
        {
            return DROP_index(db, tableName, IndexName(db.Application, tableName, columns, false));
        }

        public override IDbCommand ADD_foreignKey(DBConnection db, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName, string onUpdate = null, string onDelete = null)
        {
            MySqlCommand command = new MySqlCommand();

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
            MySqlCommand command = new MySqlCommand();

            string realTableName = ToRealTableName(db.Application, tableName, false);
            string where = "";
            if (tableIsSource)
                where = $"cu.TABLE_NAME='{realTableName}'";
            if (tableIsTarget)
            {
                if (tableIsSource)
                    where += " OR ";

                where += $"cu.REFERENCED_TABLE_NAME='{realTableName}'";
            }
            // nothing selected
            if (!tableIsSource && !tableIsTarget)
                where = "1=0";

            command.CommandText =
                $"SELECT cu.TABLE_NAME sourceTable, cu.COLUMN_NAME sourceColumn, cu.REFERENCED_TABLE_NAME targetTable, cu.REFERENCED_COLUMN_NAME targetColumn, rc.DELETE_RULE onDelete, rc.UPDATE_RULE onUpdate FROM information_schema.KEY_COLUMN_USAGE cu " +
                $"JOIN information_schema.REFERENTIAL_CONSTRAINTS rc ON cu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME AND cu.CONSTRAINT_SCHEMA = rc.CONSTRAINT_SCHEMA " +
                $"WHERE cu.TABLE_SCHEMA='{databaseName(db)}' AND cu.CONSTRAINT_NAME LIKE 'FK_%' AND ({where})";

            return command;
        }

        public override IDbCommand DROP_foreignKey(DBConnection db, string sourceTableName, string sourceColumnName, string targetTableName, string targetColumnName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"ALTER TABLE {ToRealTableName(db.Application, sourceTableName)} DROP FOREIGN KEY {ForeignKeyName(db.Application, sourceTableName, sourceColumnName, targetTableName, targetColumnName)}";

            return command;
        }
        #endregion

        #region VIEW
        public override IDbCommand CREATE_view(DBConnection db, DBView view)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"CREATE VIEW {ToRealTableName(db.Application, view.Name)} AS {view.Sql}";

            return command;
        }

        public override IDbCommand DROP_view(DBConnection db, string viewName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"DROP VIEW {ToRealTableName(db.Application, viewName)}";

            return command;
        }

        public override IDbCommand EXISTS_view(DBConnection db, string viewName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT Count(*) `exists` FROM information_schema.VIEWS WHERE TABLE_NAME='{ToRealTableName(db.Application, viewName, false)}'";

            return command;
        }

        public override IDbCommand LIST_view(DBConnection db)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"SELECT TABLE_NAME `name` FROM information_schema.VIEWS WHERE TABLE_NAME LIKE '{ToRealTableName(db.Application, "", false)}%'";

            return command;
        }

        public override IDbCommand UPDATE_view(DBConnection db, DBView view)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                $"CREATE OR REPLACE VIEW {ToRealTableName(db.Application, view.Name)} AS {view.Sql}";

            return command;
        }
        #endregion

        #region Tabloid
        public override IDbCommand LIST_tabloid(DBConnection db, ETabloid list, string tabloidName = "")
        {
            MySqlCommand command = new MySqlCommand();

            string emptyTableName = ToRealTableName(db.Application, tabloidName, false);
            string sysTableName = ToRealTableName(Application.SystemApp(), tabloidName, false);
            List<string> query = new List<string>();

            if (list.Contains(ETabloid.ApplicationTables))
                query.Add($"SELECT TABLE_NAME name, 'table' source FROM information_schema.TABLES WHERE TABLE_SCHEMA='{databaseName(db)}' AND TABLE_TYPE='BASE TABLE' AND TABLE_NAME LIKE '{emptyTableName}{(string.IsNullOrEmpty(tabloidName) ? "%" : "")}'");
            if (list.Contains(ETabloid.SystemTables))
                query.Add($"SELECT TABLE_NAME name, 'system' source FROM information_schema.TABLES WHERE TABLE_SCHEMA='{databaseName(db)}' AND TABLE_NAME LIKE '{sysTableName}{(string.IsNullOrEmpty(tabloidName) ? "%" : "")}'");
            if (list.Contains(ETabloid.Views))
                query.Add($"SELECT TABLE_NAME name, 'view' source FROM information_schema.VIEWS WHERE TABLE_SCHEMA='{databaseName(db)}' AND TABLE_NAME LIKE '{emptyTableName}{(string.IsNullOrEmpty(tabloidName) ? "%" : "")}'");

            command.CommandText =
                string.Join(" UNION ALL ", query);

            return command;
        }
        
        public override IDbCommand LIST_column(DBConnection db, string tabloidName)
        {
            MySqlCommand command = new MySqlCommand();

            command.CommandText =
                "SELECT c.COLUMN_NAME name, DATA_TYPE typeName, CHARACTER_MAXIMUM_LENGTH max_length, IS_NULLABLE='YES' is_nullable, 0 is_unique, COLUMN_DEFAULT `default` FROM information_schema.`COLUMNS` c " +
                $"WHERE c.TABLE_SCHEMA='{databaseName(db)}' AND c.TABLE_NAME='{ToRealTableName(db.Application, tabloidName, false)}'";

            return command;
        }
        #endregion
        
        public override string IndexToSql(DBConnection db, DBIndex index)
        {
            return $"{(index.isUnique ? "UNIQUE" : "")} KEY {IndexName(db.Application, index.Table.Name, index.Columns)} ({string.Join(",", index.Columns.Select(c => AddQuote(c)))})";
        }
    }
}
