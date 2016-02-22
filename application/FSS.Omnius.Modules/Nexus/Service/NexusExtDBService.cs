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

        /// <summary>
        /// Inicializuje novou SQL Query
        /// </summary>
        public NexusExtDBService NewQuery(string sql = "") { db.NewQuery(sql); return this; }

        /// <summary>
        /// Inicializuje novou subquery. SqlBuilder očekává jako parametr SqlBuilder, proto je nutné sestavení dotazu ukončit .sql
        /// </summary>
        public ExtDBSubquery NewSubquery() { return new ExtDBSubquery(); }

        /// <summary>
        /// Vrátí všechny řádky jako JToken [ {...}, {...}, {...} ]
        /// </summary>
        public JToken FetchAll() { return db.FetchAll(); }

        /// <summary>
        /// Vratí první řádek jako JToken {...}
        /// </summary>
        public JToken FetchOne() { return db.FetchOne(); }

        /// <summary>
        /// Vrací key => value páry ze všech řádků jako JToken { k1:v1, k2:v2, ... }
        /// </summary>     
        public JToken FetchHash(string keyColumn, string valueColumn) { return db.FetchHash(keyColumn, valueColumn); }

        /// <summary>
        /// Vrací všechny řádky jako asociativní JToken { k1:{r1}, k2:{r2}, ... }
        /// </summary>
        public JToken FetchAllAsHash(string keyColumn) { return db.FetchAllAsHash(keyColumn); }

        /// <summary>
        /// Vrací všechny řádky jako JToken { k1:[ {...}, {...}, ... ], k2:[ {...}, {...}, ... ], ... }
        /// </summary>
        public JToken FetchAllAsHashArray(string keyColumn) { return db.FetchAllAsHashArray(keyColumn); }

        /// <summary>
        /// Vrací hodnoty vybraného sloupce ze všech řádků jako List<Object>
        /// </summary>
        public List<Object> FetchArray(string column) { return db.FetchArray(column); }

        /// <summary>
        /// Vrací hodnotu vybraného sloupce v prvním řádku jako Object
        /// </summary>
        public Object FetchCell(string column) { return db.FetchCell(column); }

        #region SqlBuilderProxy

        public NexusExtDBService _(string body) { db._(body); return this; }
        public NexusExtDBService _(string body, params Object[] args) { db._(body, args); return this; }

        public NexusExtDBService From(string table) { db.From(table); return this; }
        public NexusExtDBService From(string table, params Object[] args) { db.From(table, args); return this; }
        public NexusExtDBService From(ExtDBSubquery query, string alias) { db.From(sql, alias); return this; }

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
        public NexusExtDBService With(ExtDBSubquery query, string alias) { db.With(sql, alias); return this; }

        #endregion
    }
}
