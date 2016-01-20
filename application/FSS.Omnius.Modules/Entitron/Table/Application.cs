using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    public partial class Application
    {
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

        public void SaveChanges()
        {
            queries.ExecuteAll();
        }
    }
}
