using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace FSS.Omnius.Entitron.Sql
{
    class SqlQuery_Table_Create : SqlQuery_withApp
    {
        private List<DBColumn> _columns = new List<DBColumn>();

        public SqlQuery_Table_Create AddColumn(DBColumn column)
        {

            if (!_columns.Any(c => c.Name == column.Name))
                _columns.Add(column);

            foreach (string s in DBColumns.getMaxLenghtDataTypes())
            {
                if (column.type.ToLower() == s)
                {
                    column.allowColumnLength = true;
                    break;
                }
            }
            if (column.type == SqlDbType.Decimal.ToString() || column.type == SqlDbType.Float.ToString())
            {
                column.allowPrecisionScale = true;
            }

            return this;
        }
        public SqlQuery_Table_Create AddColumn(
            string columnName,
            string type,
            bool allowColumnLength,
            bool allowPrecisionScale,
            int? maxLength = null,
            int? precision=null,
            int? scale =null,
            bool canBeNull = true,
            bool isUnique = false,
            string additionalOptions = null)
        {
            _columns.Add(new DBColumn()
            {
                Name = columnName,
                type = type,
                allowColumnLength = allowColumnLength,
                allowPrecisionScale = allowPrecisionScale,
                maxLength = maxLength,
                precision = precision,
                scale = scale,
                canBeNull = canBeNull,
                isUnique = isUnique,
                additionalOptions = additionalOptions
            });

            return this;
        }

        public SqlQuery_Table_Create AddParameters(string parameters)
        {
            _columns.Add(new AdditionalSqlDefinition() { definition = parameters });

            return this;
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("AppName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parColumnDef = safeAddParam("columnDefinition", string.Join(",", _columns.Select(c => c.getSqlDefinition())));

            sqlString = string.Format(
                "DECLARE @_sql NVARCHAR(MAX),@_realTableName NVARCHAR(50);" +
                "SET @_realTableName=CONCAT('Entitron_',@{2},'_',(checksum(RAND())%500000)+500000);" +
                "WHILE (SELECT COUNT(*) FROM sys.tables WHERE name=@_realTableName)>0 BEGIN SET @_realTableName=CONCAT('Entitron_',@appName,'_',(checksum(RAND())%500000)+500000);END;" +
                "SET @_sql=CONCAT('CREATE TABLE ',@_realTableName,'(',@{4},');');exec(@_sql);" +
                "INSERT INTO {1}(Name,ApplicationId,tableId)VALUES(@{3},(SELECT Id FROM {0} WHERE Name=@{2}),(SELECT object_id FROM sys.tables WHERE name=@_realTableName));",
                DB_MasterApplication,
                DB_EntitronMeta,
                parAppName,
                parTableName,
                parColumnDef);
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Create table {0} in [{1}]", table.tableName, application.Name);
        }
    }
}
