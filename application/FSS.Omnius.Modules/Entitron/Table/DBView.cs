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

        public int? viewId { get; set; }

        public DBView Create()
        {
            Application.queries.Add(new SqlQuery_ViewCreate()
            {
                application = Application,
                viewName = dbViewName,
                sql = sql
            });
            return this;
        }

        public static void Drop(Application application, string completeViewName)
        {
            SqlQuery_ViewDrop query = new SqlQuery_ViewDrop()
            {
                application = application,
                viewName = completeViewName
            };
            query.Execute();
        }

        public DBView Alter()
        {
            Application.queries.Add(new SqlQuery_ViewAlter()
            {
                application = Application,
                viewName = dbViewName,
                sql = sql
            });
            return this;
        }

        public static bool isInDb(Application app, string viewName)
        {
            if (app == null || viewName == null)
            {
                return false;
            }
            SqlQuery_ViewExists query = new SqlQuery_ViewExists() { application = app, viewName = viewName};
            foreach (DBItem i in query.ExecuteWithRead())
            {
                string name = Convert.ToString(i["name"]);
                if (name == "Entitron_" + app.Name + "_" + viewName)
                    return true;
            }
            return false;
        }

        public DBView()
        {

        }

        public DBView(int viewId)
        {
            this.viewId = viewId;
        }

        public SqlQuery_Select Select(params string[] columns)
        {
            return new SqlQuery_Select()
            {
                application = Application,
                columns = columns.ToList(),
                view = this
            };
        }
    }
}
