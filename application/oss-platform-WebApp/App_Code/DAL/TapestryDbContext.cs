using System;
using System.Data.Entity;
using FSPOC.Models.Tapestry;
using FSPOC.Models.Tapestry.Logic;
using Logger;

namespace FSPOC.DAL
{
    public class TapestryDbContext : DbContext
    {
        public DbSet<Commit> Commits { get; set; }

        public TapestryDbContext() : base("TapestryDbContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<TapestryDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Entity<Commit>().HasMany(s => s.MetaBlocks).WithOptional(s => s.ParentCommit);
                modelBuilder.Entity<MetaBlock>().HasMany(s => s.MetaBlocks).WithOptional(s => s.ParentMetaBlock);
                modelBuilder.Entity<MetaBlock>().HasMany(s => s.Blocks).WithRequired(s => s.ParentMetaBlock);
                modelBuilder.Entity<Block>().HasMany(s => s.Rules).WithRequired(s => s.ParentBlock);
                modelBuilder.Entity<Block>().HasOptional(s => s.ToolboxState).WithRequired(s => s.AssociatedBlock);
                modelBuilder.Entity<Rule>().HasMany(s => s.Items).WithRequired(s => s.ParentRule);
                modelBuilder.Entity<Item>().HasOptional(s => s.Output);
                modelBuilder.Entity<Item>().HasMany(s => s.Inputs);
                modelBuilder.Entity<Item>().HasMany(s => s.Properties).WithRequired(s => s.ParentItem);
                modelBuilder.Entity<Item>().HasOptional(s => s.ConditionSet).WithRequired(s => s.AssociatedItem);
                modelBuilder.Entity<ConditionSet>().HasMany(s => s.Conditions);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("An error occurred while Entity Framework was creating a new database for Tapestry. "
                    + "Exception message: {0}", ex.Message));
            }
        }
    }
}
