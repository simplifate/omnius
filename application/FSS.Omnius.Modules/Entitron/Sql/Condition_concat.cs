using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class Condition_concat
    {
        private Conditions _conditions;

        public Condition_concat(Conditions conditions)
        {
            _conditions = conditions;
        }

        public Conditions or()
        {
            _conditions._concat = " OR ";

            return _conditions;
        }
        public Condition_concat or(Condition_concat concat)
        {
            //concat._conditions.skipWhere = true;
            _conditions._sql += " OR " + concat._conditions._sql;
            return new Condition_concat(_conditions);
        }
        public Conditions and()
        {
            _conditions._concat = " AND ";

            return _conditions;
        }
        public Condition_concat and(Condition_concat concat)
        {
            _conditions._sql += " AND " + concat._conditions._sql;
            return new Condition_concat(_conditions);
        }

        public override string ToString()
        {
            return _conditions.ToString();
        }
    }
}
