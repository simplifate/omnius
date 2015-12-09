using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Nexus.Gate;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public class NexusExtDBService : INexusExtDBService
    {
        ExtDB db;

        public string sql {
            get { return db.sql.ToString(); }
        }

        public NexusExtDBService(string serverName, string dbName)
        {
            db = new ExtDB(serverName, dbName);
        }

        public NexusExtDBService NewQuery() { db.NewQuery(); return this; }

        public JToken FetchAll() { return db.FetchAll(); }
        public JToken FetchOne() { return db.FetchOne(); }
        public Object FetchCell(string column) { return db.FetchCell(column); }
        public List<Object> FetchArray(string column) { return db.FetchArray(column); }

        #region SqlBuilderProxy

        public NexusExtDBService _(string body) { db._(body); return this; }
        public NexusExtDBService _(string body, params Object[] args) { db._(body, args); return this; }

        public NexusExtDBService From(string table) { db.From(table); return this; }
        public NexusExtDBService From(string table, params Object[] args) { db.From(table, args); return this; }
        //public NexusExtDBService From(SqlBuilder sql, string alias) { db.From(sql, alias); return this; }

        public NexusExtDBService GroupBy() { db.GroupBy(); return this; }
        public NexusExtDBService GroupBy(string body) { db.GroupBy(body); return this; }
        public NexusExtDBService GroupBy(string body, params Object[] args) { db.GroupBy(body, args); return this; }

        public NexusExtDBService Having() { db.Having(); return this; }
        public NexusExtDBService Having(string body) { db.Having(body); return this; }
        public NexusExtDBService Having(string body, params Object[] args) { db.Having(body, args); return this; }

        public NexusExtDBService InnerJoin(string table) { db.InnerJoin(table); return this; }
        public NexusExtDBService InnerJoin(string table, params Object[] args) { db.InnerJoin(table, args); return this; }

        public NexusExtDBService Join() { db.Join(); return this; }
        public NexusExtDBService Join(string table) { db.Join(table); return this; }
        public NexusExtDBService Join(string table, params Object[] args) { db.Join(table, args); return this; }

        public NexusExtDBService LeftJoin(string table) { db.LeftJoin(table); return this; }
        public NexusExtDBService LeftJoin(string table, params Object[] args) { db.LeftJoin(table, args); return this; }

        public NexusExtDBService Limit() { db.Limit(); return this; }
        public NexusExtDBService Limit(Int32 limit) { db.Limit(limit); return this; }
        public NexusExtDBService Limit(string limit) { db.Limit(limit); return this; }
        public NexusExtDBService Limit(string body, params Object[] args) { db.Limit(body, args); return this; }

        public NexusExtDBService Offset() { db.Offset(); return this; }
        public NexusExtDBService Offset(Int32 offset) { db.Offset(offset); return this; }
        public NexusExtDBService Offset(string offset) { db.Offset(offset); return this; }
        public NexusExtDBService Offset(string body, params Object[] args) { db.Offset(body, args); return this; }

        public NexusExtDBService OrderBy() { db.OrderBy(); return this; }
        public NexusExtDBService OrderBy(string body) { db.OrderBy(body); return this; }
        public NexusExtDBService OrderBy(string body, params Object[] args) { db.OrderBy(body, args); return this; }

        public NexusExtDBService RightJoin(string table) { db.RightJoin(table); return this; }
        public NexusExtDBService RightJoin(string table, params Object[] args) { db.RightJoin(table, args); return this; }

        public NexusExtDBService Select() { db.Select(); return this; }
        public NexusExtDBService Select(string columns) { db.Select(columns); return this; }
        public NexusExtDBService Select(string columns, params Object[] args) { db.Select(columns, args); return this; }

        public NexusExtDBService Union() { db.Union(); return this; }

        public NexusExtDBService Where() { db.Where(); return this; }
        public NexusExtDBService Where(string condition) { db.Where(condition); return this; }
        public NexusExtDBService Where(string format, params Object[] args) { db.Where(format, args); return this; }

        public NexusExtDBService With(string body) { db.With(body); return this; }
        public NexusExtDBService With(string format, params Object[] args) { db.With(format, args); return this; }
        //public NexusExtDBService With(SqlBuilder sql, string alias) { db.With(sql, alias); return this; }

        #endregion



    }
}
