namespace FSS.Omnius.Modules.Entitron.Entity
{
    using System.Data.Entity;
    using CORE;
    using Master;
    using Nexus;
    using Entitron;
    using Tapestry;
    using Mozaic;
    using Persona;
    using Hermes;
    using Watchtower;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBEntities : DbContext
    {
        public DBEntities()
            : base(Omnius.Modules.Entitron.Entitron.connectionString)
        {
        }

        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Css> Css { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateCategory> TemplateCategories { get; set; }
        public virtual DbSet<ActionRule_Action> ActionRule_Action { get; set; }
        public virtual DbSet<ActionRule> ActionRules { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<AttributeRule> AttributeRules { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<WorkFlowType> WorkFlowTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<ActionRight> ActionRights { get; set; }
        public virtual DbSet<AppRight> ApplicationRights { get; set; }
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<DbSchemeCommit> DBSchemeCommits { get; set; }
        public virtual DbSet<DbTable> DbTables { get; set; }
        public virtual DbSet<Ldap> Ldaps { get; set; }
        public virtual DbSet<WS> WSs { get; set; }
        public virtual DbSet<ExtDB> ExtDBs { get; set; }
        public virtual DbSet<TapestryDesignerApp> TapestryDesignerApps { get; set; }
        public virtual DbSet<TapestryDesignerBlock> TapestryDesignerBlocks { get; set; }
        public virtual DbSet<Smtp> SMTPs { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailPlaceholder> EmailPlaceholders { get; set; }
        public virtual DbSet<LogItem> LogItems { get; set; }
        public virtual DbSet<WebDavServer> WebDavServers { get; set; }
        public virtual DbSet<FileMetadata> FileMetadataRecords { get; set; }
        public virtual DbSet<FileSyncCache> CachedFiles { get; set; }

        public virtual DbSet<DbColumn> DbColumn { get; set; }
        public virtual DbSet<DbIndex> DbIndex { get; set; }
        public virtual DbSet<DbRelation> DbRelation { get; set; }
        public virtual DbSet<DbView> DbView { get; set; }

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

            modelBuilder.Entity<ActionRule>()
                .HasMany(e => e.ActionRule_Actions)
                .WithRequired(e => e.ActionRule)
                .HasForeignKey(e => e.ActionRuleId);

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
                .HasMany(e => e.AttributeRules)
                .WithRequired(e => e.Block)
                .HasForeignKey(e => e.BlockId);

            modelBuilder.Entity<Block>()
                .HasMany(e => e.InitForWorkFlow)
                .WithOptional(e => e.InitBlock)
                .HasForeignKey(e => e.InitBlockId);

            modelBuilder.Entity<WorkFlowType>()
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

            modelBuilder.Entity<Page>()
                .HasMany(e => e.Blocks)
                .WithOptional(e => e.MozaicPage)
                .HasForeignKey(e => e.MozaicPageId);
            
            modelBuilder.Entity<User>()
                .HasMany(e => e.Groups)
                .WithMany(e => e.Users)
                .Map(m => m.ToTable("Persona_Groups_Users").MapLeftKey("UserId").MapRightKey("GroupId"));

            modelBuilder.Entity<ActionRight>()
                .HasRequired(e => e.Group)
                .WithMany(e => e.ActionRights)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<AppRight>()
                .HasRequired(e => e.Group)
                .WithMany(e => e.ApplicationRights)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<AppRight>()
                .HasRequired(e => e.Application)
                .WithMany(e => e.Rights)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Application>()
                .HasMany(e => e.Tables)
                .WithRequired(e => e.Application)
                .HasForeignKey(e => e.ApplicationId);

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
            modelBuilder.Entity<ActionRule_Action>()
                .HasRequired(a => a.ActionRule)
                .WithMany(a => a.ActionRule_Actions);

            // Nexus
            modelBuilder.Entity<Ldap>();
            modelBuilder.Entity<WS>();
            modelBuilder.Entity<ExtDB>();

            modelBuilder.Entity<FileMetadata>()
                .HasOptional(s => s.WebDavServer);
            modelBuilder.Entity<FileMetadata>()
                .HasOptional(s => s.CachedCopy)
                .WithRequired(s => s.FileMetadata);

            // Tapestry designer
            modelBuilder.Entity<TapestryDesignerApp>()
                .HasMany(s => s.MetaBlocks)
                .WithOptional(s => s.ParentApp);

            modelBuilder.Entity<TapestryDesignerMetaBlock>()
                .HasMany(s => s.MetaBlocks)
                .WithOptional(s => s.ParentMetaBlock);

            modelBuilder.Entity<TapestryDesignerMetaBlock>()
                .HasMany(s => s.Blocks)
                .WithRequired(s => s.ParentMetaBlock);

            modelBuilder.Entity<TapestryDesignerBlock>()
                .HasMany(s => s.BlockCommits)
                .WithRequired(s => s.ParentBlock);

            modelBuilder.Entity<TapestryDesignerBlockCommit>()
                .HasMany(s => s.Rules)
                .WithRequired(s => s.ParentBlockCommit);

            modelBuilder.Entity<TapestryDesignerRule>()
                .HasMany(s => s.Items)
                .WithRequired(s => s.ParentRule);

            // Hermes
            modelBuilder.Entity<Smtp>();
            modelBuilder.Entity<EmailPlaceholder>();

            modelBuilder.Entity<EmailTemplate>()
                .HasMany(s => s.PlaceholderList)
                .WithOptional(s => s.Hermes_Email_Template)
                .HasForeignKey(s => s.Hermes_Email_Template_Id);

            
        }
    }
}
