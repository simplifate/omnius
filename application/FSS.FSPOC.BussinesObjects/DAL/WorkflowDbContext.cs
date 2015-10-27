using System;
using System.Collections.Generic;
using System.Data.Entity;
using FSS.FSPOC.BussinesObjects.Entities.Actions;
using FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner;
using FSS.FSPOC.BussinesObjects.Entities.Workflow;
using Action = FSS.FSPOC.BussinesObjects.Entities.Actions.Action;

namespace FSS.FSPOC.BussinesObjects.DAL
{
    public class OmniusDbContext : DbContext,IDbContext
    {
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<DbSchemeCommit> DbSchemeCommits { get; set; }

        public DbSet<ActionRule> ActionRules { get; set; }
        public DbSet<ActionActionRule> ActionActionRules { get; set; }

        public DbSet<ActionCategory> ActionCategories { get; set; }
        public DbSet<Action> Actions { get; set; }

        public OmniusDbContext() : base("OmniusDbContext")
        {
            Database.SetInitializer(new WorkflowDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
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

                //Actions
                modelBuilder.Entity<ActionActionRule>()
                    .HasRequired(a => a.ActionRule)
                    .WithMany(a=>a.ActionActionRules);

                modelBuilder.Entity<ActionActionRule>()
                    .HasRequired(a => a.Action)
                    .WithMany(a=>a.ActionActionRules);

                modelBuilder.Entity<ActionCategory>()
                    .HasMany(a => a.Actions)
                    .WithRequired(a => a.ActionCategory);
                


            }
            catch (Exception ex)
            {
                //Log.Error(String.Format("An error occurred when Entity Framework was creating a new database. "
                //    + "Exception message: {0}", ex.Message));
            }
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void ExecuteSqlCommand(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
