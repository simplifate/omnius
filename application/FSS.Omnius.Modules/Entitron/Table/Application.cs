using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Table;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    public partial class Application
    {
        public static IQueryable<Application> getAllowed(Modules.CORE.CORE core, string userName)
        {
            return
                core.Entitron.GetStaticTables().Applications.Where(a =>
                    a.IsPublished
                    && a.IsEnabled
                    && !a.IsSystem
                    && a.ADgroups.FirstOrDefault().ADgroup_Users.Any(adu => adu.User.UserName == userName));
        }
        // return app from application DB
        [ImportExportIgnore]
        private Application _similarApp;
        [ImportExportIgnore]
        public Application similarApp
        {
            get
            {
                if (connectionString_schema == null)
                    return this;

                if (_similarApp == null)
                    _similarApp = DBEntities.appInstance(this).Applications.SingleOrDefault(a => a.Name == Name);
                return _similarApp;
            }
        }

        public string GetLayout()
        {
            return this.CssTemplate.Url;
        }

        public static Application GetByName(Modules.CORE.CORE core, string appName)
        {
            return core.Entitron.GetStaticTables().Applications.FirstOrDefault(app => app.Name == appName);
        }

        internal SqlQueue queries = new SqlQueue();

        public IEnumerable<DBTable> GetTables()
        {
            List<DBItem> items = (new SqlQuery_Select_TableList() { application = similarApp }).ExecuteWithRead();

            return items.Select(i =>
                new DBTable((int)i["tableId"])
                {
                    tableName = (string)i["Name"],
                    Application = this
                }).ToList();
        }
        public DBTable GetTable(string tableName, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException("tableName");

            SqlQuery_Table_exists query = new SqlQuery_Table_exists()
            {
                application = similarApp,
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
            SqlQuery_SelectViews query = new SqlQuery_SelectViews { application = this };
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
                application = this,
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
            queries.ExecuteAll(connectionString_data);
        }
    }
}
