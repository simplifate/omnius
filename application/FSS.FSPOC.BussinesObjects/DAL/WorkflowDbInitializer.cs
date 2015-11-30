using System.Collections.Generic;
using System.Data.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.BussinesObjects.DAL
{
    public class WorkflowDbInitializer : DropCreateDatabaseIfModelChanges<DBEntities>
    {
        protected override void Seed(DBEntities context)
        {
        }
    }
}