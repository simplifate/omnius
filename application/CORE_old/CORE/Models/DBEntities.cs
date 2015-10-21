namespace CORE.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBEntities : DbContext
    {
        public DBEntities()
            : base("name=DBEntities")
        {
        }

        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<WorkFlowType> WorkFlow_Types { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<ActionRole> ActionRoles { get; set; }
        public virtual DbSet<AttributeRule> AttributeRoles { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<ActionRole_Action> ActionRole_Action { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<ActionCategory> ActionCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // WorkFlow - Application
            modelBuilder.Entity<WorkFlow>()
                        .HasRequired<Application>(wf => wf.Application)
                        .WithMany(a => a.WorkFlows)
                        .HasForeignKey(wf => wf.ApplicationId);

            // WorkFlow - WorkFlow Type
            modelBuilder.Entity<WorkFlow>()
                        .HasRequired<WorkFlowType>(wf => wf.Type)
                        .WithMany(t => t.WorkFlows)
                        .HasForeignKey(wf => wf.TypeId);

            // WorkFlow parent - children
            modelBuilder.Entity<WorkFlow>()
                        .HasOptional<WorkFlow>(wf => wf.Parent)
                        .WithMany(wf => wf.Children)
                        .HasForeignKey(wf => wf.ParentId);

            // WorkFlow - InitBlock
            modelBuilder.Entity<WorkFlow>()
                        .HasKey(wf => wf.InitBlockId);
            modelBuilder.Entity<Block>()
                        .HasOptional<WorkFlow>(b => b.InitToWF)
                        .WithRequired(wf => wf.InitBlock);

            // Block - WorkFlow
            modelBuilder.Entity<Block>()
                        .HasRequired<WorkFlow>(b => b.WorkFlow)
                        .WithMany(wf => wf.Blocks)
                        .HasForeignKey(b => b.WorkFlowId);

            // Attribute Rules - Block
            modelBuilder.Entity<AttributeRule>()
                        .HasRequired<Block>(ar => ar.Block)
                        .WithMany(b => b.AttributeRules)
                        .HasForeignKey(ar => ar.BlockId);

            // Action Role - Source Block
            modelBuilder.Entity<ActionRole>()
                        .HasRequired<Block>(ar => ar.SourceBlock)
                        .WithMany(b => b.SourceToActions)
                        .HasForeignKey(ar => ar.SourceBlockId);

            // Action Role - Target Block
            modelBuilder.Entity<ActionRole>()
                        .HasRequired<Block>(ar => ar.TargetBlock)
                        .WithMany(b => b.TargetToACtions)
                        .HasForeignKey(ar => ar.TargetBlockId);

            // Action Role - Actor
            modelBuilder.Entity<ActionRole>()
                        .HasRequired<Actor>(ar => ar.Actor)
                        .WithMany(a => a.ActionRoles)
                        .HasForeignKey(ar => ar.ActorId);

            // Action Role - Actions
            modelBuilder.Entity<ActionRole_Action>()
                        .HasRequired<ActionRole>(ara => ara.ActionRole)
                        .WithMany(ar => ar.Actions)
                        .HasForeignKey(ara => ara.ActionRoleId);

            // Actions - Action Roles
            modelBuilder.Entity<ActionRole_Action>()
                        .HasRequired<Action>(ara => ara.Action)
                        .WithMany(a => a.ActionRoles)
                        .HasForeignKey(ara => ara.ActionId);

            // Actions - Action Category
            modelBuilder.Entity<Action>()
                        .HasRequired<ActionCategory>(a => a.Category)
                        .WithMany(ac => ac.Actions)
                        .HasForeignKey(a => a.CategoryId);

            // Action parent - children
            modelBuilder.Entity<ActionCategory>()
                        .HasOptional<ActionCategory>(ac => ac.Parent)
                        .WithMany(ac => ac.Children)
                        .HasForeignKey(ac => ac.ParentId);
        }
    }
}
