using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    interface IGroup<T> where T : IQueryable
    {
        T Group(ESqlFunction function = ESqlFunction.none, Func<ConditionColumn, ConditionConcat> having = null, params string[] columns);
    }

    public class GroupBy
    {
        public GroupBy(DBConnection db)
        {
            _db = db;
        }

        private DBConnection _db;

        public IEnumerable<string> Columns { get; set; }
        public ESqlFunction Function { get; set; }
        public Manager<Condition> Having { get; set; }
    }
}
