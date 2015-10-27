using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.FSPOC.BussinesObjects.DAL
{
    public class UnitWork :IUnitWork
    {
        private IDbContext Context { get; set; }

        public UnitWork(IDbContext context)
        {
            Context = context;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
