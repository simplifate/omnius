using System;
using System.Collections.Generic;
using System.Text;

namespace FSS.Omnius.Modules.Entitron.Queryable.Cond
{
    public class ConditionConcat
    {
        internal ConditionConcat(Manager<Condition> condition)
        {
            _cond = condition;
            string tabloidName = _cond.i.tabloidName;
            _cond.Next();
            _cond.i.tabloidName = tabloidName;
        }
        
        private Manager<Condition> _cond;

        public ConditionColumn And
        {
            get
            {
                _cond.i.concat = "AND";

                return new ConditionColumn(_cond);
            }
        }
        public ConditionColumn Or
        {
            get
            {
                _cond.i.concat = "OR";

                return new ConditionColumn(_cond);
            }
        }
    }
}
