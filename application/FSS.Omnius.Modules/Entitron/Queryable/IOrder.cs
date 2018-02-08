using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    interface IOrder<T> where T : IQueryable
    {
        T Order(AscDesc ascDesc = AscDesc.Asc, params string[] columnNames);
    }

    public class Order
    {
        public IEnumerable<string> Columns { get; set; }
        public AscDesc AscDesc { get; set; }

        public string ToSql(DBCommandSet set, IDbCommand command)
        {
            return $"ORDER BY {string.Join(",", Columns.Select(c => set.AddQuote(c)))} {AscDesc}";
        }
    }
}
