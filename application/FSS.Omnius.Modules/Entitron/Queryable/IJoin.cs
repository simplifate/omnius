using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    interface IJoin<T> where T : IQueryable
    {
        T Join(string joinTableName, string joinColumnName, string originColumnName);
    }

    public class Join : IMultiple
    {
        public Join(DBConnection db)
        {
            _db = db;
            JoinType = "JOIN";
        }
        private DBConnection _db { get; set; }

        public string joinTableName { get; set; }
        public string joinColumnName { get; set; }
        public string originTableName { get; set; }
        public string originColumnName { get; set; }
        public string JoinType { get; set; }

        public string Separator => " ";

        public string ToSql(DBCommandSet set, IDbCommand command)
        {
            string realJoinTableName = set.ToRealTableName(_db.Application, joinTableName);
            string realOriginTableName = set.ToRealTableName(_db.Application, originTableName);

            return $"{JoinType} {realJoinTableName} ON {realJoinTableName}.{set.QuotesBegin}{joinColumnName}{set.QuotesEnd}={realOriginTableName}.{set.QuotesBegin}{originColumnName}{set.QuotesEnd}";
        }

        public void Start(bool first)
        {
        }
    }
}
