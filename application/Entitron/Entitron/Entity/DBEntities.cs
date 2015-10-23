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
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<ActionRight> ActionRights { get; set; }
        public virtual DbSet<AppRight> ApplicationRights { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>()
                .HasMany(e => e.Pages)
                .WithRequired(e => e.Application)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Application>()
                .HasMany(e => e.WorkFlows)
                .WithRequired(e => e.Application)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Css>()
                .HasMany(e => e.Pages)
                .WithMany(e => e.Css)
                .Map(m => m.ToTable("Mozaic_CssPages").MapLeftKey("CssId").MapRightKey("PageId"));

            modelBuilder.Entity<Template>()
                .HasMany(e => e.Pages)
                .WithRequired(e => e.MasterTemplate)
                .HasForeignKey(e => e.MasterTemplateId);

            modelBuilder.Entity<TemplateCategory>()
                .HasMany(e => e.Templates)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<TemplateCategory>()
                .HasMany(e => e.Children)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<ActionCategory>()
                .HasMany(e => e.Children)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<ActionCategory>()
                .HasMany(e => e.Actions)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<ActionRole>()
                .HasMany(e => e.ActionRole_Actions)
                .WithRequired(e => e.ActionRole)
                .HasForeignKey(e => e.ActionRoleId);

            modelBuilder.Entity<Action>()
                .HasMany(e => e.ActionRole_Actions)
                .WithRequired(e => e.Action)
                .HasForeignKey(e => e.ActionId);

            modelBuilder.Entity<Actor>()
                .HasMany(e => e.ActionRoles)
                .WithRequired(e => e.Actor)
                .HasForeignKey(e => e.ActorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Block>()
                .HasMany(e => e.SourceTo_ActionRoles)
                .WithRequired(e => e.SourceBlock)
                .HasForeignKey(e => e.SourceBlockId);

            modelBuilder.Entity<Block>()
                .HasMany(e => e.TargetTo_ActionRoles)
                .WithRequired(e => e.TargetBlock)
                .HasForeignKey(e => e.TargetBlockId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Block>()
                .HasMany(e => e.AttributeRoles)
                .WithRequired(e => e.Block)
                .HasForeignKey(e => e.BlockId);

            modelBuilder.Entity<Block>()
                .HasOptional(e => e.InitForWorkFlow)
                .WithRequired(e => e.InitBlock);

            modelBuilder.Entity<WorkFlow_Type>()
                .HasMany(e => e.WorkFlows)
                .WithRequired(e => e.Type)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkFlow>()
                .HasMany(e => e.Blocks)
                .WithRequired(e => e.WorkFlow)
                .HasForeignKey(e => e.WorkFlowId);

            modelBuilder.Entity<WorkFlow>()
                .HasMany(e => e.Children)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<Block>()
                .HasKey(e => e.MozaicPageId);
            modelBuilder.Entity<Page>()
                .HasOptional(e => e.Block)
                .WithRequired(e => e.MozaicPage);
            
            modelBuilder.Entity<User>()
                .HasMany(e => e.Groups)
                .WithMany(e => e.Users)
                .Map(m => m.ToTable("Persona_Groups_Users").MapLeftKey("UserId").MapRightKey("GroupId"));

            modelBuilder.Entity<ActionRight>()
                .HasRequired(e => e.Group)
                .WithMany(e => e.ActionRights)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<ActionRight>()
                .HasRequired(e => e.Action)
                .WithMany(e => e.Rigths)
                .HasForeignKey(e => e.ActionId);

            modelBuilder.Entity<AppRight>()
                .HasRequired(e => e.Group)
                .WithMany(e => e.ApplicationRights)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<AppRight>()
                .HasRequired(e => e.Application)
                .WithMany(e => e.Rights)
                .HasForeignKey(e => e.ApplicationId);
        }
    }
}
