using System;
using System.Collections.Generic;
using System.Text;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    interface IPage<T> where T : IQueryable
    {
        T Limit(int count);
        T Page(int page, int count);
    }

    public class Page
    {
        public int RowsPerPage { get; set; }
        public int PageIndex { get; set; }
    }
}
