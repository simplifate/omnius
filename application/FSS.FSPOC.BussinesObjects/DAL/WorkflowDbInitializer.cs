using System.Collections.Generic;
using System.Data.Entity;
using FSS.Omnius.Entitron.Entity.Tapestry;
using FSS.Omnius.Entitron.Entity;

namespace FSS.Omnius.BussinesObjects.DAL
{
    public class WorkflowDbInitializer : DropCreateDatabaseIfModelChanges<DBEntities>
    {
        protected override void Seed(DBEntities context)
        {
        }
    }
}