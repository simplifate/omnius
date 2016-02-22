using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace FSS.Omnius.Modules.Entitron.Sql
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
            if (column.type == SqlDbType.Decimal.ToString())
            {
                column.allowPrecisionScale = true;
            }

            return this;
        }
        public SqlQuery_Table_Create AddColumn(
            string columnName,
            string type,
            bool isPrimary,
            bool allowColumnLength,
            bool allowPrecisionScale,
            bool canBeNull,
            bool isUnique,
            int? maxLength = null,
            int? precision=null,
            int? scale =null, 
            string additionalOptions = null)
        {
            _columns.Add(new DBColumn()
            {
                Name = columnName,
                type = type,
                isPrimary = isPrimary,
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

            string columnDefinition = string.Join(",", _columns.Select(c => c.getSqlDefinition()));
            string realTableName = $"Entitron_{application.Name}_{table.tableName}";

            sqlString =
                $"CREATE TABLE [{realTableName}] ({columnDefinition});" +
                $"INSERT INTO {DB_EntitronMeta} ( Name, ApplicationId, tableId) VALUES ( @{parTableName}, ( SELECT Id FROM {DB_MasterApplication} WHERE Name= @{parAppName} ) , ( SELECT object_id FROM sys.tables WHERE name='{realTableName}') );";
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Create table {0} in [{1}]", table.tableName, application.Name);
        }
    }
}
