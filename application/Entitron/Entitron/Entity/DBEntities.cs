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

        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Css> Css { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateCategory> TemplateCategories { get; set; }
        public virtual DbSet<ActionCategory> ActionCategories { get; set; }
        public virtual DbSet<ActionRole_Action> ActionRole_Action { get; set; }
        public virtual DbSet<ActionRole> ActionRoles { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<AttributeRole> AttributeRoles { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<WorkFlow_Type> WorkFlow_Types { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>()
                .HasMany(e => e.Mozaic_Pages)
                .WithRequired(e => e.Master_Applications)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Application>()
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

            modelBuilder.Entity<TemplateCategory>()
                .HasMany(e => e.Mozaic_Template)
                .WithRequired(e => e.Mozaic_TemplateCategories)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<TemplateCategory>()
                .HasMany(e => e.Mozaic_TemplateCategories1)
                .WithOptional(e => e.Mozaic_TemplateCategories2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<ActionCategory>()
                .HasMany(e => e.Tapestry_ActionCategories1)
                .WithOptional(e => e.Tapestry_ActionCategories2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<ActionCategory>()
                .HasMany(e => e.Tapestry_Actions)
                .WithRequired(e => e.Tapestry_ActionCategories)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<ActionRole>()
                .HasMany(e => e.Tapestry_ActionRole_Action)
                .WithRequired(e => e.Tapestry_ActionRoles)
                .HasForeignKey(e => e.ActionRoleId);

            modelBuilder.Entity<Action>()
                .HasMany(e => e.Tapestry_ActionRole_Action)
                .WithRequired(e => e.Tapestry_Actions)
                .HasForeignKey(e => e.ActionId);

            modelBuilder.Entity<Actor>()
                .HasMany(e => e.Tapestry_ActionRoles)
                .WithRequired(e => e.Tapestry_Actors)
                .HasForeignKey(e => e.ActorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Block>()
                .HasMany(e => e.Tapestry_ActionRoles)
                .WithRequired(e => e.Tapestry_Blocks)
                .HasForeignKey(e => e.SourceBlockId);

            modelBuilder.Entity<Block>()
                .HasMany(e => e.Tapestry_ActionRoles1)
                .WithRequired(e => e.Tapestry_Blocks1)
                .HasForeignKey(e => e.TargetBlockId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Block>()
                .HasMany(e => e.Tapestry_AttributeRoles)
                .WithRequired(e => e.Tapestry_Blocks)
                .HasForeignKey(e => e.BlockId);

            modelBuilder.Entity<Block>()
                .HasOptional(e => e.Tapestry_WorkFlows1)
                .WithRequired(e => e.Tapestry_Blocks1);

            modelBuilder.Entity<WorkFlow_Type>()
                .HasMany(e => e.Tapestry_WorkFlows)
                .WithRequired(e => e.Tapestry_WorkFlow_Types)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkFlow>()
                .HasMany(e => e.Tapestry_Blocks)
                .WithRequired(e => e.Tapestry_WorkFlows)
                .HasForeignKey(e => e.WorkFlowId);

            modelBuilder.Entity<WorkFlow>()
                .HasMany(e => e.Tapestry_WorkFlows1)
                .WithOptional(e => e.Tapestry_WorkFlows2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<Block>()
                .HasKey(e => e.MozaicPageId);
            modelBuilder.Entity<Page>()
                .HasOptional(e => e.Block)
                .WithRequired(e => e.MozaicPage);
        }
    }
}
