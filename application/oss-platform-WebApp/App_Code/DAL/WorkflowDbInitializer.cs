using System.Data.Entity;

namespace FSPOC.DAL
{
    public class WorkflowDbInitializer : CreateDatabaseIfNotExists<WorkflowDbContext>
    {
        protected override void Seed(WorkflowDbContext context)
        {
            base.Seed(context);
        }
    }
}