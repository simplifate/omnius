using FSS.Omnius.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.BussinesObjects.DAL
{
    public class UnitWork :IUnitWork
    {
        private DBEntities Context { get; set; }

        public UnitWork(DBEntities context)
        {
            Context = context;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
