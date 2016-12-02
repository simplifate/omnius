using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Rename : SqlQuery_withAppTable
    {
        public string originColumnName { get; set; }
        public string newColumnName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string parNewName = safeAddParam("newColumnName", newColumnName);
            string parFullOriginColumnName = safeAddParam("fullOriginColumnName", $"Entitron_{application.Name}_{table.tableName}.{originColumnName}");

            sqlString =
                $"exec sp_RENAME @{parFullOriginColumnName}, @{parNewName}, 'COLUMN';";

            base.BaseExecution(connection);
        }

        public override string ToString()
        {
            return string.Format("Rename column {0} in {1}[{2}]", originColumnName, table.tableName, application.Name);
        }
    }
}
