using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Sql;

namespace FSS.Omnius.Modules.Entitron.Table
{
    public class DBView
    {
        public Application Application { get; set; }
        public string dbViewName { get; set; }
        public string sql { get; set; }

        public DBView Create()
        {
            Application.queries.Add(new SqlQuery_ViewCreate()
            {
                Application = Application,
                viewName = dbViewName,
                sql = sql
            });
            return this;
        }

        public static void Drop(string appName, string viewName)
        {
            SqlQuery_ViewDrop query = new SqlQuery_ViewDrop()
            {
                appName = appName,
                viewName = viewName
            };
            query.Execute();
        }

        public DBView Alter()
        {
            Application.queries.Add(new SqlQuery_ViewAlter()
            {
                Application = Application,
                viewName = dbViewName,
                sql = sql
            });
            return this;
        }

        public static bool isInDb(string appName, string viewName)
        {
            if (appName == null || viewName == null)
            {
                return false;
            }
            SqlQuery_ViewExists query = new SqlQuery_ViewExists() {appName = appName,viewName = viewName};
            foreach (DBItem i in query.ExecuteWithRead())
            {
                string name = Convert.ToString(i["name"]);
                if (name == "Entitron_" + appName + "_" + viewName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
