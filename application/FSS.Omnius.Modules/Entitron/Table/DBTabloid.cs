using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBTabloid
    {
        public Application Application { get; set; }
        public virtual string Name { get; set; }
        private DBTabloidColumns _columns;
        public virtual DBTabloidColumns columns
        {
            get
            {
                if (_columns == null)
                    _columns = new DBTabloidColumns(this);

                return _columns;
            }
        }
        protected bool? _isInDB;
        public virtual bool isInDB
        {
            get
            {
                if (_isInDB == null)
                {
                    if (Application == null || Name == null)
                        return false;

                    SqlQuery_Tabloid_Exists query = new SqlQuery_Tabloid_Exists
                    {
                        application = Application,
                        tabloidName = Name
                    };
                    _isInDB = (query.ExecuteWithRead().Count == 1);
                }

                return _isInDB.Value;
            }
        }
    }
}
