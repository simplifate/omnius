using System.Data.Entity;
using FSSWorkflowDesigner.Models;

namespace FSSWorkflowDesigner.DAL
{
    public class WorkflowDbContext : DbContext
    {
        public DbSet<Workflow> Workflows { get; set; }

        public WorkflowDbContext() : base("WorkflowDbContext")
        {
            Database.SetInitializer(new WorkflowDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>()
                        .HasMany(s => s.Inputs)
                        .WithRequired(s => s.Activity);
            modelBuilder.Entity<Activity>()
                        .HasMany(s => s.Outputs)
                        .WithRequired(s => s.Activity);
            modelBuilder.Entity<Commit>()
                        .HasMany(s => s.Activities)
                        .WithRequired(s => s.Commit);
            modelBuilder.Entity<Workflow>()
                        .HasMany(s => s.Commits)
                        .WithRequired(s => s.Workflow);
        }
    }
}