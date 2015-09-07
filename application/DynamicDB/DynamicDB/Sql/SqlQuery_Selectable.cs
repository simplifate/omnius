using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Selectable : SqlQuery_withApp
    {
        private string _join = "";
        private string _where = "";
        private string _order = "";
        private string _group = "";
        
        public SqlQuery_Selectable(string ApplicationName) : base(ApplicationName)
        {
        }

        public SqlQuery_Selectable where(string condition)
        {
            _where += condition;

            return this;
        }
    }
}
