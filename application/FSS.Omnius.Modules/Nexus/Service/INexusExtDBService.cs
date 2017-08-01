using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Gate;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface INexusExtDBService<T> where T: NexusExtDBBaseService
    {
        T NewQuery(string sql = "");

        JToken FetchAll();
        JToken FetchOne();
        JToken FetchHash(string keyColumn, string valueColumn);
        JToken FetchAllAsHash(string keyColumn);
        JToken FetchAllAsHashArray(string keyColumn);
        List<Object> FetchArray(string column);
        Object FetchCell(string column);
        
        #region SqlBuilderProxy

        T _(string body);
        T _(string body, params Object[] args);
        T _(JArray body);

        T From(string table);
        T From(string table, params Object[] args);
        T From(ExtDBSubquery query, string alias);

        T GroupBy();
        T GroupBy(string body);
        T GroupBy(string body, params Object[] args);

        T Having();
        T Having(string body);
        T Having(string body, params Object[] args);

        T InnerJoin(string table);
        T InnerJoin(string table, params Object[] args);

        T Join();
        T Join(string table);
        T Join(string table, params Object[] args);

        T LeftJoin(string table);
        T LeftJoin(string table, params Object[] args);

        T Limit();
        T Limit(Int32 limit);
        T Limit(string limit);
        T Limit(string body, params Object[] args);

        T Offset();
        T Offset(Int32 offset);
        T Offset(string offset);
        T Offset(string body, params Object[] args);

        T OrderBy();
        T OrderBy(string body);
        T OrderBy(string body, params Object[] args);

        T RightJoin(string table);
        T RightJoin(string table, params Object[] args);

        T Select();
        T Select(string columns);
        T Select(string columns, params Object[] args);

        T Union();

        T Where();
        T Where(string condition);
        T Where(string format, params Object[] args);

        T With(string body);
        T With(string format, params Object[] args);
        T With(ExtDBSubquery query, string alias);

        #endregion   
    }
}
