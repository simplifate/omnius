using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface INexusExtDBService
    {
        NexusExtDBService NewQuery();

        JToken FetchAll();
        JToken FetchOne();
        object FetchCell(string column);
        List<object> FetchArray(string column);

        #region SqlBuilderProxy

        NexusExtDBService _(string body);
        NexusExtDBService _(string body, params Object[] args);

        NexusExtDBService From(string table);
        NexusExtDBService From(string table, params Object[] args);
        //NexusExtDBService From(SqlBuilder sql, string alias);

        NexusExtDBService GroupBy();
        NexusExtDBService GroupBy(string body);
        NexusExtDBService GroupBy(string body, params Object[] args);

        NexusExtDBService Having();
        NexusExtDBService Having(string body);
        NexusExtDBService Having(string body, params Object[] args);

        NexusExtDBService InnerJoin(string table);
        NexusExtDBService InnerJoin(string table, params Object[] args);

        NexusExtDBService Join();
        NexusExtDBService Join(string table);
        NexusExtDBService Join(string table, params Object[] args);

        NexusExtDBService LeftJoin(string table);
        NexusExtDBService LeftJoin(string table, params Object[] args);

        NexusExtDBService Limit();
        NexusExtDBService Limit(Int32 limit);
        NexusExtDBService Limit(string limit);
        NexusExtDBService Limit(string body, params Object[] args);

        NexusExtDBService Offset();
        NexusExtDBService Offset(Int32 offset);
        NexusExtDBService Offset(string offset);
        NexusExtDBService Offset(string body, params Object[] args);

        NexusExtDBService OrderBy();
        NexusExtDBService OrderBy(string body);
        NexusExtDBService OrderBy(string body, params Object[] args);

        NexusExtDBService RightJoin(string table);
        NexusExtDBService RightJoin(string table, params Object[] args);

        NexusExtDBService Select();
        NexusExtDBService Select(string columns);
        NexusExtDBService Select(string columns, params Object[] args);

        NexusExtDBService Union();

        NexusExtDBService Where();
        NexusExtDBService Where(string condition);
        NexusExtDBService Where(string format, params Object[] args);

        NexusExtDBService With(string body);
        NexusExtDBService With(string format, params Object[] args);
        //NexusExtDBService With(SqlBuilder sql, string alias);

        #endregion   
    }
}
