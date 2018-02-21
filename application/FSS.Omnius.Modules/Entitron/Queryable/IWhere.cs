using System;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    interface IWhere<T> where T : IQueryable
    {
        T Where(Func<ConditionColumn, ConditionConcat> condition);
    }
}
