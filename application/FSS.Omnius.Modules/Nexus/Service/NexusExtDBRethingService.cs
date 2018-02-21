using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Gate;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public class NexusExtDBRethingService : NexusExtDBBaseService
    {
        private RethinkDB r = RethinkDB.R;
        private Connection c;
        private string dbName;

        private ReqlExpr query;
        private string mode;

        public NexusExtDBRethingService(int extDBId)
        {
            var context = DBEntities.instance;
            Entitron.Entity.Nexus.ExtDB info = context.ExtDBs.Where(d => d.Id == extDBId).SingleOrDefault();

            if (info == null) {
                throw new Exception("Nexus RethinkDB service: Cannot connect to database. Integration is not defined.");
            }

            Connect(info);
        }

        public NexusExtDBRethingService(Entitron.Entity.Nexus.ExtDB info)
        {
            Connect(info);
        }

        private void Connect(Entitron.Entity.Nexus.ExtDB info)
        {
            dbName = info.DB_Name;

            c = r.Connection()
                    .Hostname(info.DB_Server)
                    .Port(Convert.ToInt32(info.DB_Port))
                    .Timeout(60)
                    .Connect();
        } 

        public override NexusExtDBBaseService NewQuery(string sql = "")
        {
            throw new NotImplementedException();
        }

        public NexusExtDBRethingService NewSubquery()
        {
            throw new NotImplementedException();
        }

        public override JToken FetchAll()
        {
            Cursor<JObject> all = query.RunCursor<JObject>(c);
            JArray rows = new JArray();

            foreach(JObject row in all) {
                rows.Add(row);
            }

            return rows;
        }

        public override JToken FetchOne()
        {
            return query.RunResult<JArray>(c)[0];
        }

        public override JToken FetchHash(string keyColumn, string valueColumn)
        {
            throw new NotImplementedException();
        }

        public override JToken FetchAllAsHash(string keyColumn)
        {
            throw new NotImplementedException();
        }

        public override JToken FetchAllAsHashArray(string keyColumn)
        {
            throw new NotImplementedException();
        }

        public override List<object> FetchArray(string column)
        {
            throw new NotImplementedException();
        }

        public override object FetchCell(string column)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService _(string body)
        {
            switch(mode) {
                case "limit": return Limit(body);
                case "offset": return Offset(body);
                case "orderby": return OrderBy(body);
                case "select": return Select(body);
                case "where": throw new NotImplementedException();
            }
            return this;
        }

        public override NexusExtDBBaseService _(JArray body)
        {
            switch(mode) {
                case "where": return Where(body);
            }
            return this;
        }

        public override NexusExtDBBaseService _(string body, params object[] args)
        {
            return _(string.Format(body, args));
        }

        public override NexusExtDBBaseService From(string table)
        {
            query = r.Db(dbName).Table(table);
            return this;
        }

        public override NexusExtDBBaseService From(string table, params object[] args)
        {
            return From(string.Format(table, args));
        }

        public override NexusExtDBBaseService From(ExtDBSubquery query, string alias)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService GroupBy()
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService GroupBy(string body)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService GroupBy(string body, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Having()
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Having(string body)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Having(string body, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService InnerJoin(string table)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService InnerJoin(string table, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Join()
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Join(string table)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Join(string table, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService LeftJoin(string table)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService LeftJoin(string table, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Limit()
        {
            mode = "limit";
            return this;
        }

        public override NexusExtDBBaseService Limit(int limit)
        {
            Limit();
            query = query.Limit(limit);
            return this;
        }

        public override NexusExtDBBaseService Limit(string limit)
        {
            return Limit(Convert.ToInt32(limit));
        }

        public override NexusExtDBBaseService Limit(string body, params object[] args)
        {
            return Limit(string.Format(body, args));
        }

        public override NexusExtDBBaseService Offset()
        {
            mode = "offset";
            return this;
        }

        public override NexusExtDBBaseService Offset(int offset)
        {
            Offset();
            query = query.Skip(offset);
            return this;
        }

        public override NexusExtDBBaseService Offset(string offset)
        {
            return Offset(Convert.ToInt32(offset));
        }

        public override NexusExtDBBaseService Offset(string body, params object[] args)
        {
            return Offset(string.Format(body, args));
        }

        public override NexusExtDBBaseService OrderBy()
        {
            mode = "orderby";
            return this;
        }

        public override NexusExtDBBaseService OrderBy(string body)
        {
            OrderBy();

            string[] order = body.Split(' ');
            bool useIndex = order[0].StartsWith("index:");
            string column = useIndex ? order[0].Substring(6) : order[0];
            string dir = order.Count() > 1 ? order[1].ToLower() : "asc";

            switch(dir) {
                case "asc": {
                        query = useIndex ? query.OrderBy().OptArg("index", r.Asc(column)) : query.OrderBy(r.Asc(column));
                        break;
                    }
                case "desc": {
                        query = useIndex ? query.OrderBy().OptArg("index", r.Desc(column)) :  query.OrderBy(r.Desc(column));
                        break;
                    }
            }
            
            return this;
        }

        public override NexusExtDBBaseService OrderBy(string body, params object[] args)
        {
            return OrderBy(string.Format(body, args));
        }

        public override NexusExtDBBaseService RightJoin(string table)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService RightJoin(string table, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Select()
        {
            mode = "select";
            return this;
        }

        public override NexusExtDBBaseService Select(string columns)
        {
            Select();
            List<string> cols = new List<string>();
            string[] tmp = columns.Split(new char[] { ',', ';' });
            foreach(string col in tmp) {
                cols.Add(col.Trim(' '));
            }
            
            query = query.WithFields(cols);
            return this;
        }

        public override NexusExtDBBaseService Select(string columns, params object[] args)
        {
            return Select(string.Format(columns, args));
        }

        public override NexusExtDBBaseService Union()
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Where()
        {
            mode = "where";
            return this;
        }

        #warning Not implemented. Format is too different, so we need conditions as JToken
        public override NexusExtDBBaseService Where(string condition)
        {
            throw new NotImplementedException();
        }

        #warning Not implemented. Format is too different, so we need conditions as JToken
        public override NexusExtDBBaseService Where(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService Where(JArray conditions)
        {
            Where();
            query = query.Filter(doc => BuildFilter(doc, conditions));

            return this;
        }

        private ReqlExpr BuildFilter(ReqlExpr doc, JArray conditions)
        {
            var statement = r.And();
            foreach (JToken cond in conditions) {
                string column = (string)cond["column"];
                string op = (string)cond["operator"];
                object value = (object)cond["value"];

                switch (op) {
                    case "eq": statement = statement.And(doc[column].Eq(value)); break;
                    case "lt": statement = statement.And(doc[column].Lt(value)); break;
                    case "gt": statement = statement.And(doc[column].Gt(value)); break;
                    case "lte": statement = statement.And(doc[column].Le(value)); break;
                    case "gte": statement = statement.And(doc[column].Ge(value)); break;
                    case "like": statement = statement.And(doc[column].Match(value)); break;
                    case "not-like": statement = statement.And(doc[column].Match(value).Not()); break;
                    case "in": {
                            var st = r.Or();
                            string[] values = ((string)value).Split(new char[] { ',', ';' });
                            foreach(string val in values) {
                                st = st.Or(doc[column].Eq(val));
                            }
                            statement = statement.And(st);
                            break;
                        }
                    case "not-in": {
                            var st = r.And();
                            string[] values = ((string)value).Split(new char[] { ',', ';' });
                            foreach (string val in values) {
                                st = st.And(doc[column].Eq(val).Not());
                            }
                            statement = statement.And(st);
                            break;
                        }
                }

            }

            return statement;
        }

        public override NexusExtDBBaseService With(string body)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService With(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override NexusExtDBBaseService With(ExtDBSubquery query, string alias)
        {
            throw new NotImplementedException();
        }
        
        public override NexusExtDBResult Insert(string table, JToken row)
        {
            var result = r.Db(dbName).Table(table).Insert(row).RunResult(c);
            return new NexusExtDBResult(result);
        }

        public override NexusExtDBResult Update(string table, JToken row, object id)
        {
            var result = r.Db(dbName).Table(table).Get(id).Update(row).RunResult(c);
            return new NexusExtDBResult(result);
        }

        public override NexusExtDBResult Delete(string table, object id)
        {
            var result = r.Db(dbName).Table(table).Get(id).Delete().RunResult(c);
            return new NexusExtDBResult(result);
        }
    }
}
