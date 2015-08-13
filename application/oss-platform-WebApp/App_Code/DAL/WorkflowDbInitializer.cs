using System.Data.Entity;
using FSSWorkflowDesigner.Models;

namespace FSSWorkflowDesigner.DAL
{
    public class WorkflowDbInitializer : CreateDatabaseIfNotExists<WorkflowDbContext>
    {
        protected override void Seed(WorkflowDbContext context)
        {
            base.Seed(context);
        }
    }
}