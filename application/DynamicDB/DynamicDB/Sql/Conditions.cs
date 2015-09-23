﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    public class Conditions
    {
        internal SqlQuery _query;

        internal string _columnName = null;
        internal string _sql = "";
        internal string _concat = "";

        public Conditions(SqlQuery query)
        {
            _query = query;
        }

        public Condition_Operators column(string columnName)
        {
            _columnName = columnName;

            return new Condition_Operators(this);
        }

        public override string ToString()
        {
            return _sql;
        }
    }
}
