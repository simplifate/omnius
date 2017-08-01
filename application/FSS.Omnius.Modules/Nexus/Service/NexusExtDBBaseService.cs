using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Gate;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public abstract class NexusExtDBBaseService
    {
        public abstract NexusExtDBBaseService NewQuery(string sql = "");

        public abstract JToken FetchAll();
        public abstract JToken FetchOne();
        public abstract JToken FetchHash(string keyColumn, string valueColumn);
        public abstract JToken FetchAllAsHash(string keyColumn);
        public abstract JToken FetchAllAsHashArray(string keyColumn);
        public abstract List<Object> FetchArray(string column);
        public abstract Object FetchCell(string column);
        
        #region SqlBuilderProxy

        public abstract NexusExtDBBaseService _(string body);
        public abstract NexusExtDBBaseService _(string body, params Object[] args);
        public abstract NexusExtDBBaseService _(JArray body);

        public abstract NexusExtDBBaseService From(string table);
        public abstract NexusExtDBBaseService From(string table, params Object[] args);
        public abstract NexusExtDBBaseService From(ExtDBSubquery query, string alias);

        public abstract NexusExtDBBaseService GroupBy();
        public abstract NexusExtDBBaseService GroupBy(string body);
        public abstract NexusExtDBBaseService GroupBy(string body, params Object[] args);

        public abstract NexusExtDBBaseService Having();
        public abstract NexusExtDBBaseService Having(string body);
        public abstract NexusExtDBBaseService Having(string body, params Object[] args);

        public abstract NexusExtDBBaseService InnerJoin(string table);
        public abstract NexusExtDBBaseService InnerJoin(string table, params Object[] args);

        public abstract NexusExtDBBaseService Join();
        public abstract NexusExtDBBaseService Join(string table);
        public abstract NexusExtDBBaseService Join(string table, params Object[] args);

        public abstract NexusExtDBBaseService LeftJoin(string table);
        public abstract NexusExtDBBaseService LeftJoin(string table, params Object[] args);

        public abstract NexusExtDBBaseService Limit();
        public abstract NexusExtDBBaseService Limit(Int32 limit);
        public abstract NexusExtDBBaseService Limit(string limit);
        public abstract NexusExtDBBaseService Limit(string body, params Object[] args);

        public abstract NexusExtDBBaseService Offset();
        public abstract NexusExtDBBaseService Offset(Int32 offset);
        public abstract NexusExtDBBaseService Offset(string offset);
        public abstract NexusExtDBBaseService Offset(string body, params Object[] args);

        public abstract NexusExtDBBaseService OrderBy();
        public abstract NexusExtDBBaseService OrderBy(string body);
        public abstract NexusExtDBBaseService OrderBy(string body, params Object[] args);

        public abstract NexusExtDBBaseService RightJoin(string table);
        public abstract NexusExtDBBaseService RightJoin(string table, params Object[] args);

        public abstract NexusExtDBBaseService Select();
        public abstract NexusExtDBBaseService Select(string columns);
        public abstract NexusExtDBBaseService Select(string columns, params Object[] args);

        public abstract NexusExtDBBaseService Union();

        public abstract NexusExtDBBaseService Where();
        public abstract NexusExtDBBaseService Where(string condition);
        public abstract NexusExtDBBaseService Where(string format, params Object[] args);
        public abstract NexusExtDBBaseService Where(JArray conditions);

        public abstract NexusExtDBBaseService With(string body);
        public abstract NexusExtDBBaseService With(string format, params Object[] args);
        public abstract NexusExtDBBaseService With(ExtDBSubquery query, string alias);

        #endregion   
    }
}
