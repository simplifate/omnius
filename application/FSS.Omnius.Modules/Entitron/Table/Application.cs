using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Table;

namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    public partial class Application
    {
        public static IEnumerable<Application> getAllowed(Modules.CORE.CORE core, string userName)
        {
            return
                core.Entitron.GetStaticTables().Applications.Where(a =>
                    a.IsPublished
                    && a.IsEnabled
                    && a.ADgroups.FirstOrDefault().ADgroup_Users.Any(adu => adu.User.UserName == userName));
        }

        internal SqlQueue queries = new SqlQueue();

        public IEnumerable<DBTable> GetTables()
        {
            List<DBItem> items = (new SqlQuery_Select_TableList() { ApplicationName = Name }).ExecuteWithRead();

            return items.Select(i =>
                new DBTable((int)i["tableId"])
                {
                    tableName = (string)i["Name"],
                    Application = this
                }).ToList();
        }
        public DBTable GetTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException("tableName");

            SqlQuery_Table_exists query = new SqlQuery_Table_exists()
            {
                applicationName = Name,
                tableName = tableName
            };

            // if table exists
            List<DBItem> tables = query.ExecuteWithRead();
            if (tables.Count > 0)
            {
                DBItem table = tables.First();
                return new DBTable((int)table["tableId"]) { Application = this, tableName = (string)table["Name"] };
            }

            return null;
        }

        public List<string> GetViewNames()
        {
            SqlQuery_SelectViews query = new SqlQuery_SelectViews();
            List<string> viewNames=new List<string>();
            foreach (DBItem i in query.ExecuteWithRead())
            {
                string name = Convert.ToString(i["name"]);
                viewNames.Add(name);
            }
            return viewNames;
        } 

        public DBView GetView(string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
                throw new ArgumentNullException("viewName");

            SqlQuery_ViewExists query = new SqlQuery_ViewExists()
            {
                appName = Name,
                viewName = viewName
            };

            // if table exists
            List<DBItem> views = query.ExecuteWithRead();
            if (views.Count > 0) {
                DBItem view = views.First();
                return new DBView() { Application = this, dbViewName = (string)view["name"] };
            }

            return null;
        }

        public void SaveChanges()
        {
            queries.ExecuteAll();
        }
    }
}
