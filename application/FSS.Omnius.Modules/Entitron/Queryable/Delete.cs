using System;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    public class Delete : IQueryable, IWhere<Delete>, IJoin<Delete>
    {
        public Delete(DBConnection db, string tabloidName)
        {
            _tabloidName = tabloidName;
            _db = db;
            
            _conditions = new Manager<Condition>(_db);
            _conditions.i.tabloidName = tabloidName;
            _join = new Manager<Join>(_db);
        }

        private DBConnection _db;
        private string _tabloidName;
        private Manager<Condition> _conditions;
        private Manager<Join> _join;

        public int Run()
        {
            return _db.ExecuteNonQuery(_db.CommandSet.DELETE(_db, _tabloidName, _conditions, _join));
        }

        public Delete Where(Func<ConditionColumn, ConditionConcat> condition)
        {
            _conditions.Start();
            condition(new ConditionColumn(_conditions));

            return this;
        }

        public Delete Join(string joinTableName, string joinColumnName, string originColumnName)
        {
            _join.i.joinTableName = joinTableName;
            _join.i.joinColumnName = joinColumnName;
            _join.i.originTableName = _tabloidName;
            _join.i.originColumnName = originColumnName;
            _join.Next();

            return this;
        }
    }
}
