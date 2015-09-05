using System.Data.Entity;
using FSPOC.Models;

namespace FSPOC.DAL
{
    public class WorkflowDbContext : DbContext
    {
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<DbSchemeCommit> DbSchemeCommits { get; set; }

        public WorkflowDbContext() : base("WorkflowDbContext")
        {
            Database.SetInitializer(new WorkflowDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Workflow Designer
            modelBuilder.Entity<Activity>()
                        .HasMany(s => s.Inputs)
                        .WithRequired(s => s.Activity);
            modelBuilder.Entity<Activity>()
                        .HasMany(s => s.Outputs)
                        .WithRequired(s => s.Activity);
            modelBuilder.Entity<WorkflowCommit>()
                        .HasMany(s => s.Activities)
                        .WithRequired(s => s.WorkflowCommit);
            modelBuilder.Entity<Workflow>()
                        .HasMany(s => s.WorkflowCommits)
                        .WithRequired(s => s.Workflow);

            // Database Designer
            modelBuilder.Entity<DbTable>()
                        .HasMany(s => s.Columns)
                        .WithRequired(s => s.DbTable);
            modelBuilder.Entity<DbTable>()
                        .HasMany(s => s.Indices)
                        .WithRequired(s => s.DbTable);
            modelBuilder.Entity<DbSchemeCommit>()
                        .HasMany(s => s.Tables)
                        .WithRequired(s => s.DbSchemeCommit);
            modelBuilder.Entity<DbSchemeCommit>()
                        .HasMany(s => s.Relations)
                        .WithRequired(s => s.DbSchemeCommit);
            modelBuilder.Entity<DbSchemeCommit>()
                        .HasMany(s => s.Views)
                        .WithRequired(s => s.DbSchemeCommit);
        }
    }
}
