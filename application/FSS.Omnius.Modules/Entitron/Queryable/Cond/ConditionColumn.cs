namespace FSS.Omnius.Modules.Entitron.Queryable.Cond
{
    public class ConditionColumn
    {
        internal ConditionColumn(Manager<Condition> condition)
        {
            _cond = condition;
        }

        private Manager<Condition> _cond;
        
        public ConditionOperation Column(string columnName)
        {
            _cond.i.column = columnName.Contains(".") ? columnName : $"{_cond.i.tabloidName}.{columnName}" ;
            return new ConditionOperation(_cond);
        }
    }
}
