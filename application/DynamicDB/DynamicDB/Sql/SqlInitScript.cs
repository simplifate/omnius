using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlInitScript : SqlQuery
    {
        internal const string aplicationTableName = "dbo.Applications";

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _sqlString =
                "CREATE PROCEDURE getTableRealNameWithMeta @applicationName NVARCHAR(50), @tableName NVARCHAR(50), @tableRealName NVARCHAR(50) OUTPUT, @DbTablePrefix NVARCHAR(50) OUTPUT, @DbMetaTables NVARCHAR(50) OUTPUT AS " +
                "DECLARE @tableId INT, @sql NVARCHAR(MAX);" +
                "SELECT @DbTablePrefix = DbTablePrefix, @DbMetaTables = DbMetaTables FROM " + aplicationTableName + " WHERE Name = @applicationName;" +
                "SET @sql = CONCAT('SELECT @tableId=Id FROM ', @DbMetaTables, ' WHERE Name = @tableName');" +
                "exec sp_executesql @sql, N'@tableId Int output, @tableName NVARCHAR(50)', @tableId output, @tableName; SET @tableRealName = CONCAT(@DbTablePrefix, @tableId); ";
            base.BaseExecution(transaction);

            _sqlString =
                "CREATE PROCEDURE getTableRealName @applicationName NVARCHAR(50), @tableName NVARCHAR(50), @tableRealName NVARCHAR(50) OUTPUT AS " +
                "DECLARE @DbTablePrefix NVARCHAR(50),@DbMetaTables NVARCHAR(50);" +
                "exec getTableRealNameWithMeta @applicationName, @tableName, @tableRealName OUTPUT, @DbTablePrefix OUTPUT, @DbMetaTables OUTPUT;";
            base.BaseExecution(transaction);
        }
    }
}
