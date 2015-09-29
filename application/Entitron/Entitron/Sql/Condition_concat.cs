using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    public class Condition_concat
    {
        public static Condition_concat Empty()
        {
            return new Condition_concat(new Conditions(new SqlQuery()));
        }

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
        public Conditions and()
        {
            _conditions._concat = " AND ";

            return _conditions;
        }

        public override string ToString()
        {
            return _conditions.ToString();
        }
    }
}
