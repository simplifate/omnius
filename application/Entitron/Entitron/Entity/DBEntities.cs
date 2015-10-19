namespace Entitron.Entity
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

        public virtual DbSet<Modules> CORE_Modules { get; set; }
        public virtual DbSet<Applications> Master_Applications { get; set; }
        public virtual DbSet<Css> Mozaic_Css { get; set; }
        public virtual DbSet<Pages> Mozaic_Pages { get; set; }
        public virtual DbSet<Template> Mozaic_Template { get; set; }
        public virtual DbSet<TemplateCategories> Mozaic_TemplateCategories { get; set; }
        public virtual DbSet<ActionCategories> Tapestry_ActionCategories { get; set; }
        public virtual DbSet<ActionRole_Action> Tapestry_ActionRole_Action { get; set; }
        public virtual DbSet<ActionRoles> Tapestry_ActionRoles { get; set; }
        public virtual DbSet<Actions> Tapestry_Actions { get; set; }
        public virtual DbSet<Actors> Tapestry_Actors { get; set; }
        public virtual DbSet<AttributeRoles> Tapestry_AttributeRoles { get; set; }
        public virtual DbSet<Blocks> Tapestry_Blocks { get; set; }
        public virtual DbSet<WorkFlow_Types> Tapestry_WorkFlow_Types { get; set; }
        public virtual DbSet<WorkFlows> Tapestry_WorkFlows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Applications>()
                .HasMany(e => e.Mozaic_Pages)
                .WithRequired(e => e.Master_Applications)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Applications>()
                .HasMany(e => e.Tapestry_WorkFlows)
                .WithRequired(e => e.Master_Applications)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Css>()
                .HasMany(e => e.Mozaic_Pages)
                .WithMany(e => e.Mozaic_Css)
                .Map(m => m.ToTable("Mozaic_CssPages").MapLeftKey("CssId").MapRightKey("PageId"));

            modelBuilder.Entity<Template>()
                .HasMany(e => e.Mozaic_Pages)
                .WithRequired(e => e.Mozaic_Template)
                .HasForeignKey(e => e.MasterTemplateId);

            modelBuilder.Entity<TemplateCategories>()
                .HasMany(e => e.Mozaic_Template)
                .WithRequired(e => e.Mozaic_TemplateCategories)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<TemplateCategories>()
                .HasMany(e => e.Mozaic_TemplateCategories1)
                .WithOptional(e => e.Mozaic_TemplateCategories2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<ActionCategories>()
                .HasMany(e => e.Tapestry_ActionCategories1)
                .WithOptional(e => e.Tapestry_ActionCategories2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<ActionCategories>()
                .HasMany(e => e.Tapestry_Actions)
                .WithRequired(e => e.Tapestry_ActionCategories)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<ActionRoles>()
                .HasMany(e => e.Tapestry_ActionRole_Action)
                .WithRequired(e => e.Tapestry_ActionRoles)
                .HasForeignKey(e => e.ActionRoleId);

            modelBuilder.Entity<Actions>()
                .HasMany(e => e.Tapestry_ActionRole_Action)
                .WithRequired(e => e.Tapestry_Actions)
                .HasForeignKey(e => e.ActionId);

            modelBuilder.Entity<Actors>()
                .HasMany(e => e.Tapestry_ActionRoles)
                .WithRequired(e => e.Tapestry_Actors)
                .HasForeignKey(e => e.ActorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Blocks>()
                .HasMany(e => e.Tapestry_ActionRoles)
                .WithRequired(e => e.Tapestry_Blocks)
                .HasForeignKey(e => e.SourceBlockId);

            modelBuilder.Entity<Blocks>()
                .HasMany(e => e.Tapestry_ActionRoles1)
                .WithRequired(e => e.Tapestry_Blocks1)
                .HasForeignKey(e => e.TargetBlockId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Blocks>()
                .HasMany(e => e.Tapestry_AttributeRoles)
                .WithRequired(e => e.Tapestry_Blocks)
                .HasForeignKey(e => e.BlockId);

            modelBuilder.Entity<Blocks>()
                .HasOptional(e => e.Tapestry_WorkFlows1)
                .WithRequired(e => e.Tapestry_Blocks1);

            modelBuilder.Entity<WorkFlow_Types>()
                .HasMany(e => e.Tapestry_WorkFlows)
                .WithRequired(e => e.Tapestry_WorkFlow_Types)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkFlows>()
                .HasMany(e => e.Tapestry_Blocks)
                .WithRequired(e => e.Tapestry_WorkFlows)
                .HasForeignKey(e => e.WorkFlowId);

            modelBuilder.Entity<WorkFlows>()
                .HasMany(e => e.Tapestry_WorkFlows1)
                .WithOptional(e => e.Tapestry_WorkFlows2)
                .HasForeignKey(e => e.ParentId);
        }
    }
}
