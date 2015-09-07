using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Column_Drop : SqlQuery_withApp
    {
        public string tableName { get; set; }
        public string columnName { get; set; }

        public SqlQuery_Column_Drop(string ApplicationName) : base(ApplicationName)
        {
            // ##!!##
        }
    }
}
