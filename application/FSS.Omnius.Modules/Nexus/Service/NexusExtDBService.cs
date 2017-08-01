using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Nexus.Gate;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public class NexusExtDBService : NexusExtDBBaseService
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
        public override NexusExtDBBaseService NewQuery(string sql = "") { db.NewQuery(sql); return this; }

        /// <summary>
        /// Inicializuje novou subquery. SqlBuilder očekává jako parametr SqlBuilder, proto je nutné sestavení dotazu ukončit .sql
        /// </summary>
        public ExtDBSubquery NewSubquery() { return new ExtDBSubquery(); }

        /// <summary>
        /// Vrátí všechny řádky jako JToken [ {...}, {...}, {...} ]
        /// </summary>
        public override JToken FetchAll() { return db.FetchAll(); }

        /// <summary>
        /// Vratí první řádek jako JToken {...}
        /// </summary>
        public override JToken FetchOne() { return db.FetchOne(); }

        /// <summary>
        /// Vrací key => value páry ze všech řádků jako JToken { k1:v1, k2:v2, ... }
        /// </summary>     
        public override JToken FetchHash(string keyColumn, string valueColumn) { return db.FetchHash(keyColumn, valueColumn); }

        /// <summary>
        /// Vrací všechny řádky jako asociativní JToken { k1:{r1}, k2:{r2}, ... }
        /// </summary>
        public override JToken FetchAllAsHash(string keyColumn) { return db.FetchAllAsHash(keyColumn); }

        /// <summary>
        /// Vrací všechny řádky jako JToken { k1:[ {...}, {...}, ... ], k2:[ {...}, {...}, ... ], ... }
        /// </summary>
        public override JToken FetchAllAsHashArray(string keyColumn) { return db.FetchAllAsHashArray(keyColumn); }

        /// <summary>
        /// Vrací hodnoty vybraného sloupce ze všech řádků jako List<Object>
        /// </summary>
        public override List<Object> FetchArray(string column) { return db.FetchArray(column); }

        /// <summary>
        /// Vrací hodnotu vybraného sloupce v prvním řádku jako Object
        /// </summary>
        public override Object FetchCell(string column) { return db.FetchCell(column); }

        #region SqlBuilderProxy

        public override NexusExtDBBaseService _(string body) { db._(body); return this; }
        public override NexusExtDBBaseService _(string body, params Object[] args) { db._(body, args); return this; }
        public override NexusExtDBBaseService _(JArray body) { return Where(body); }

        public override NexusExtDBBaseService From(string table) { db.From(table); return this; }
        public override NexusExtDBBaseService From(string table, params Object[] args) { db.From(table, args); return this; }
        public override NexusExtDBBaseService From(ExtDBSubquery query, string alias) { db.From(sql, alias); return this; }

        public override NexusExtDBBaseService GroupBy() { db.GroupBy(); return this; }
        public override NexusExtDBBaseService GroupBy(string body) { db.GroupBy(body); return this; }
        public override NexusExtDBBaseService GroupBy(string body, params Object[] args) { db.GroupBy(body, args); return this; }

        public override NexusExtDBBaseService Having() { db.Having(); return this; }
        public override NexusExtDBBaseService Having(string body) { db.Having(body); return this; }
        public override NexusExtDBBaseService Having(string body, params Object[] args) { db.Having(body, args); return this; }

        public override NexusExtDBBaseService InnerJoin(string table) { db.InnerJoin(table); return this; }
        public override NexusExtDBBaseService InnerJoin(string table, params Object[] args) { db.InnerJoin(table, args); return this; }

        public override NexusExtDBBaseService Join() { db.Join(); return this; }
        public override NexusExtDBBaseService Join(string table) { db.Join(table); return this; }
        public override NexusExtDBBaseService Join(string table, params Object[] args) { db.Join(table, args); return this; }

        public override NexusExtDBBaseService LeftJoin(string table) { db.LeftJoin(table); return this; }
        public override NexusExtDBBaseService LeftJoin(string table, params Object[] args) { db.LeftJoin(table, args); return this; }

        public override NexusExtDBBaseService Limit() { db.Limit(); return this; }
        public override NexusExtDBBaseService Limit(Int32 limit) { db.Limit(limit); return this; }
        public override NexusExtDBBaseService Limit(string limit) { db.Limit(limit); return this; }
        public override NexusExtDBBaseService Limit(string body, params Object[] args) { db.Limit(body, args); return this; }

        public override NexusExtDBBaseService Offset() { db.Offset(); return this; }
        public override NexusExtDBBaseService Offset(Int32 offset) { db.Offset(offset); return this; }
        public override NexusExtDBBaseService Offset(string offset) { db.Offset(offset); return this; }
        public override NexusExtDBBaseService Offset(string body, params Object[] args) { db.Offset(body, args); return this; }

        public override NexusExtDBBaseService OrderBy() { db.OrderBy(); return this; }
        public override NexusExtDBBaseService OrderBy(string body) { db.OrderBy(body); return this; }
        public override NexusExtDBBaseService OrderBy(string body, params Object[] args) { db.OrderBy(body, args); return this; }

        public override NexusExtDBBaseService RightJoin(string table) { db.RightJoin(table); return this; }
        public override NexusExtDBBaseService RightJoin(string table, params Object[] args) { db.RightJoin(table, args); return this; }

        public override NexusExtDBBaseService Select() { db.Select(); return this; }
        public override NexusExtDBBaseService Select(string columns) { db.Select(columns); return this; }
        public override NexusExtDBBaseService Select(string columns, params Object[] args) { db.Select(columns, args); return this; }

        public override NexusExtDBBaseService Union() { db.Union(); return this; }

        public override NexusExtDBBaseService Where() { db.Where(); return this; }
        public override NexusExtDBBaseService Where(string condition) { db.Where(condition); return this; }
        public override NexusExtDBBaseService Where(string format, params Object[] args) { db.Where(format, args); return this; }

        public override NexusExtDBBaseService Where(JArray conditions)
        {
            List<string> conds = new List<string>();
            foreach (JToken cond in conditions) {
                string column = (string)cond["column"];
                string op = (string)cond["operator"];
                object value = (object)cond["value"];

                switch (op) {
                    case "eq": conds.Add(string.Format("{0} = {1}", column, QuoteValue(value))); break;
                    case "lt": conds.Add(string.Format("{0} < {1}", column, QuoteValue(value))); break;
                    case "gt": conds.Add(string.Format("{0} > {1}", column, QuoteValue(value))); break;
                    case "lte": conds.Add(string.Format("{0} <= {1}", column, QuoteValue(value))); break;
                    case "gte": conds.Add(string.Format("{0} >= {1}", column, QuoteValue(value))); break;
                    case "like": conds.Add(string.Format("{0} LIKE {1}", column, QuoteValue(value, "%", "%"))); break;
                    case "not-like": conds.Add(string.Format("{0} NOT LIKE {1}", column, QuoteValue(value, "%", "%"))); break;
                    case "in": {
                            List<string> vals = new List<string>();
                            string[] values = ((string)value).Split(new char[] { ',', ';' });
                            foreach (string val in values) {
                                vals.Add(QuoteValue(val));
                            }
                            conds.Add(string.Format("{0} IN ({1})", column, string.Join(", ", vals)));
                            break;
                        }
                    case "not-in": {
                            List<string> vals = new List<string>();
                            string[] values = ((string)value).Split(new char[] { ',', ';' });
                            foreach (string val in values) {
                                vals.Add(QuoteValue(val));
                            }
                            conds.Add(string.Format("{0} NOT IN ({1})", column, string.Join(", ", vals)));
                            break;
                        }
                }
            }

            return Where(string.Join(" AND ", conds));
        }

        public override NexusExtDBBaseService With(string body) { db.With(body); return this; }
        public override NexusExtDBBaseService With(string format, params Object[] args) { db.With(format, args); return this; }
        public override NexusExtDBBaseService With(ExtDBSubquery query, string alias) { db.With(sql, alias); return this; }

        #endregion

        private string QuoteValue(object value, string prefix = "", string suffix = "")
        {
            return value is string ? $"'{prefix}{value}{suffix}'" : (value is int ? value.ToString() : $"'{prefix}{value.ToString()}{suffix}'");
        }
    }
}
