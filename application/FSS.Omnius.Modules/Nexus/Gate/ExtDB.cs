using System;
using System.Linq;
using DbExtensions;
using FSS.Omnius.Modules.Entitron.Entity;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Nexus.Gate
{
    class ExtDB
    {
        private Database db;
        public SqlBuilder sql;

        public ExtDB(string serverName, string dbName)
        {
            DBEntities e = new DBEntities();

            Entitron.Entity.Nexus.ExtDB row = e.ExtDBs.Single(m => m.DB_Server == serverName && m.DB_Name == dbName);
            if (row == null) {
                throw new Exception(string.Format("Konfigurace pro server {0} a databázi {1} nebyla nalezena", serverName, dbName));
            }

            DbConnection conn = GetConnection(row);
            db = new Database(conn);
        }

        public ExtDB NewQuery()
        {
            sql = new SqlBuilder();
            return this;
        }

        public JToken FetchAll()
        {
            SqlSet set = new SqlSet(sql, db.Connection);
            return JToken.FromObject(set.ToList());
        }

        public JToken FetchOne()
        {
            return FetchAll().First();
        }

        public Object FetchCell(string column)
        {
            return FetchOne()[column];
        }

        public List<Object> FetchArray(string column)
        {
            List<Object> list = new List<Object>();

            JToken rows = FetchAll();
            foreach(JToken row in rows) {
                list.Add(row[column]);
            }

            return list;
        }



        #region SqlBuilderProxy

        public ExtDB _(string body) { sql._(body); return this; }
        public ExtDB _(string body, params Object[] args) { sql._(body, args); return this; }

        public ExtDB From(string table) { sql.FROM(table); return this; }
        public ExtDB From(string table, params Object[] args) { sql.FROM(table, args); return this; }
        //public ExtDB From(SqlBuilder sql, string alias) { sql.FROM(sql, alias); return this; }

        public ExtDB GroupBy() { sql.GROUP_BY(); return this; }
        public ExtDB GroupBy(string body) { sql.GROUP_BY(body); return this; }
        public ExtDB GroupBy(string body, params Object[] args) { sql.GROUP_BY(body, args); return this; }

        public ExtDB Having() { sql.HAVING(); return this; }
        public ExtDB Having(string body) { sql.HAVING(body); return this; }
        public ExtDB Having(string body, params Object[] args) { sql.HAVING(body, args); return this; }

        public ExtDB InnerJoin(string table) { sql.INNER_JOIN(table); return this; }
        public ExtDB InnerJoin(string table, params Object[] args) { sql.INNER_JOIN(table, args); return this; }

        public ExtDB Join() { sql.JOIN(); return this; }
        public ExtDB Join(string table) { sql.JOIN(table); return this; }
        public ExtDB Join(string table, params Object[] args) { sql.JOIN(table, args); return this; }

        public ExtDB LeftJoin(string table) { sql.LEFT_JOIN(table); return this; }
        public ExtDB LeftJoin(string table, params Object[] args) { sql.LEFT_JOIN(table, args); return this; }

        public ExtDB Limit() { sql.LIMIT(); return this; }
        public ExtDB Limit(Int32 limit) { sql.LIMIT(limit); return this; }
        public ExtDB Limit(string limit) { sql.LIMIT(limit); return this; }
        public ExtDB Limit(string body, params Object[] args) { sql.LIMIT(body, args); return this; }

        public ExtDB Offset() { sql.OFFSET(); return this; }
        public ExtDB Offset(Int32 offset) { sql.OFFSET(offset); return this; }
        public ExtDB Offset(string offset) { sql.OFFSET(offset); return this; }
        public ExtDB Offset(string body, params Object[] args) { sql.OFFSET(body, args); return this; }

        public ExtDB OrderBy() { sql.ORDER_BY(); return this; }
        public ExtDB OrderBy(string body) { sql.ORDER_BY(body); return this; }
        public ExtDB OrderBy(string body, params Object[] args) { sql.ORDER_BY(body, args); return this; }

        public ExtDB RightJoin(string table) { sql.RIGHT_JOIN(table); return this; }
        public ExtDB RightJoin(string table, params Object[] args) { sql.RIGHT_JOIN(table, args); return this; }

        public ExtDB Select() { sql.SELECT(); return this; }
        public ExtDB Select(string columns) { sql.SELECT(columns); return this; }
        public ExtDB Select(string columns, params Object[] args) { sql.SELECT(columns, args); return this; }

        public ExtDB Union() { sql.UNION(); return this; }

        public ExtDB Where() { sql.WHERE(); return this; }
        public ExtDB Where(string condition) { sql.WHERE(condition); return this; }
        public ExtDB Where(string format, params Object[] args) { sql.WHERE(format, args); return this; }

        public ExtDB With(string body) { sql.WITH(body); return this; }
        public ExtDB With(string format, params Object[] args) { sql.WITH(format, args); return this; }
        //public ExtDB With(SqlBuilder sql, string alias) { sql.WITH(sql, alias); return this; }

        #endregion

        #region Tools

        private DbConnection GetConnection(Entitron.Entity.Nexus.ExtDB row)
        {
            switch(row.DB_Type)
            {
                case Entitron.Entity.Nexus.ExtDBType.MySQL:
                    {
                        string cs = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=Preferred;",
                                row.DB_Server, row.DB_Port, row.DB_Name, row.DB_User, row.DB_Password
                            );
                        DbConnection conn = new MySql.Data.MySqlClient.MySqlConnection(cs);
                        return conn;
                    }
                default: {
                        throw new Exception("Formát connection stringu nebyl nalezen");
                    }
            }
        }

        #endregion
    }
}
