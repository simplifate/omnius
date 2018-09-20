using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public abstract class SqlQuery_withAppTabloid : SqlQuery_withApp
    {
        public DBTabloid tabloid;

        protected string realTabloidName;

        protected override void PreExecution()
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                throw new ArgumentNullException("application");
            if (tabloid == null)
                throw new ArgumentNullException("table or view");

            realTabloidName = RealTableName(application, tabloid.Name);
        }
    }
}
